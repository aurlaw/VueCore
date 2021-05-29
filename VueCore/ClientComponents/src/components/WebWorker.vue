<template>
    <div class="ww">
          Web Workers: {{name}} TODO
    </div>
</template>

<script>
export default {
  name: 'WebWorker',
  props: {
    name: String
  }
}
</script>
<style scoped>
    .ww {
        border: 1px solid #009;
        background: rgba(0, 0, 153, 0.25);
        padding: 1rem;
    }
</style>
