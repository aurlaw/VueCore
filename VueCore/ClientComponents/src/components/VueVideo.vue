<template>
    <video-player :options="videoOptions"/>
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
	}
};

</script>