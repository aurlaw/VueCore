<template>
  <div class="app">
    <HelloWorld msg="Welcome to Your Vue.js App">
      <section>
        <hr/>
        <Clock />
      </section>
    </HelloWorld>
  </div>
</template>

<script>
import HelloWorld from './components/HelloWorld.vue'
import Clock from './components/Clock.vue'

export default {
  name: 'App',
  components: {
    HelloWorld,
    Clock
  }
}
</script>

<style>
.app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
