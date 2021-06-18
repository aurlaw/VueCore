<template>
<section>
<ul class="nav nav-tabs">
    <li class="nav-item"><v-link class="nav-link" :class="setActiveClass('/home/vueroute')" :href="getLink('/home/vueroute')">Home</v-link></li>
    <li class="nav-item"><v-link class="nav-link" :class="setActiveClass('/home/vueroute/about')" :href="getLink('/home/vueroute/about')">About</v-link></li>
    <li class="nav-item"><v-link class="nav-link" :class="setActiveClass('/home/vueroute/foo')" :href="getLink('/home/vueroute/foo')">Under Construction</v-link></li>
  </ul>
  <article class="shadow">
    <div class="container">
      <slot></slot>
    </div>
  </article>
</section>
</template>

<script>
import VLink from '../components/VLink.vue'
export default {
  name: 'MainLayout',
  components: {
    VLink
  },
  methods: {
      getLink(linkPath) {
          return linkPath;
      },
      setActiveClass(linkPath) {
        return window.location.pathname === linkPath ? 'active' : '';
      },
  }
}
</script>
<style scoped>
  article {
    padding: 1rem;
    min-height: 300px;
  }
</style>