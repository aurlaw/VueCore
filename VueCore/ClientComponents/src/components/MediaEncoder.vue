<template>
  <div class="container">
    <div class="row">
        <div class="col media-comp">
            <h2>{{name}}</h2>
            <input type="text" v-model="title" class="form-control" placeholder="Enter Title" required/>
              <vue-dropzone ref="dropzone" id="drop1" 
                  :options="dropOptions" 
                  @vdropzone-file-added="onFileAdded"
                  @vdropzone-removed-file="onRemovedFile"
                  @vdropzone-success="onUploadSuccess"></vue-dropzone>
              <button class="btn btn-primary" @click="processFiles">Process File</button>
              <button class="btn btn-danger" @click="removeAllFiles">Remove File</button>

          <section class="panel">
            <div class="alert alert-info" role="alert">
              {{hubStatus}}
            </div>        
            <div class="spinner-border text-success" role="status" v-if="isProcessing">
              <span class="sr-only">Loading...</span>
            </div>          

            <div class="alert alert-secondary" role="alert" v-if="message.length">
              {{message}}
            </div>     
          </section>
        </div>
        <div class="col video-comp">
         <div class="row row-cols-1" id="video-player">
           <div class="col">
              <div class="jumbotron"  v-if="activeMediaJob != null">
                <h3 class="display-4">{{activeMediaJob.title}}</h3>
                <hr class="my-4">
                  <div class="d-flex">
                    <vue-video v-bind:hlsSrc="activeVideo" class="flex-fill" />
                  </div>
              </div>             
           </div>
         </div>   
          <div class="row row-cols-1 row-cols-md-2">
            <div class="col mb-4" v-for="item in processedData" :key="item.jobName">
              <div class="card h-100">
                <img v-bind:src="item.thumbnail" v-bind:alt="item.outputAssetName" class="card-img-top">
                <div class="card-body">
                  <h5 class="card-title">{{item.title}}</h5>
                  <p class="card-text"><strong>Job:</strong> {{item.jobName}}<br>
                  <strong>Input Asset:</strong> {{item.inputAssetName}}<br>
                  <strong>Output Asset:</strong> {{item.outputAssetName}}</p>
                  <button class="btn btn-primary" type="button" @click="launchVideo(item)">Launch Video</button>
                  <button class="btn btn-danger" @click="onDeleteMedia(item)">Delete</button>
                </div>
              </div>
            </div>
          </div>
<!-- 
          <section class="video-info" v-for="item in processedData" :key="item.jobName">
            <ul>
              <li><strong>Job:</strong> {{item.jobName}}</li>
              <li><strong>Input Asset:</strong> {{item.inputAssetName}}</li>
              <li><strong>Output Asset:</strong> {{item.outputAssetName}}</li>
              <li>
                  <strong>Stream Urls:</strong>
                <ul class="small">
                  <li v-for="(url, index) in item.streamUrlList" :key="index">
                    {{url}}
                  </li>
                </ul>
                <small>HLS URL: {{getHLSUrl(item.streamUrlList)}}</small>
              </li>
            </ul>
            <div class="player">
              <vue-video v-bind:hlsSrc="getHLSUrl(item.streamUrlList)" />
            </div>
              <button class="btn btn-danger" @click="onDeleteMedia(item)">Delete</button>
          </section> -->
        </div>
    </div>
  </div>
</template>
<script>
import vueDropzone from "vue2-dropzone";
import {HubConnectionBuilder} from "@microsoft/signalr";
import {saveObject, removeKey, getObject} from "../utilites/storage";
import VueVideo from "./VueVideo";

