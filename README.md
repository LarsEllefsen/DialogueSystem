# Unity Dialogue System

Dialogue System is a (yet-to-be-named!) flexible and customizable graph-based Dialogue System for Unity. The dialogue graph and graph editor is built upon the fantastic [Xnode](https://github.com/Siccity/xNode) package.

## Installation


### 1. Add Dialogue System Package
The recommended (and easiest) way to install is through the Unity Package Manager (UPM). 
Simply open the Package Manager and click "Add package from GIT Url" in the upper left corner, and add the URL:

```https://github.com/LarsEllefsen/DialogueSystem.git```

### 2. Add Xnode
The Unity Package Manager is as of this writing not able to handle dependencies outside of the unity registry, so for the time being you will have to manually install Xnode.
Through the Package Manager, click "Add package from GIT Url" and add:

```https://github.com/Siccity/xNode.git```

## Quick start

### 1. Creating a dialogue graph
In order to get started you need to create a Dialogue Graph asset. Simply right click anywhere in your project explorer and click "Dialogue Graph" from the Create menu. Clicking the asset will open the Dialogue Node Editor. Start by adding a Branch Node (Right click -> Dialogue System -> Branch Node), which will serve as en entry point in our graph. 

From here you can add nodes either by dragging the output connection out and releasing it somewhere, or by right clicking and adding manually. Try connecting the Branch Node to a Text Node with some text.

### 2. Setting up the Dialogue Manager Component
Once you have a graph ready, simply add the ***DialogueManager*** component to any gameObject. 
