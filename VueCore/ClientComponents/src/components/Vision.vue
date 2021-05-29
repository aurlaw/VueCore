<template>
    <div class="vision">
        <h2>Vision: {{name}}</h2>
          <vue-dropzone ref="dropzone" id="drop1" 
              :options="dropOptions" 
              @vdropzone-file-added="onFileAdded"
              @vdropzone-removed-file="onRemovedFile"
              @vdropzone-success="onUploadSuccess"></vue-dropzone>
          <button class="btn btn-primary" @click="processFiles">Process Files</button>
          <button class="btn btn-danger" @click="removeAllFiles">Remove All Files</button>
          <hr>
        <div class="list">
          <div v-for="item in processedFiles" :key="item.url">
            <div>
                <span v-bind:style="setColor(item.analysis.color.accentColor)">Accent: {{item.analysis.color.accentColor}} </span>, 
                <span v-bind:style="setColor(item.analysis.color.dominantColorBackground)">Background: {{item.analysis.color.dominantColorBackground}} </span>, 
                <span v-bind:style="setColor(item.analysis.color.dominantColorForeground)">Foreground: {{item.analysis.color.dominantColorForeground}}  </span>
            </div>
            <section v-if="item.analysis.descriptions.length" class="section">
              <h3>Summary:</h3>
              <div v-for="(desc, index) in item.analysis.descriptions" :key="setKey('d', index)">
                {{desc.caption}} <small>({{desc.confidence}})</small>
              </div>
            </section>
            <section v-if="item.analysis.tags.length" class="section">
              <h3>Tags:</h3>                
              <span v-for="(tag, index) in item.analysis.tags" class="tag" :key="setKey('t', index)">
                {{tag.caption}} <small>({{tag.confidence}})</small> 
              </span>
            </section>
            <section v-if="item.analysis.categories.length" class="section">
              <h3>Categories:</h3>
              <div v-for="(tag, index) in item.analysis.categories" :key="setKey('c', index)">
                {{tag.name}} <small>({{tag.score}})</small>
              </div>
            </section>
            <vue-picture border-color="#0f0" v-bind:border-width="3" text-color="#fff"
              v-bind:imgSrc="item.url" 
              v-bind:detectedObjects="item.analysis.objects" />
          </div>
        </div>          
    </div>
</template>
<script>
import vueDropzone from "vue2-dropzone";
import vuePicture from './VuePicture';

export default {
  name: 'Vision',
  props: {
    name: String
  },
  data: () => ({
    dropOptions: {
      url: "/Vision/upload",
      maxFilesize: 4, // MB
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
      vueDropzone,
      vuePicture
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
    setColor(color) {
      return {
        'color': color
      }
    },
    setKey(prefix, keyId) {
      return prefix + '_' + keyId;
    }
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
      width: 80%;
      background: #fff;
    }
    .vision  .section {
      border: 1px solid #666;
      box-shadow: 2px 2px 6px 0px #666;
      margin: 1rem auto;
      padding: 1rem;
    }
    .vision  .section .tag {
      padding: 2px;
      font-style: italic;
      border: 1px solid #333;
      margin: 0.25rem;
      display: inline-block;    
    }
</style>
