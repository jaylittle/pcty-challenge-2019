<template>
  <div>
    <generic-header></generic-header>
    <form method="POST" action="token/issue">
      <input type="hidden" name="successUrl" :value="successUrl" />
      <input type="hidden" name="failUrl" :value="state.relativeBaseUrl + 'log/in'" />
      <div class="container">
        <h3>{{ state.fullTitle }}</h3>
        <b-alert show dismissible v-for="message in messages" v-bind:key="message.text" v-bind:variant="message.variant || 'danger'">{{ message.text }}</b-alert>
        <div class="row">
          <div class="col-sm">
            <div class="form-group">
              <label for="userName">User Name</label>
              <input type="text" id="userName" name="userName" class="form-control" placeholder="Enter User Name" ref="fldUserName">
            </div>
          </div>
        </div>
        <div class="row">
          <div class="col-sm">
            <div class="form-group">
              <label for="password">Password</label>
              <input type="password" id="password" name="password" class="form-control" placeholder="Password">
            </div>
          </div>
        </div>
        <div class="row">
          <div class="col-sm">
            <button type="submit" class="btn btn-primary">Login</button>
            <a :href="state.authAzureLoginUrl" class="btn btn-secondary" v-if="state.authAzureADEnable">Login with Azure AD</a>
          </div>
        </div>
      </div>
    </form>
  </div>
</template>

<script>
  export default {
    name: "login",
    mounted() {
      let queryParams = appHelpers.url.parseQueryString();
      if (queryParams.expired) {
        this.messages.push(appHelpers.message.create('Your login token has expired.  Please log back into the system.', 'danger'));
      }
      if (queryParams.manual) {
        this.messages.push(appHelpers.message.create('You logged out of the system by clicking the Logout button.', 'danger'));
      }
      if (queryParams.authFailed) {
        this.messages.push(appHelpers.message.create('Your attempt to authenticate failed.  Please try again.', 'danger'));
      }
      this.successUrl = this.state.relativeBaseUrl + (queryParams.successUrl ? queryParams.successUrl : 'home');
      if (this.state.authDirectEnable || this.state.authActiveDirectoryEnable)
      {
        this.$refs.fldUserName.focus();
      }
    },
    data() {
      return {
        state: window.appState,
        successUrl: '',
        messages: [
          { text: 'Hint: Try logging in with the username and password of Tester!', variant: 'info' }
        ]
      };
    },
    computed: {
    }
  };
</script>