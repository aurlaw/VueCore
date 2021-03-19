<template>
    <div class="privacy">
          Privacy Policy Name : {{policyName}}
    </div>
</template>

<script>
export default {
  name: 'Privacy',
  props: {
    policyName: String
  }
}
</script>
<style scoped>
    .privacy {
        border: 1px solid #090;
        background: rgba(0, 153, 0, 0.25);
    }
</style>
