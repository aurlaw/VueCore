<template>
  <div class="container-fluid shadow doc-comp">
      <div class="row">
          <div class="col">
                <h2>{{name}}</h2>
          </div>
      </div>
    <div class="row">
        <div class="col">
            List/Filter documents.<br>
            Add PDF file field<br>
            Extract PDF content, save to Lucene index<br>
            Search index <br>
            <hr>
            Workflow
            <ul>
                <li>Activity: Save file to Azure Storage</li>
                <li>Activity: Update Document fileurl property</li>
                <li>Activity: 
                    <ul>
                        <li>Extract text from PDF</li>
                        <li>Save extracted txt to lucene index</li>
                    </ul>
                </li>
                <li>Activity: Send email with completion sucess</li>
            </ul>
        </div>
    </div>
    <div class="row doc-entry py-4">
        <div class="col">
            <h3>Add Document</h3>
        </div>
    </div>
    <div class="row doc-entry">
        <div class="col-6">
            <input type="text" v-model="title" class="form-control" placeholder="Enter Name" required/>
        </div>
        <div class="col-6">
            <textarea cols="10" rows="5" v-model="notes" class="form-control" placeholder="Enter Notes"></textarea>
        </div>
        <div class="col-12">
          <vue-dropzone ref="dropzone" id="drop1" 
              :options="dropOptions" 
              @vdropzone-file-added="onFileAdded"
              @vdropzone-removed-file="onRemovedFile"
              @vdropzone-success="onUploadSuccess"></vue-dropzone>
        </div>
        <div class="col-6">
              <button class="btn btn-primary" @click="processDocument">Create Document</button>
              <button class="btn btn-danger" @click="removeAllFiles">Remove All Files</button>
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
import vueDropzone from "vue2-dropzone";

export default {
    name: 'DocumentManager',
    props: {
         name: String
         },
    data: () => ({
        dropOptions: {
        url: "/document/upload",
        maxFilesize: 4, // MB
        maxFiles: 1,
        acceptedFiles: "application/pdf",
        autoProcessQueue: false,
        //   chunking: true,
        //   chunkSize: 500, // Bytes
        // thumbnailWidth: 150, // px
        // thumbnailHeight: 150,
        addRemoveLinks: true,
        dictDefaultMessage: "<i class='fa fa-cloud-upload'></i> - Add PDF only. Max 1. Max file size 4MB"
        },
      title: '',
      notes:'',
      result:'',
      files: [],
      processedFiles:[],
      isProcessing:false,
    }),
    components: {
        vueDropzone
    },
    computed: {
        hasResult() {
            return this.result !== '';
        }
    },
    methods: {
        removeAllFiles() {
            this.$refs.dropzone.removeAllFiles();
            this.files = [];
        },
        processDocument() {            
            this.isProcessing = true;
            const headers =  { "x-vuecore-title": this.title , "x-vuecore-notes": this.notes};
            this.$refs.dropzone.setOption("headers", headers);
            console.log("Process document");
            //this.$refs.dropzone.processQueue();
        },
        onFileAdded(file) {
            this.files.push(file.name);
        },    
        onUploadSuccess(file, response) {
            //   console.log(file);
            this.isProcessing = false;
            console.log(response);
            if(response.success) {
                this.processedFiles.push(response);
            }
        },    
        onRemovedFile(file, error, xhr) {
            if(file && file.name) {
                this.files = this.files.filter(f => f !== file.name);            
            }        
        },               
        // createDoc() {
        //     const model = {name: this.title, notes: this.notes};
        //     // console.log('Save Document', model);
        //     var _this = this;
        //     var headers = new Headers();
        //     headers.append("Content-Type", "application/json");
        //     var raw = JSON.stringify(model);
        //     var requestOptions = {
        //         method: 'POST',
        //         headers: headers,
        //         body: raw,
        //         redirect: 'follow'
        //     };
        //     fetch('/document/save', requestOptions)
        //     .then(function(d) {
        //         console.log(d);
        //         _this.result = "Saved";
        //         _this.title = '';
        //         _this.notes = '';
        //     });
        // },
    }
}

</script>
<style  scoped>
    .doc-comp {
        background: rgb(216, 203, 219);
        padding: 1rem;
    }
    .doc-entry {
        background: rgb(179, 146, 187);

    }

</style>