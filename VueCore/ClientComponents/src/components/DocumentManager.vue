<template>
  <div class="container-fluid shadow doc-comp">
      <div class="row">
          <div class="col">
                <h2>{{name}}</h2>
          </div>
      </div>
    <div class="row">
        <div class="col-6">
            <input type="text" v-model="title" class="form-control" placeholder="Enter Name" required/>
        </div>
        <div class="col-6">
            <textarea cols="10" rows="5" v-model="notes" class="form-control" placeholder="Enter Notes"></textarea>
        </div>
        <div class="col-6">
              <button class="btn btn-primary" @click="createDoc">Create Document</button>
        </div>
        <div class="col-6" v-if="hasResult">
            <div class="alert alert-info" role="alert">
              {{result}}
            </div>        
        </div>
    </div>
  </div>
</template>
<script>
export default {
    name: 'DocumentManager',
    props: {
         name: String
         },
    data: () => ({
      title: '',
      notes:'',
      result:'',
    }),
    components: {},
    computed: {
        hasResult() {
            return this.result !== '';
        }
    },
    methods: {
        createDoc() {
            const model = {name: this.title, notes: this.notes};
            // console.log('Save Document', model);
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
            fetch('/document/save', requestOptions)
            .then(function(d) {
                console.log(d);
                _this.result = "Saved";
                _this.title = '';
                _this.notes = '';
            });
        },
    }
}

</script>
<style  scoped>
    .doc-comp {
        background: rgb(196, 168, 204);
        padding: 1rem;
    }

</style>