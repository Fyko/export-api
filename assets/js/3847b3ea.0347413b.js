"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[581],{3905:(e,t,r)=>{r.d(t,{Zo:()=>s,kt:()=>m});var n=r(7294);function o(e,t,r){return t in e?Object.defineProperty(e,t,{value:r,enumerable:!0,configurable:!0,writable:!0}):e[t]=r,e}function i(e,t){var r=Object.keys(e);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(e);t&&(n=n.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),r.push.apply(r,n)}return r}function p(e){for(var t=1;t<arguments.length;t++){var r=null!=arguments[t]?arguments[t]:{};t%2?i(Object(r),!0).forEach((function(t){o(e,t,r[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(r)):i(Object(r)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(r,t))}))}return e}function a(e,t){if(null==e)return{};var r,n,o=function(e,t){if(null==e)return{};var r,n,o={},i=Object.keys(e);for(n=0;n<i.length;n++)r=i[n],t.indexOf(r)>=0||(o[r]=e[r]);return o}(e,t);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);for(n=0;n<i.length;n++)r=i[n],t.indexOf(r)>=0||Object.prototype.propertyIsEnumerable.call(e,r)&&(o[r]=e[r])}return o}var c=n.createContext({}),u=function(e){var t=n.useContext(c),r=t;return e&&(r="function"==typeof e?e(t):p(p({},t),e)),r},s=function(e){var t=u(e.components);return n.createElement(c.Provider,{value:t},e.children)},l={inlineCode:"code",wrapper:function(e){var t=e.children;return n.createElement(n.Fragment,{},t)}},d=n.forwardRef((function(e,t){var r=e.components,o=e.mdxType,i=e.originalType,c=e.parentName,s=a(e,["components","mdxType","originalType","parentName"]),d=u(r),m=o,f=d["".concat(c,".").concat(m)]||d[m]||l[m]||i;return r?n.createElement(f,p(p({ref:t},s),{},{components:r})):n.createElement(f,p({ref:t},s))}));function m(e,t){var r=arguments,o=t&&t.mdxType;if("string"==typeof e||o){var i=r.length,p=new Array(i);p[0]=d;var a={};for(var c in t)hasOwnProperty.call(t,c)&&(a[c]=t[c]);a.originalType=e,a.mdxType="string"==typeof e?e:o,p[1]=a;for(var u=2;u<i;u++)p[u]=r[u];return n.createElement.apply(null,p)}return n.createElement.apply(null,r)}d.displayName="MDXCreateElement"},1959:(e,t,r)=>{r.r(t),r.d(t,{assets:()=>c,contentTitle:()=>p,default:()=>l,frontMatter:()=>i,metadata:()=>a,toc:()=>u});var n=r(7462),o=(r(7294),r(3905));const i={id:"setup",title:"Setup",sidebar_position:1},p="Setup",a={unversionedId:"setup",id:"setup",title:"Setup",description:"This is the reccomended way of running Export API.",source:"@site/docs/setup.md",sourceDirName:".",slug:"/setup",permalink:"/export-api/docs/setup",draft:!1,editUrl:"https://github.com/Fyko/export-api/edit/main/website/docs/setup.md",tags:[],version:"current",sidebarPosition:1,frontMatter:{id:"setup",title:"Setup",sidebar_position:1},sidebar:"tutorialSidebar",previous:{title:"Introduction",permalink:"/export-api/docs/intro"},next:{title:"gRPC",permalink:"/export-api/docs/api-versions/gRPC"}},c={},u=[],s={toc:u};function l(e){let{components:t,...r}=e;return(0,o.kt)("wrapper",(0,n.Z)({},s,r,{components:t,mdxType:"MDXLayout"}),(0,o.kt)("h1",{id:"setup"},"Setup"),(0,o.kt)("h1",{id:"docker-compose"},"Docker Compose"),(0,o.kt)("admonition",{type:"tip"},(0,o.kt)("p",{parentName:"admonition"},"This is the reccomended way of running Export API.")),(0,o.kt)("p",null,"To start using ",(0,o.kt)("inlineCode",{parentName:"p"},"fyko/export-api"),", add the service to your ",(0,o.kt)("inlineCode",{parentName:"p"},"docker-compose.yml")," file!"),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-yml"},'services:\n  exportapi:\n    image: ghcr.io/fyko/export-api\n    expose:\n      - "80"\n')),(0,o.kt)("p",null,"You can then interact with the API with ",(0,o.kt)("inlineCode",{parentName:"p"},"http://exportapi/..."),"."),(0,o.kt)("h1",{id:"docker-run"},(0,o.kt)("inlineCode",{parentName:"h1"},"docker run")),(0,o.kt)("p",null,(0,o.kt)("inlineCode",{parentName:"p"},"docker run -p yourport:80 --rm -it ghcr.io/fyko/export-api")),(0,o.kt)("p",null,"You can then interact with the API with ",(0,o.kt)("inlineCode",{parentName:"p"},"http://127.0.0.1:yourport/..."),"."))}l.isMDXComponent=!0}}]);