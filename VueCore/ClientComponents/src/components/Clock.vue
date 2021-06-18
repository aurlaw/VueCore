<template>
    <div class="clock-wrapper row justify-content-md-center">
      <div class="col-sm">
        <analog-clock v-if="tick" :minute="time.minutes" :tick="tick"></analog-clock>
      </div>
      <div class="col-sm">
        <text-clock :time="time" v-if="tick"></text-clock>
      </div>
    </div>
</template>
<script>
import TextClock from './clock-comp/TextClock.vue'
import AnalogClock from './clock-comp/AnalogClock.vue'

export default {
  name: 'Clock',
  components: {
      TextClock,
      AnalogClock
  },
  data() {
    return {
      tick: 0,
      time: { hours: 0, minutes: 0, seconds: 0 }
    }
  },
  methods: {
    updateTime(time) {
      this.tick++
      this.time = {
        hours: time.getHours(),
        minutes: time.getMinutes(),
        seconds: time.getSeconds()
      }

      setTimeout(() => this.updateTime(new Date()), 1000 - new Date().getMilliseconds())
    }
  },  
  mounted() {
    this.updateTime(new Date())
  }
}
</script>
<style scoped>
    .clock-wrapper {
        /* position: relative; */
        margin: 2rem;
        min-height: 200px;
    }
    
</style>