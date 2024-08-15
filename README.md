Unity Cryptomatte
===============

Unity package that offers the functionality to render "Cryptomatte" ID Mattes
from Unity.  
These ID Mattes can subsequently be used in any compositing software
like Nuke to isolate mattes for objects in compositing.  
The package enables the creation ID mattes on object, material and asset level.
Additionally to the Unity package this repositiory contains tools for Nuke that
help with using the generated ID Mattes in Nuke by relinking the metadata from 
an external .json file, since it is currently not possible to write metadata 
directly into the .exr headers while rendering in Unity.



---

### Install

* Clone this repository from Github
```sh
git clone https://github.com/SchmuckerDaniel/UnityCryptomatte.git
```


* Clone Scriptable Render Pipeline Fork from Github
This was the fork used for setting up and testing. 
It is very outdated by now, but no testing was done on a more current version.
```sh
git clone https://github.com/SchmuckerDaniel/ScriptableRenderPipeline.git
```


* Clone Unity Python Fork from Github  
This is a plugin for Unity3D that provides support for running Python code in Unity.  
This is used to do the hashing of object/material/asset names in Unity.
```sh
git clone https://github.com/SchmuckerDaniel/unity-python.git  
```  

  
  

---

### Requirements

* Unity Editor 2019.3 recomended
* Unity High Definition Render Pipeline
* 'Offline Render' Plugin  
currently also requires Plugin 'Offline Render' from the Asset store to
correctly render Cryptomattes since Unity's builtin Recorder doesn't render
the values correctly


---

### Setup

* create new Unity Project or use exisiting Project using Unity's High
Definition Render Pipeline
* install packages via Unity Package Manager (install from disk):
	* From the cloned Scriptable Render Pipeline Fork: install Unity HD Render Pipeline
	* Unity Python
	* Cryptomatte

* use HDRP Wizard in Unity to correclty configure the HDRP:  
https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@6.7/manual/Render-Pipeline-Wizard.html
* configure HDRP for cryptomatte use:  
Go to Project Settings - Quality - HDRP - HDRenderPipelineAsset - (press the default button if no settings appear)  
change Color Buffer Format (Rendering Tab) to R32G32B32A32 to enable full floating point precision when rendering
