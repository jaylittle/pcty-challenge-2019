<template>
  <div>
    <generic-header></generic-header>
    <div class="container">
      <h1>Placeholder</h1>
    </div>
  </div>
</template>

<script>
  export default {
    name: "login",
    mounted() {
    },
    data() {
      return {
        state: window.appState,
        messages: []
      };
    },
    computed: {
    }
  };
</script>