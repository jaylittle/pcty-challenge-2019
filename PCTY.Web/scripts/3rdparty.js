import "core-js";
import "regenerator-runtime";
import Vue from "vue";
import VueResource from "vue-resource";
import VueRouter from "vue-router";

Vue.use(VueResource);
Vue.use(VueRouter);

window.Vue = Vue;
window.VueResource = VueResource;
window.VueRouter = VueRouter;

import BootstrapVue from "bootstrap-vue";

Vue.use(BootstrapVue);

window.BootstrapVue = BootstrapVue;