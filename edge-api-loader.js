
// https://github.com/agracio/edge-js?tab=readme-ov-file#coreclr
// Edge on Windows will default to .NET 4.5 and mono on MacOS/Linux. 
// If we want to use CORE, we need to set an env var in the target system
// or force an instance one like so:
process.env.EDGE_USE_CORECLR = 1


// This is not working, giving me a 
// `CoreClrEmbedding::Initialize - Failed to initialize CoreCLR, HRESULT: 0x80070057` error
// process.env.EDGE_APP_ROOT = './opengl-offscreen-renderer/bin/Debug/net8.0'

// Decided to copy all the dependencies from the binary folder into the root of the project
// for the time being




const edge = require('edge-js');

const dll_path = './opengl-offscreen-renderer/bin/Debug/net8.0/opengl-offscreen-renderer.dll'


// This will run async
const RenderTriangle = edge.func({
  assemblyFile: dll_path,
  typeName: 'OpenGLOffScreenRendering.Core',
  methodName: 'RenderTriangle'
});

// This will run sync
const RenderTriangleSync = edge.func({
  assemblyFile: dll_path,
  typeName: 'OpenGLOffScreenRendering.Core',
  methodName: 'RenderTriangleSync',
  sync: true
});

const triangleParams = {
  screenW: 800,
  screenH: 600,
  clr: {
    r: 255,
    g: 0,
    b: 255,
  },
  bgClr: {
    r: 15,
    g: 15,
    b: 15,
  }
}

const pixels = RenderTriangleSync(triangleParams, true);
console.log(pixels);





// const ThreadTest = edge.func({
//   assemblyFile: dll_path,
//   typeName: 'OpenGLOffScreenRendering.Core',
//   methodName: 'ThreadTest'
// });
// function handler (err, res) {
//   if (err) throw err;
//   console.log(`${typeof res}: `, res);
// }
// let res = ThreadTest({}, handler);
// console.log(res);






console.log('end of main JS program');
