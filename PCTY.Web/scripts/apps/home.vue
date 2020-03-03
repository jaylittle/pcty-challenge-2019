<template>
  <div>
    <generic-header></generic-header>
    <router-view></router-view>
  </div>
</template>

<script>

  import homeEmployee from '../components/home/employee.vue'

  const homeRoutes = [
    { name: 'Employees', path: '/', component: homeEmployee },
    { path: '*', component: { template: '<h1>Route Not Found</h1>' } }
  ];

  const homeRouter = new VueRouter({
    mode: 'history',
    base: `${window.appState.relativeBaseUrl}/`,
    routes: homeRoutes
  });
  
  export default {
    name: "home",
    router: homeRouter,
    mounted() {
      window.spa = this;
    }
  };
</script>