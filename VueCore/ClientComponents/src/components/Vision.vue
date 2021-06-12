<template>
    <div class="vision">
        <h2>{{name}}</h2>
          <vue-dropzone ref="dropzone" id="drop1" 
              :options="dropOptions" 
              @vdropzone-file-added="onFileAdded"
              @vdropzone-removed-file="onRemovedFile"
              @vdropzone-success="onUploadSuccess"></vue-dropzone>
          <button class="btn btn-primary" @click="processFiles">Process Files</button>
          <button class="btn btn-danger" @click="removeAllFiles">Remove All Files</button>
          <hr>
      <div class="spinner-border text-success" role="status" v-if="isProcessing">
        <span class="sr-only">Loading...</span>
      </div>        
        <div class="row">
          <div class="col-9 my-2" v-bind:class="{ 'offset-3': (index +1) % 2 === 0 }" v-for="(item, index) in processedFiles" :key="item.url">
            <image-card 
              :id="getVisionId(index)"
              :imgSrc="item.thumbnailUrl"
              :name="getVisionName(item)" 
              :description="getVisionDescription(item)"
              :tags="getVisionCategory(item)"
              category="Image">
                <div>
                  <div class="p-2">
                      <span v-bind:style="setColor(item.analysis.color.accentColor)">Accent: {{item.analysis.color.accentColor}} </span>, 
                      <span v-bind:style="setColor(item.analysis.color.dominantColorBackground)">Background: {{item.analysis.color.dominantColorBackground}} </span>, 
                      <span v-bind:style="setColor(item.analysis.color.dominantColorForeground)">Foreground: {{item.analysis.color.dominantColorForeground}}  </span>
                  </div>
                  <vue-picture border-color="#0f0" v-bind:border-width="3" text-color="#fff"
                    v-bind:imgSrc="item.url" 
                    v-bind:detectedObjects="item.analysis.objects" />
                </div>
              </image-card>

<!-- 


              <div class="p-2">
                  <span v-bind:style="setColor(item.analysis.color.accentColor)">Accent: {{item.analysis.color.accentColor}} </span>, 
                  <span v-bind:style="setColor(item.analysis.color.dominantColorBackground)">Background: {{item.analysis.color.dominantColorBackground}} </span>, 
                  <span v-bind:style="setColor(item.analysis.color.dominantColorForeground)">Foreground: {{item.analysis.color.dominantColorForeground}}  </span>
              </div>

            <section class="section">
              <vue-picture border-color="#0f0" v-bind:border-width="3" text-color="#fff"
                v-bind:imgSrc="item.url" 
                v-bind:detectedObjects="item.analysis.objects" />
            </section>

            <section v-if="item.analysis.descriptions.length" class="section">
              <h3>Summary:</h3>
              <div v-for="(desc, index) in item.analysis.descriptions" :key="setKey('d', index)">
                {{desc.caption}} <small>({{desc.confidence}})</small>
                <br />
              <button class="btn btn-danger" type="button" @click="onDeleteImg(item)">Delete</button>
              </div>
            </section>

            <section class="section">
                <div v-bind:style="setTheme(item.analysis.color)">
                  <figure class="figure">
                    <img :src="item.thumbnailUrl" class="img-fluid rounded" />
                    <figcaption class="figure-caption">Image Theme</figcaption>
                  </figure>                  
                </div>
            </section> -->
<!--             
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
 -->

          </div>
        </div>          
    </div>
</template>
<script>
import vueDropzone from "vue2-dropzone";
import vuePicture from './ui/VuePicture';
import imageCard from './ui/ImageCard';
import {saveObject, removeKey, getObject} from "../utilites/storage";

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
      addRemoveLinks: true,
      dictDefaultMessage: "<i class='fa fa-cloud-upload'></i> - Add images only. Max 4. Max file size 4MB"
      
    },
    files: [],
    processedFiles:[],
    isProcessing:false,
  }),
  components: {
      vueDropzone,
      vuePicture,
      imageCard
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
    getVisionId(idx) {
      return 'v_' + idx;
    },
    getVisionName(imageItem) {
      if(imageItem.analysis.descriptions.length) {
        return imageItem.analysis.descriptions[0].caption;
      }
      return 'No Name';
    },
    getVisionDescription(imageItem) {
      let desc  = '';
      if(imageItem.analysis.tags.length) {
        desc = imageItem.analysis.tags.map(function(tag) {
          return tag.caption;
        }).join(",");
      }
      return desc;
    },
    getVisionCategory(imageItem) {
      let desc  = '';
      if(imageItem.analysis.categories.length) {
        desc = imageItem.analysis.categories.map(function(tag) {
          return tag.name;
        }).join(",");
      }
      return desc;
    },
    removeAllFiles() {
    //   this.deleteFromApi();
      this.$refs.dropzone.removeAllFiles();
      this.files = [];
      // this.processedFiles = [];
      // removeKey('visionArr');
    },
    processFiles() {
      this.isProcessing = true;
      this.$refs.dropzone.processQueue();
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
        saveObject('visionArr', this.processedFiles);

      }
    },
    onRemovedFile(file, error, xhr) {
        if(file && file.name) {
           this.files = this.files.filter(f => f !== file.name);            
        }        
    },
    onDeleteImg(imgData) {
      if(confirm("Are you sure you want to delete this item?"))
      {
        console.log(imgData);
      }
    },
    setTheme(themeColor) {
      return {
        'margin': '0.5rem',
        'padding': '0.5rem',
        'background': themeColor.dominantColorBackground,
        'color': themeColor.dominantColorForeground + ' !important',
        'border': '1px solid ' + themeColor.accentColor
        
      }
    },
    setColor(color) {
      return {
        'color': color
      }
    },
    setKey(prefix, keyId) {
      return prefix + '_' + keyId;
    },
    processedDataLoad() {
      const data = getObject('visionArr');
      // console.log('processedDataLoad', data);
      if(data != null && Array.isArray(data)) {
        this.processedFiles = data;
      }
    }
  },
  mounted() {
    this.processedDataLoad();
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
    hr {
      border-width: 3px !important;
    }

</style>
