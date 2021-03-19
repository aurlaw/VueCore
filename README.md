# VueCore

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


### Installing Parcel
Yarn:

```yarn global add parcel-bundler```

npm:

```npm install -g parcel-bundler```


### Running with Watch:

```
cd VueCore
yarn install
```

### Compiles and hot-reloads for development
```
parcel watch ./ClientComponents/bundle.js --out-dir ./wwwroot/dist
dotnet watch run
```

### Compiles and minifies for production
```
parcel build ./ClientComponents/bundle.js --out-dir ./wwwroot/dist
dotnet publish -c Release
```

