# Dialogue System
Dialogue System is a node based dialogue graph used to easily create dialogues within your Unity Projects. Project based on the dialogue system built by [Wafflus](https://github.com/Wafflus) : https://github.com/Wafflus/unity-dialogue-system/tree/master

## Instructions
You can install this package in two ways.
### A) Asset Package
The suggested way to install this package is through the Unity's Asset Package system, via the .unitypackage file.

-> **The GIT URL you need to download to the asset file is the following** (download and install into your project): **https://github.com/insignia-interactive/Unity-Dialogue-System/releases**

For more info on how to install custom packages from th asset menu, please read here: https://docs.unity3d.com/Manual/AssetPackages.html

### B) Manual Install
You can also install this package manually, by copying the source files directly into your project's assets folder.

## Features

### Creation of Single or Multiple choice text or audio nodes.

Able to be created both through a contextual menu or a search window.

![Creation of the Nodes](https://imgur.com/g7Oo4pQ.gif)

### Grouping of Nodes

Nodes can be grouped to allow for better organization within your Graph.

They can also be used to filter dialogues within the Custom Inspector.

![Grouped Nodes](https://imgur.com/B21Znkk.gif)

### Feedback on Nodes or Groups with the same name in the same scope

Whenever a Node or a Group have elements of the same type with the same name in the same scope, their colors will be updated to allow the user to know they need to update their names.

The Save button will also be disabled until the names are changed.

This happens because the Graph Elements will end up becoming asset files in the Project folder.

![Same Name in the Same Scope Error Feedback](https://imgur.com/6rEqcR0.gif)
  
### Minimap

A minimap that allows the user to have an overview of where the Nodes are placed and to navigate to them by clicking in the respective shapes.

![Minimap](https://imgur.com/2rGVj3b.gif)
  
### Save & Load System

A Graph can be saved into a Scriptable Object that can be used to load it again later on.

The Graph elements will also be transformed into Scriptable Objects that can be used at Runtime.

Some, or most of those Scriptable Objects are used just for the Custom Inspector.

![Save & Load System](https://imgur.com/PUk2Jtq.gif)
  
### Clear & Reset Button

Allows for clearing the Graph (remove any element on it) and resetting the file name.

![Clearing and Resetting the Graph](https://imgur.com/ucHMBgQ.gif)
  
### Custom Inspector

Allows the ease of choosing a starting dialogue for a runtime dialogue system the user might want to develop.

The Inspector allows the filtering of dialogues by grouped and non-grouped dialogues, as well as by dialogues that can be considered "starting" dialogues.

![Custom Inspector](https://imgur.com/7gqUHpR.gif)

### Use
After the installation, you can use the package features.
#### Dialogue System Graph:
• Navigate to and click `Window\Insignia Interactive\Dialogue Graph`.
• Dock the window and create your node based dialogue.
• Save the nodes (turns into scriptable objects which can be used in game).

---

**If you find this project useful, please let me know!!!**\
I'd be super happy to see you taking part in it and sharing it around.

---

## Contributing
If you want to contribute:

1. Fork the project: https://github.com/insignia-interactive/Unity-Dialogue-System/forks
2. Create your own feature branch
3. Commit your changes to GitHub
4. Push to the branch 
5. Create a new Pull Request

More information about contributing here: https://github.com/firstcontributions/first-contributions
