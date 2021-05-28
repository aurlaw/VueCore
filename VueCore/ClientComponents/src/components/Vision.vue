<template>
    <div class="vision">
        Vision: {{name}}<br>
          <vue-dropzone ref="dropzone" id="drop1" 
              :options="dropOptions" 
              @vdropzone-file-added="onFileAdded"
              @vdropzone-removed-file="onRemovedFile"
              @vdropzone-success="onUploadSuccess"></vue-dropzone>
          <button class="btn btn-primary" @click="processFiles">Process Files</button>
          <button class="btn btn-danger" @click="removeAllFiles">Remove All Files</button>
          <hr>
        <ul class="list">
          <li v-for="item in processedFiles" :key="item.url">
            <img v-bind:src="item.url" class="img-fluid" />
          </li>
        </ul>          
    </div>
</template>

<script>
import vueDropzone from "vue2-dropzone";


export default {
  name: 'Vision',
  props: {
    name: String
  },
  data: () => ({
    dropOptions: {
      url: "/Vision/upload",
      maxFilesize: 2, // MB
      maxFiles: 4,
      acceptedFiles: "image/*",
      autoProcessQueue: false,
    //   chunking: true,
    //   chunkSize: 500, // Bytes
      thumbnailWidth: 150, // px
      thumbnailHeight: 150,
      addRemoveLinks: true      
    },
    files: [],
    processedFiles:[],
  }),
  components: {
      vueDropzone
  },
  methods: {
    deleteFromApi() {
        if(this.files.length === 0)
        {
            return;
        }
        var headers = new Headers();
        headers.append("Content-Type", "application/json");

        var raw = JSON.stringify(this.files);

        var requestOptions = {
            method: 'POST',
            headers: headers,
            body: raw,
            redirect: 'follow'
        };
        fetch('/Vision/RemoveUpload', requestOptions)
        .then(d => console.log(d));
    },
    removeAllFiles() {
    //   this.deleteFromApi();
      this.$refs.dropzone.removeAllFiles();
    },
    processFiles() {
      this.$refs.dropzone.processQueue();
    },
    onFileAdded(file) {
        this.files.push(file.name);
    },
    onUploadSuccess(file, response) {
    //   console.log(file);
      console.log(response);
      if(response.result) {
        this.processedFiles.push(response);
      }
    },
    onRemovedFile(file, error, xhr) {
        if(file && file.name) {
           this.files = this.files.filter(f => f !== file.name);            
        }        
    },
  }  
}
</script>
<style  scoped>
    .vision {
        border: 1px solid #080;
        background: rgba(0, 146, 19, 0.25);
        padding: 1rem;

    }
    .vision  .list {
      width: 50%;
    }
</style>
