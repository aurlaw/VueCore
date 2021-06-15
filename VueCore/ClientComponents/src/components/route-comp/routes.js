const Home = () => import('./pages/Home.vue');
const About = () => import('./pages/About.vue');


export default {
    '/': Home,
    '/about': About
  }