<template>
  <div id="appWebHeader" class="pb-3">
    <b-navbar toggleable="md" :type="navType" :variant="navVariant">

      <b-navbar-brand>
        {{ state.title }}
      </b-navbar-brand>

      <b-navbar-toggle target="nav-collapse" class="float-right"></b-navbar-toggle>

      <!-- Left aligned nav items -->
      <b-navbar-nav class="d-none d-md-flex">
      </b-navbar-nav>

      <!-- Right aligned nav items -->
      <b-collapse id="nav-collapse" is-nav>
        <b-navbar-nav class="ml-none ml-md-auto d-block d-md-flex">
          
          <b-nav-item-dropdown class="d-inline-block d-md-flex" v-bind:text="state.userName" right>
            <b-dropdown-item href="#" v-if="!state.hasAdmin" v-on:click="login()">Login</b-dropdown-item>
            <b-dropdown-item href="#" v-if="state.hasAdmin" v-on:click="logout()">Logout</b-dropdown-item>
          </b-nav-item-dropdown>

        </b-navbar-nav>
      </b-collapse>

    </b-navbar>
    <div class="loader-modal" v-show="loaderVisible">
      <vue-simple-spinner size="massive" :line-size="10"></vue-simple-spinner>
    </div>
  </div>
</template>

<script>
  export default {
    name: "generic-header",
    components: { },
    mounted() {
      appHelpers.loader.register(this.loader);
    },
    data() {
      return {
        state: window.appState,
        headerMessages: [],
        loaderVisible: false,
      };
    },
    methods: {
      handleExpiration(response) {
        window.location = appHelpers.url.fix("/log/out?expired=true");
      },
      login(event) {
        window.location = appHelpers.url.fix("/log/in");
      },
      logout(event) {
        window.location = appHelpers.url.fix("/log/out");
      },
      loader(visible) {
        this.loaderVisible = visible;
      },
    },
    computed: {
      navVariant() {
        return 'dark';
      },
      navType() {
        return 'dark';
      },
    }
  };
</script>

<style>
  .loader-modal {
    position: fixed;
    top: 0;
    left: 0;
    height: 100vh;
    width: 100vw;
    padding-top: 45vh;
    background-color: black;
    opacity: 0.8;
    z-index: 2000;
  }
</style>