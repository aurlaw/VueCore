<template>
    <div class="ww shadow">
          <h2>{{name}}</h2>
          Count: {{count}}<br>
          WW Count: {{wwCount}}
          <hr>
          <button class="btn btn-primary" @click="updateCount">Update Counts</button>
          <button class="btn btn-secondary" @click="stopCount">Stop Counts</button>
          <hr>
          <button class="btn btn-info" @click="launchNonWorker">Launch Non-Worker</button>
          <button class="btn btn-success" @click="launchWorker">Launch Worker</button>
    </div>
</template>

<script>
import Users from '../data/users';
import worker from '../utilites/Worker';
import WebWorkerSetup from '../utilites/WorkerSetup';

export default {
  name: 'WebWorker',
  props: {
    name: String
  },
  data: () => ({
    ww:null,
    interalId:null,
    count:0,
    wwCount:0
  }),
  methods: {
    updateCount() {
      let _this = this;
      this.interalId = setInterval(function(){ _this.count++; }, 300);
    },
    stopCount() {
      if(this.interalId) {
        clearInterval(this.interalId);
        this.count = 0;
      }
    },
    launchNonWorker() {
        this.wwCount = 0;
        const users = Users();
        this.wwCount = users.length;
    },
    launchWorker() {
        // convert user function to string and pass as message to worker
        // this is a work around since we cannot pass the function directly
      this.wwCount = 0;
      const users = Users.toString();
      if(this.ww) {
        let _this = this;
        console.log('post to worker');
        this.ww.postMessage({users});
        this.ww.addEventListener("message", event => {
          console.log(event);
          _this.wwCount = event.data.length;
        });
      }

    },
  },

  mounted() {
    this.ww = new WebWorkerSetup(worker);
  },
  unmounted() {
    if(this.ww) {
      this.ww.terminate();
    }
  },

}
</script>
<style scoped>
    .ww {
        border: 1px solid rgb(195, 238, 255);
        background: rgba(0, 105, 153, 0.25);
        padding: 1rem;
    }
</style>
