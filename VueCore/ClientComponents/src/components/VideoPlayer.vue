<template>
        <video ref="videoPlayer" class="video-js vjs-theme-sea"></video>
</template>

<script>
import videojs from 'video.js';
//
export default {
    name: "VideoPlayer",
    props: {
        options: {
            type: Object,
            default() {
                return {};
            }
        }
    },
    data() {
        return {
            player: null
        }
    },
    methods: {
        changeSource(src, type) {
            if(this.player != null) {
                const newSrc = {src, type};
                this.player.src(newSrc);
            }
        }
    },
    mounted() {
        this.player = videojs(this.$refs.videoPlayer, this.options, function onPlayerReady() {
            console.log('onPlayerReady', this);
        })
    },

    beforeDestroy() {
        if (this.player) {
            this.player.dispose()
        }
    }
}
</script>