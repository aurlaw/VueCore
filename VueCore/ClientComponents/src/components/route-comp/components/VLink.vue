<template>
  <a
    :href="href"
    @click.prevent="go">
    <slot></slot>
  </a>
</template>

<script>
import routes from '../routes'
export default {
  props: {
    href: {
      type:String,
      required: true
    }
  },
  methods: {
    go () {
        window.bus.$emit('currentRoute', this.href);
    //   this.$root.$root.currentRoute = this.href
      window.history.pushState(null, "", this.href)
    //   history.go();
    }
  }
}
</script>