<template>
<div>
  <ul>
    <li><v-link :href="getLink('/home/vueroute')">Home</v-link></li>
    <li><v-link :href="getLink('/home/vueroute/about')">About</v-link></li>
    <li><v-link :href="getLink('/home/vueroute/foo')">Under Construction</v-link></li>
  </ul>
  <slot></slot>
</div>
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
      }
  }
}
</script>