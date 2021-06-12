<template>
    <div>
        <a class="card-link" data-toggle="modal" :data-target="hashId">
            <article class="img-card">
                <img class="post-image" :src="imgSrc" />
                <div class="article-details">
                    <h4 class="post-category">{{category}}</h4>
                    <h3 class="post-title">{{name}}</h3>
                    <p class="post-description">
                        {{description}}
                    </p>
                    <p class="post-author">
                        {{tags}}
                    </p>
                </div>
            </article>
        </a>
        <!-- Modal -->
        <div class="modal fade" :id="id" tabindex="-1" role="dialog" aria-labelledby="Image Detail" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabel">{{name}}</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <slot></slot>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
export default {
	name: "ImageCard",
    props: {
        id:String,
        name: String,
        description: String,
        tags: String,
        category: String,
        imgSrc:String
    },
    methods: {
        showDetails() {
            alert('Show Modal');
        },
    },
    computed: {
        hashId() {
            return '#' + this.id;
        }
    },
}
</script>


