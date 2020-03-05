<template>
  <div class="content">
    <b-alert :show="message.countdown || true" fade dismissible v-for="message in messages" :key="message.text" :variant="message.variant || 'danger'">{{ message.text }}</b-alert>
    <div class="row">
      <div class="col-md-12">
        <h5 class="border-bottom pb-3">
          Employees
          <button class="float-right btn btn-primary btn-sm mx-1" @click="showEmployee()">Add Employee</button>
        </h5>
      </div>
    </div>
    <div class="row">
      <div class="col-sm-12">
        <div class="form-group">
          <input type="text" class="form-control" v-model="filters.searchText" placeholder="Type to Search" ref="fldSearchText" />
        </div>
      </div>
    </div>
    <b-table :items="employees" :fields="fields" :filter="filters.searchText">
      <template v-slot:cell(actions)="data">
        <button class="btn btn-primary btn-sm" @click="showEmployee(data.item.guid)">Edit</button>
        <button class="btn btn-secondary btn-sm" @click="deleteEmployee(data.item.guid)">Delete</button>
      </template>
    </b-table>
    <b-modal ref="modalEmployee" size="lg" :title="employeeTitle" @ok="upsertEmployee" @hidden="hiddenEmployee">
      <b-alert :show="message.countdown || true" fade dismissible v-for="message in modalMessages" :key="message.text" :variant="message.variant || 'danger'">{{ message.text }}</b-alert>
      <div class="row px-1">
        <div class="col-md-12">
          <h5 class="pb-3">
            General
            <button class="float-right btn btn-info btn-sm mx-1" @click="calculateBenefitCost()">Recalc Benefit Cost</button>
          </h5>
        </div>
      </div>
      <div class="form-row pt-2 border-top">
        <div class="form-group col-md-5">
          <label>First Name</label>
          <input type="text" class="form-control d-print-none" v-model="editEmployee.firstName">
        </div>
        <div class="form-group col-md-2">
          <label>Middle Initial</label>
          <input type="text" class="form-control d-print-none" v-model="editEmployee.middleInitial">
        </div>
        <div class="form-group col-md-5">
          <label>Last Name</label>
          <input type="text" class="form-control d-print-none" v-model="editEmployee.lastName">
        </div>
        <div class="form-group col-md-6">
          <label>Yearly Salary</label>
          <input type="text" class="form-control d-print-none" v-model="editEmployee.yearlySalary">
          <small class="form-text text-muted">
            Per Check: {{ perCheckSalary }}
          </small>
        </div>
        <div class="form-group col-md-6 text-right">
          <label>Yearly Benefit Cost</label>
          <div> {{ formatCurrency(editEmployee.benefitCost) }}</div>
          <small class="form-text text-muted">
            Per Check: {{ perCheckBenefitCost }}
          </small>
        </div>
      </div>
      <div class="row px-1">
        <div class="col-md-12">
          <h5 class="pb-3">
            Dependents {{ employeeDependentCount }}
            <button class="float-right btn btn-success btn-sm mx-1" @click="addDependent()">Add Dependent</button>
          </h5>
        </div>
      </div>
      <div class="form-row pt-2 border-top" v-for="(dependent, index) in editEmployee.dependents" :key="index">
        <div class="form-group col-md-5">
          <label>First Name</label>
          <input type="text" class="form-control d-print-none" v-model="dependent.firstName">
        </div>
        <div class="form-group col-md-2">
          <label>Middle Initial</label>
          <input type="text" class="form-control d-print-none" v-model="dependent.middleInitial">
        </div>
        <div class="form-group col-md-5">
          <label>Last Name</label>
          <input type="text" class="form-control d-print-none" v-model="dependent.lastName">
        </div>
        <div class="form-group col-md-5">
          <label>Relationship</label>
          <input type="text" class="form-control d-print-none" v-model="dependent.relationship">
        </div>
        <div class="form-group col-md-7">
          <button class="float-right btn btn-danger btn-sm mx-1" @click="removeDependent(dependent)">Remove Dependent</button>
        </div>
      </div>
    </b-modal>
  </div>
</template>

