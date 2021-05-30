<template>
    <div class="media-comp">
        <h2>{{name}}</h2>
          <vue-dropzone ref="dropzone" id="drop1" 
              :options="dropOptions" 
              @vdropzone-file-added="onFileAdded"
              @vdropzone-removed-file="onRemovedFile"
              @vdropzone-success="onUploadSuccess"></vue-dropzone>
          <button class="btn btn-primary" @click="processFiles">Process File</button>
          <button class="btn btn-danger" @click="removeAllFiles">Remove File</button>
    </div>
</template>
<script>
import vueDropzone from "vue2-dropzone";

export default {
  name: 'MediaEncoder',
  props: {
    name: String
  },
  data: () => ({
    dropOptions: {
      url: "/media/upload",
      maxFilesize: 70, // MB
      maxFiles: 1,
      acceptedFiles: "video/*",
      autoProcessQueue: false,
    //   chunking: true,
    //   chunkSize: 500, // Bytes
      thumbnailWidth: 150, // px
      thumbnailHeight: 150,
      addRemoveLinks: true,
      dictDefaultMessage: "<i class='fa fa-cloud-upload'></i> - Add video only. Max file size 70MB"
      
    },
    files: [],
    processedFiles:[],
  }),
  components: {
      vueDropzone
  },
  methods: {
    removeAllFiles() {
    //   this.deleteFromApi();
      this.$refs.dropzone.removeAllFiles();
      this.processedFiles = [];
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
      if(response.success) {
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
    .media-comp {
        border: 1px solid #ccc;
        background: rgba(43, 73, 241, 0.25);
        padding: 1rem;

    }

</style>