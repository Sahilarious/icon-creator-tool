# icon-creator-tool
WIP : Editor tool used to take pictures (e.g. to create 2D icons) of 3D object prefabs.

All pertinent files for the Icon Creator Tool reside in "Assets/IconCreator"

Can be accessed in the Unity editor via "Window/Art Tools/IconCreatorWindow"

TODO:

* Resolve some aliasing issues. Increasing anti-aliasing in Settings does not seem to work. If Unity does not have anything built in, I'll see if I can play around with the code in the UpdateViewport function of IconCreatorWindow.cs. To see if I can manually soften the edges where pixel stepping occurs.
* Add more lighing options (e.g. intensity, position, rotation, etc.)
* Add the ability to save and load up chosen Icon Creator Window settings, so they can be used again at a later date. Perhaps save them in json files?
* Add the ability to choose image size and image type. For now the default is 512^2 and .png respectively.
* Add ability to name created images manually. Perhaps keep a toggle field that if checked/true will still create a name automatically.  
* Maybe add functionality to save animations (.gifs). 
