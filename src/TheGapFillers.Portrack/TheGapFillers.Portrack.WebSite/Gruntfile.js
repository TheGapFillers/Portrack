module.exports = function (grunt) {

    // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        // Concatenates all the js and css from the bower components folder into scripts.js and stylesheets.css
        bower_concat: {
            all: {
                dest: 'bower/scripts.js',
                cssDest: 'bower/stylesheets.css',
                exclude: [
                    'datatables-plugins'
                ]
            },
        },
        copy: {
            main: {
                files: [{
                    //for bootstrap fonts
                    expand: true,
                    dot: true,
                    cwd: 'bower_components/bootstrap/dist',
                    src: ['fonts/*.*']
                }, {

                    //for font-awesome
                    expand: true,
                    dot: true,
                    cwd: 'bower_components/font-awesome',
                    src: ['fonts/*.*']
                    
                }]
            },
        },
        // minify the scripts.js files
        uglify: {
            bower: {
                options: {
                    mangle: true,
                    compress: true
                },
                files: {
                    'bower/scripts.min.js': 'bower/scripts.js'
                }
            }
        },
        // minify the stylesheets.css files
        cssmin: {
            options: {
                shorthandCompacting: false,
                roundingPrecision: -1
            },
            target: {
                files: {
                    'bower/stylesheets.min.css': ['bower/stylesheets.css']
                }
            }
        }
    });

    // Load all the grunt tasks
    require('load-grunt-tasks')(grunt);

    // Default tasks.
    grunt.registerTask('default', ['bower_concat', 'copy', 'uglify', 'cssmin']);

};