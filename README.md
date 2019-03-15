# WIP

This is still a work in progress... As long as this notice is up, please don't try this, as it likely won't work anyway (if it works at all).

# TeddyBear

Tile based map editor with some nice features

# History

Teddybear was a project set up by Jeroen P. Broks in order to provide a tile-based map editor for "The Secrets of Dyrt" project. It has not seen that much use since later projects would most of all rely upon Kthura, which is as an object based map editor much more sophisticated, and has a larger scale of possibilities.
Teddybear has however been used again in the Cynthia Johnson project, and also in BallPlay Cupid, since for puzzle games, a tile-based editor can be a much better approach to things.

TeddyBear was originally written in BlitzMax, and its name is a pun to the tool "TEd", used by id Software to design the maps for their Commander Keen and Wolfenstein series (and it has been used for many other games, such as Cosmo's Cosmic Adventure, Rise of the Triad, Blake Stone and many more). TeddyBear however is a lot more flexible and more sophisticated than TEd, due to it being able to rely on an OOP based programming background (not as much as Kthura, but still), and it has a bigger customization set up.

As BlitzMax is not used that much any more, I wanted to redo both TeddyBear and Kthura, and I also wanted them to be more user-friendly than the original tools, as both tools suffer a bit here. This is also why TeddyBear will now come with a launcher and a quick project creation wizard, so you can at least set up some very basic stuff. You will need to do a few more studies to get the full possibilities out of TeddyBear, but hey Kthura is gonna be harder :P

Also note that TeddyBear is by itself pretty much relying on my own classes and particularly the JCR6 class. Teddybear's native file format is actually just a JCR6 file, and due to JCR's merging system TeddyBear is easy to blend in that way. Exporters for Lua, JSON, "pure" JavaScript and Python are planned.

# This repository

I will in this repository have a few sub-folders:

subfolder | contains
---|---
TeddyClass  | This will contain some class files written in C# to load and show TeddyBear maps. Even manipulation will be possible (psst. the editor uses these classes itself, as well) :P
TeddyEdit   | This will be the editor. The first model is set up with MonoGame. If that remains that way will remain to be seen, but for now it should do.
TeddyLaunch | The launcher

# Compiling building

Make sure that prior to even beginning to compile that you have a folder for my projects in general. I'll just assume it to be T:\Tricky for now, okay?

~~~batch
T:\Tricky>git clone tricky1975/trickyunits_csharp TrickyUnits
T:\Tricky>git clone jcr6/jcr6_csharp JCR6
T:\Tricky>git clone TrickyGameTools/TeddyBear
~~~

That should get you everything you need, then we can do the following to compile
~~~batch
T:\Tricky>cd TeddyBear
T:\Tricky\TeddyBear>TeddyMake
~~~

Please note that the building script "TeddyMake" assumes the "msbuild.exe" tool to be found at "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe". If you have another location, set "TRICKYMSBUILD" to your location prior to running the TeddyBuild script.

### Why not use a "makefile"?

Since the prime target is now Windows, I'm not sure if MAKE is the best solution anyway, and beside msbuild.exe already checks if source files have been changed and need rebuilding, so stuff that doesn't need to be compiled, won't be compiled anyway. Also in Windows having MingGW or other compilers installed does not really guarantee MAKE can be found, like on Unix system. All in all this was the better ride.


# License

The TeddyBear classes to attach to your own programs will be licensed under a zlib license. The files used for the launcher, the wizard and the editor specifically will be licensed under the GPL 3.0