<script>
  export default {
    name: "system-user",
    created() {
      this.init();
    },
    mounted() {
      this.$refs.fldSearchText.focus();
    },
    watch: {
      'filters.showDeleted': 'init'
    },
    data() {
      return {
        state: window.appState,
        fields: [
          { key: 'actions', label: 'Actions' },
          { key: 'lastName', label: 'Last Name', sortable: true, stickyColumn: true },
          { key: 'firstName', label: 'First Name', sortable: true, stickyColumn: true },
          { key: 'middleInitial', label: 'Middle Initial', sortable: true },
          { key: 'yearlySalary', label: 'Yearly Salary', sortable: true, formatter: this.formatCurrency },
          { key: 'benefitCost', label: 'Benefit Cost', sortable: true, formatter: this.formatCurrency },
        ],
        employees: [],
        editEmployee: {

        },
        dependentCtr: 0,
        filters: {
          searchText: '',
          sortField: '',
          sortDescending: false
        },
        totalRows: 0,
        messages: [],
        modalMessages: [],
        payChecksPerYear: 26.00,
        defaultPayCheckAmount: 2000.00
      };
    },
    computed: {
      employeeTitle() {
        if (this.editEmployee.editFlag) {
          return `Editing Employee ${this.editEmployee.lastName}, ${this.editEmployee.firstName} ${this.editEmployee.middleInitial}`;
        } else {
          return 'Adding New Employee';
        }
      },
      employeeDependentCount() {
        if (this.editEmployee.dependents && this.editEmployee.dependents.length) {
          return `(${this.editEmployee.dependents.length})`;
        }
        return '';
      },
      perCheckBenefitCost() {
        return this.formatCurrency((this.editEmployee.benefitCost || 0) / this.payChecksPerYear);
      },
      perCheckSalary() {
        return this.formatCurrency((this.editEmployee.yearlySalary || 0) / this.payChecksPerYear);
      }
    },
    methods: {
      formatDate: appHelpers.formatters.formatDate,
      formatCurrency: appHelpers.formatters.formatCurrency,
      init() {
        this.messages = [ ];
        this.pageSize = 100;
        appHelpers.loader.start();
        return apiClient.employee.list().then((response) => {
          this.employees = response.body;
          this.totalRows = this.employees.length;
        }, (response) => {
          this.messages = appHelpers.message.getFromResponse(response);
        }).finally(() => {
          appHelpers.loader.stop();
        });
      },
      onFiltered (filteredItems) {
        this.totalRows = filteredItems.length;
      },
      showEmployee(employeeGuid) {
        if (employeeGuid) {
          appHelpers.loader.start();
          apiClient.employee.get(employeeGuid).then((response) => {
            this.editEmployee = response.body;
            this.editEmployee.editFlag = true;
            this.$refs.modalEmployee.show();
          }, (response) => {
            this.messages = appHelpers.message.getFromResponse(response);
          }).finally(() => {
            appHelpers.loader.stop();
          });
        } else {
          this.editEmployee = {
            yearlySalary: this.payChecksPerYear * this.defaultPayCheckAmount,
            benefitCost: 1000.00,
            editFlag: false,
            dependents: [ ]
          };
          this.$refs.modalEmployee.show();
        }
      },
      upsertEmployee(modalEvent) {
        appHelpers.loader.start();
        this.modalMessages = [ ];
        return apiClient.employee.upsert(this.editEmployee).then((response) => {
          this.hideEmployee();
          this.init();
        }, (response) => {
          this.modalMessages = appHelpers.message.getFromResponse(response);
          modalEvent.preventDefault();
          this.$refs.modalEmployee.show();
        }).finally(() => {
          appHelpers.loader.stop();
        });
      },
      deleteEmployee(employeeGuid) {
        appHelpers.loader.start();
        this.messages = [ ];
        return apiClient.employee.delete(employeeGuid).then((response) => {
          this.init();
        }, (response) => {
          this.messages = appHelpers.message.getFromResponse(response);
          modalEvent.preventDefault();
          this.$refs.modalEmployee.show();
        }).finally(() => {
          appHelpers.loader.stop();
        });
      },
      hideEmployee() {
        this.$refs.modalEmployee.hide();
      },
      hiddenEmployee() {
        this.editEmployee = { };
      },
      addDependent() {
        this.dependentCtr++;
        this.editEmployee.dependents = this.editEmployee.dependents || [ ];
        this.editEmployee.dependents.push({ firstName: `Dependent ${this.dependentCtr}`, lastName: this.editEmployee.lastName, middleInitial: '', relationship: 'Child' });
      },
      removeDependent(dependent) {
        let dependentIndex = this.editEmployee.dependents.findIndex((d) => d === dependent);
        if (dependentIndex >= 0) {
          this.editEmployee.dependents.splice(dependentIndex, 1);
        }
      },
      calculateBenefitCost() {
        appHelpers.loader.start();
        this.modalMessages = [ ];
        return apiClient.employee.calculateBenefitCost(this.editEmployee).then((response) => {
          this.editEmployee.benefitCost = parseFloat(response.body);
        }, (response) => {
          this.modalMessages = appHelpers.message.getFromResponse(response);
        }).finally(() => {
          appHelpers.loader.stop();
        });
      }
    }
  };
</script>