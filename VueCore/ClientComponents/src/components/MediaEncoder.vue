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

      <section class="panel">
        <div class="alert alert-info" role="alert">
          {{hubStatus}}
        </div>        
        <div class="alert alert-secondary" role="alert" v-if="message.length">
          {{message}}
        </div>        
      </section>
    </div>
</template>
<script>
import vueDropzone from "vue2-dropzone";
import {HubConnectionBuilder} from "@microsoft/signalr";

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
    processedFiles: [],
    hubStatus: '',
    message: '',
    hubConn: {},
    groupId: '',
  }),
  components: {
      vueDropzone
  },
  methods: {
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
        _this.processedFiles.push(mediaJob.streamUrl);
        _this.setMessage(`Received: ${mediaJob.streamUrl}`);
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
    removeAllFiles() {
    //   this.deleteFromApi();
      this.$refs.dropzone.removeAllFiles();
      this.processedFiles = [];
    },
    processFiles() {
      const headers =  { "x-vuecore-groupid": this.groupId };
      this.$refs.dropzone.setOption("headers", headers);

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
  },
  mounted() {
    this.setHubStatus('Mounted...');
    this.configureHub();
  }  
}


</script>
<style  scoped>
    .media-comp {
        border: 1px solid #ccc;
        background: rgba(43, 73, 241, 0.25);
        padding: 1rem;
    }
    .media-comp .panel {
      margin: 2rem auto;
      background: #fff;
      padding: 1rem;
    }

</style>