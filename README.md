# VueCore

[View Demo Site](https://aurlaw-vuecore-gbvki.ondigitalocean.app/)

.NET 5 with Vue components. Uses Parcel for build environment of Vue. 

This is not a SPA, it is a .NET Core MVC app utlizing individual Vue components - see ```VueCore/ClientComponents/src/main.js``` for set up of components. 

Each component is mounted within an HTML element using the class `__vue-root` and an id representing the name of the component to mount.

```HTML
<div id="h_CommentBox" 
    data-post-id="10" 
    data-post-comment=".NET 5, Vue" 
    class="__vue-root">
    </div>

```

### Requires
* .NET 5
* Node 10+
* Parcel
* Vue 
* Docker (optional)


### Installing Parcel
Yarn:

```yarn global add parcel-bundler```

npm:

```npm install -g parcel-bundler```


### Installation:

```
cd VueCore
```

Yarn:

```
yarn install
```

npm:

NPM Only. Ignore if using Yarn. Add the following to the ```scripts``` section within 
```package.json```

```
  "scripts": {
    "preinstall": "npx npm-force-resolutions",
  }
```
*This is addresses a known bug with Parcel and Babel Preset-env.*
https://github.com/parcel-bundler/parcel/issues/5943

Then execute

```
npm install
```



### Compiles and hot-reloads for development
Yarn:

```
yarn start
```

npm:

```
npm start
```


### Compiles and minifies for production
Yarn:

```
yarn build
```

npm:

```
npm build
```

## Docker

```
cd VueCore
docker build -t aurlaw/vuecore:1.0 .
docker run -d -p 9090:80 aurlaw/vuecore:1.0

```

## Tagging

```
git tag v1.0
git push --tags
```

https://leemartin.dev/how-to-develop-a-countdown-clock-using-vue-and-luxon-for-rockstars-e3ecff9338ef

## Azure User Secrets

vue-core

```
dotnet user-secrets set "AzureStorage:ConnectionString" "STORAGE CONNECTION STRING"
dotnet user-secrets set "AzureStorage:Container" "STORAGE CONTAINER NAME"
dotnet user-secrets set "AzureVision:Endpoint" "VISION ENPOINT"
dotnet user-secrets set "AzureVision:SubscriptionKey" "VISION SUBSCRIPTION KEY"
```
