<template>
    <div class="comment">
        Comment Box<br>
        Id: {{postId}} <br>
        Comment: {{postComment}} 
    </div>
</template>

<script>
export default {
  name: 'CommentBox',
  props: {
    postId: String,
    postComment: String
  }
}
</script>
<style scoped>
    .comment {
        border: 1px solid #000;
    }
</style>
