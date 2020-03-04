/* API Client Library */
let self = module.exports = {
  fixUrl(currentUrl) {
    if (currentUrl.startsWith("/"))
    {
      let baseUrl = document.getElementsByTagName('base')[0].href;
      baseUrl = baseUrl.endsWith("/") ? baseUrl.substring(0, baseUrl.length - 1) : baseUrl;
      return `${baseUrl}${currentUrl}`;
    }
    return currentUrl;
  },
  employee: {
    list() {
      return Vue.http.get(self.fixUrl(`/api/employee`));
    },
    get(employeeId) {
      return Vue.http.get(self.fixUrl(`/api/employee/${employeeId}`));
    },
    delete(employeeId) {
      return Vue.http.delete(self.fixUrl(`/api/employee/${employeeId}`));
    },
    upsert(employee) {
      return Vue.http.post(self.fixUrl(`/api/employee/`), employee);
    },
    calculateBenefitCost(employee) {
      return Vue.http.post(self.fixUrl(`/api/employee/action/calculate-benefit-cost`), employee);
    }
  },
  state: {
    isAuthorized(tenantId) {
      return Vue.http.get(self.fixUrl(`/api/state/isAuthorized`));
    }
  },
  token: {
    refresh() {
      return Vue.http.get(self.fixUrl('/token/refresh'));
    }
  }
};