# edgejs-opengl
A quick example of running an OpenGL-based dll on a Nodejs server using edge-js

Right now it doesn't work, because I can't figure out how to run GLFW in the main thread. See https://github.com/agracio/edge-js/issues/214


## Run/install

Make sure to install node dependencies:

    npm i

Run using 

    node edge-api-loader.js

This should give you an error ` GLFW can only be called from the main thread!`


## Notes

Couldn't figure out how to set up dependencies on `edge-js`, so that's why there is a copy of all the OpenTK libraries on the root of the project, and why all dlls are versioned in this repo... 😭😭😭