export default {
  name: 'MediaEncoder',
  props: {
    name: String
  },
  data: () => ({
    dropOptions: {
        url: "/media/upload",
        maxFilesize: 150, // MB
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
    title: '',
    files: [],
    processedData: [],
    hubStatus: '',
    message: '',
    hubConn: {},
    groupId: '',
    activeMediaJob: null,
    isProcessing:false,

  }),
  components: {
      vueDropzone,
      VueVideo
  },
  computed: {
    activeVideo: function() {
      if(this.activeMediaJob != null) {
        return this.getHLSUrl(this.activeMediaJob.streamUrlList);
      }
      return '';
    },
  },
  methods: {
    deleteMedia(mediaJob) {
        var _this = this;
        var headers = new Headers();
        headers.append("Content-Type", "application/json");

        var raw = JSON.stringify(mediaJob);

        var requestOptions = {
            method: 'POST',
            headers: headers,
            body: raw,
            redirect: 'follow'
        };
        fetch('/media/removeMedia', requestOptions)
        .then(function(d) {
          console.log(d);
            _this.processedDataRemove(mediaJob);
        });
    },    
    configureHub() {
      const _this = this;

      this.hubConn = new HubConnectionBuilder()
        .withUrl('/mediaprocessHub')
        .withAutomaticReconnect()
        .build();

      this.hubConn.onclose((err) => {
          // console.assert(connection.state === signalR.HubConnectionState.Disconnected);
          _this.setHubStatus( `Connection closed due to error "${err}". Try refreshing this page to restart the connection.`);
          if(err) {
              console.error('onclose', err.toString());
          }
          // console.log('onclose');
      });
      this.hubConn.onreconnecting((err) => {
          // console.assert(connection.state === signalR.HubConnectionState.Reconnecting);
          _this.setHubStatus(`Connection lost due to error "${err}". Reconnecting.`);
          if(err) {
              console.error('onreconnecting', err.toString());
          }
          // console.log('onreconnecting');
      });
      this.hubConn.onreconnected((connectionId) => {
          // console.assert(connection.state === signalR.HubConnectionState.Connected);
          _this.setHubStatus(`Connection reestablished. Connected with connectionId "${connectionId}".`);
          console.error('onreconnected', connectionId);
          console.log('onreconnected');
      });

      // app events
      this.hubConn.on("SetGroupId", function(groupId) {
        console.log('setting group id', groupId);
        _this.groupId = groupId;
        _this.setMessage(`Retrieved Group Id: ${_this.groupId}`);
      });
      //
      this.hubConn.on("SendUploaded", function(fileName) {
        console.log('SendUploaded', fileName);
        _this.setMessage(`Uploading...: ${fileName}`);
      });
      this.hubConn.on("SendReceived", function(mediaJob) {
        console.log('SendReceived', mediaJob);
        _this.isProcessing = false;
        _this.processedDataAdd(mediaJob);
        _this.removeAllFiles();
        _this.title = '';
        _this.setMessage('Received media info');
      });
      this.hubConn.on("SendProgress", function(message) {
        console.log('SendProcessed', message);
        _this.setMessage(`Progress: ${message}`);
      });
      this.hubConn.on("SendError", function(msg) {
        console.log('SendError', msg);
        _this.setMessage(`Error: ${msg}`);
      });

      // signalR connection
      console.log('connecting...', this.hubConn);
      this.hubConn.start().then(function() {
          console.log('started hub connection...');
          _this.setHubStatus(`${_this.hubConn.state} to hub`);
          _this.getGroupId();
      })
      .catch(function (err) {
          _this.setHubStatus(_this.hubConn.state + " " + err.toString());
          return console.error('connectHub', err.toString());
      });
    },
    setHubStatus(msg) {
      this.hubStatus = msg;
    },
    setMessage(msg) {
      this.message = msg;
    },
    getGroupId() {
      const _this = this;
      this.hubConn.invoke("GenerateGroupId").catch(function(err) {
            _this.setMessage(_this.hubConn.state + " " + err.toString());
            return console.error('GenerateGroupId', err.toString());
        });
    },
    getHLSUrl(streamingUrlList) {
      const hlsType = 'format=m3u8-aapl';
      if(streamingUrlList.length) {
          return streamingUrlList.find(url => url.includes(hlsType));
      }
    },
    launchVideo(mediaJob) {
      // console.log('launchVideo', mediaJob);
      this.activeMediaJob = mediaJob;
      document.getElementById('video-player').scrollIntoView();
      //
    },
    removeAllFiles() {
    //   this.deleteFromApi();
      this.$refs.dropzone.removeAllFiles();
      this.files = [];
      // this.processedDataPrune();
    },
    processFiles() {
      const headers =  { "x-vuecore-groupid": this.groupId, "x-vuecore-title": this.title };
      this.$refs.dropzone.setOption("headers", headers);
      this.isProcessing = true;
      this.$refs.dropzone.processQueue();
    },
    onFileAdded(file) {
        this.files.push(file.name);
    },
    onUploadSuccess(file, response) {
    //   console.log(file);
      console.log(response);
      // if(response.success) {
      //   this.processedFiles.push(response);
      // }
    },
    onRemovedFile(file, error, xhr) {
        if(file && file.name) {
           this.files = this.files.filter(f => f !== file.name);            
        }        
    },
    onDeleteMedia(mediaJob) {
      if(confirm("Are you sure you want to delete this item?"))
      {
        this.deleteMedia(mediaJob);
      }
    },
    processedDataAdd(mediaJob) {
      this.processedData.push(mediaJob);
      saveObject('mediaJobArr', this.processedData);
    },
    processedDataRemove(mediaJob) {
      console.log('processedDataRemove', mediaJob);
      this.processedData = this.processedData.filter(f => f.jobName !== mediaJob.jobName);            
      saveObject('mediaJobArr', this.processedData);
    },
    processedDataPrune() {
      this.processedData = [];
      removeKey('mediaJobArr');
    },
    processedDataLoad() {
      const data = getObject('mediaJobArr');
      // console.log('processedDataLoad', data);
      if(data != null && Array.isArray(data)) {
        this.processedData = data;
      }
    }
  },
  mounted() {
    this.setHubStatus('Mounted...');
    this.configureHub();
    this.processedDataLoad();
  }  
}


</script>
<style  scoped>
    .media-comp {
        background: rgb(151 178 202);
        padding: 1rem;
    }
    .media-comp .panel {
      margin: 2rem auto;
      background: #fff;
      padding: 1rem;
    }
    .video-comp {
      overflow: hidden;
      background: rgb(151 178 202);
    }
    .video-info {
      padding: 1rem;
      background: #fff;
      border: 1px solid #ccc;
      margin: 1rem auto;
    }
    .video-info ul {
      list-style: none;
    }
    /* .video-info .player {
      width: 40%;
    } */

</style>