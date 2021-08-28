<template>
    <div class="container-fluid shadow doc-comp">
      <div class="row payment-header">
          <div class="col">
                <h1>{{name}}</h1>
          </div>
      </div>
    <div class="row payment py-4">
        <div class="col">
            <h3>Schedule Payment</h3>
        </div>
    </div>
    <div class="row payment">
        <div class="col-6">
            <input type="number" v-model="amount" class="form-control" placeholder="Amount" step="0.10" required/>
        </div>
        <div class="col-6">
            
            <input type="datetime-local" v-model="scheduled" class="form-control" placeholder="Scheduled Date" required/>
        </div>
        <div class="col-6">
              <button class="btn btn-primary" @click="schedulePayment">Schedule Payment</button>
        </div>
    </div>
    </div>      
</template>

<script>

export default {
    name: 'Payment',
    props: {
         name: String
         },
    data: () => ({
      amount: 0.0,
      scheduled:'',
      result:'',

    }),
    methods: {
        schedulePayment() {
            console.log('amount',  this.amount);
            console.log('scheduled', this.scheduled);

            var model = {amount: this.amount, scheduledAt: this.scheduled };
            var _this = this;
            var headers = new Headers();
            headers.append("Content-Type", "application/json");

            var raw = JSON.stringify(model);

            var requestOptions = {
                method: 'POST',
                headers: headers,
                body: raw,
                redirect: 'follow'
            };
            fetch('/payment/schedule', requestOptions)
            .then(function(d) {
                console.log(d);
            });

        },
    },
}

</script>

<style  scoped>
    .payment-header {
        background: rgb(132 187 144);
        padding: 1rem;
    }
    .payment {
        background: rgb(68 142 60);
    }

</style>