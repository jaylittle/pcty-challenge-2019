/* Configure XSRF Token */
let xsrfToken = window.appState.xsrfToken;
if (xsrfToken) {
  Vue.http.headers.common['xsrf-form-token'] = xsrfToken;
}

import appHelpers from "./libraries/app-helpers";
window.appHelpers = appHelpers;

import genericHeader from "./components/generic/header.vue"
Vue.component('generic-header', genericHeader);