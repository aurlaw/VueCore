<template>
    <div class="pic" v-if="show">
        <img ref="img" v-bind:src="imgSrc" class="img-fluid" @load="triggerLoad" />
        <div class="box" v-for="(obj, index) in detectedObjects" v-bind:style="drawBorder(obj.rectangle)" :key="index"><span>{{obj.property}}</span></div>
    </div>
    <div v-else class="d-flex my-1 align-items-center">
      <strong>Loading image...</strong>
      <div class="spinner-border text-primary ml-auto" role="status" aria-hidden="true"></div>
    </div>    
</template>

<script>
export default {
  name: 'VuePicture',
  props: {
    imgSrc: String,    
    detectedObjects: Array,
    borderColor: {
      type: String,
      default: '#f00'
    },
    borderWidth: {
      type: Number,
      default: 1
    },
    textColor: {
      type: String,
      default: '#000'
    },
    isVisible: {
      type:Boolean,
      default:true
    }
  },
  data: () => ({
      imgLoaded: false,
      actualW: 0,
      actualH: 0,
      naturalW: 0,
      naturalH: 0,
      adjW: 0,
      adjH: 0,
      visibility:false
  }),
  computed: {
    show() {
      if(this.isVisible) {
        return true;
      } else {
        return this.visibility;
      }
    },
  },
  methods: {
    drawBorder(rect) {
        const x = rect.x * this.adjW;
        const y = rect.y * this.adjH;
        const w = rect.w * this.adjW;
        const h = rect.h * this.adjH;

        const style = {
            'top': y + 'px',
            'left': x + 'px',
            'width': w + 'px',
            'height': h + 'px',
            'color': this.textColor,
            'border': this.borderWidth + 'px solid ' + this.borderColor
        };
        return style;
    },
    drawObjects() {
      if(!this.show) {
        return;
      }
        // console.log(this.detectedObjects);
        this.actualW = this.$refs.img.clientWidth;
        this.actualH = this.$refs.img.clientHeight;
        this.naturalW = this.$refs.img.naturalWidth;
        this.naturalH = this.$refs.img.naturalHeight;

        this.adjW = this.actualW / this.naturalW;
        this.adjH = this.actualH / this.naturalH;
        // console.log(`adj W: ${this.adjW} | adj H: ${this.adjH}`);
        this.imgLoaded = true;
    },      
    triggerLoad () {
      this.drawObjects();
    },
    setVisibilty(isVisible) {
      let _this = this;
      setTimeout(function(){ 
         _this.visibility = isVisible;
         }, 1000);
    },
  },
}
</script>
<style scoped>
    .pic {
        margin: 0;
        padding: 0;
        position: relative;
    }
    .pic img {
        margin: 0;
        padding: 0;
    }
    .pic .box {
        position: absolute;
    }
</style>
