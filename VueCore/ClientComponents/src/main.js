import Vue from 'vue'
// import App from './App.vue'
const App = () => import('./App');
const CommentBox = () => import('./components/CommentBox.vue');
const Privacy = () => import('./components/Privacy.vue');
const WebWorker = () => import('./components/WebWorker.vue');
const Vision = () => import('./components/Vision.vue');

Vue.config.productionTip = false

const APPS = { 
    App,
    CommentBox,
    Privacy,
    WebWorker,
    Vision
};

function renderAppInElement(el) {
    let id = el.id;
    const idArr = id.split('_');
    if(idArr.length > 1) {
        id = idArr[idArr.length-1];
    }

    const App = APPS[id];
    if (!App) return;

    // // get props from elements data attribute, like the post_id
    const props = Object.assign({}, el.dataset);
  
    new Vue({
        render: h => h(App, {props})
      }).$mount(el);
}
// eslint-disable-next-line no-undef
document.querySelectorAll('.__vue-root').forEach(renderAppInElement);
