# To Run the project

#### Install grunt comand line and typesctipt :

>open a command line with administrator rigths and type:

>       npm install -g grunt-cli
>       npm install -g tsd

#### Install nodeand bower dependencies:

>open a command line with the path in TheGapFillers.Portrack.WebSite and type:

>       npm install -- downloads the nodeJs dependencies in package.json to a folder called node_modules

>       bower install -- downloads bower dependencies in bower.json to a folder called bower_components

>       grunt -- runs the default grunt task that will bundle all js and css from the bower_components folder and minifies them, creates a folder called bower and places the files there. It also copies the fontawsome and bootstrap fonts to a fonts folder

#### Install typescript defenety typings:

>open a command line with the path in TheGapFillers.Portrack.WebSite and type:

>       tsd reinstall -- downloads typescript defenitions in tsd.json to a folder called typings

# References
>   [template](http://startbootstrap.com/template-overviews/sb-admin-2/)
>   [node](https://nodejs.org/)
>   [grunt](http://gruntjs.com/)
>   [grunt-cli](http://gruntjs.com/getting-started)
>   [bower](http://bower.io/)
>   [tsd](https://github.com/DefinitelyTyped/tsd/tree/master)