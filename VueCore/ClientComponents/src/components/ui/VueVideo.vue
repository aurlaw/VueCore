<template>
    <video-player :options="videoOptions" ref="videoPlayer" />

</template>

<script>
import VideoPlayer from "./VideoPlayer";

export default {
	name: "VueVideo",
	components: {
		VideoPlayer
	},
    props: {
        hlsSrc: String,
        autoplay: {
            type:Boolean,
            default:false
        }
    },
	data() {
		return {
			videoOptions: {
				autoplay: this.autoplay,
				controls: true,
				sources: [
					{
						src: this.hlsSrc,
                        type: "application/x-mpegURL"
					}
				]
			}
		};
	},
	watch: {
		hlsSrc: function(newSrc, oldSrc) {
			this.$refs.videoPlayer.changeSource(newSrc, "application/x-mpegURL");
		}
	}
};

</script>
