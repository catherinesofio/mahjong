# Mahjong

## Overview
> *This project was done using **Unity 2020.3.25f1**.*

Open the project in Unity, under the *Scenes* folder select the ***Menu*** scene, and then press play!

The game is lost when there are still tiles on the board, but there are no matches left. If all tiles have been matched or only 1 tile remains, the player will win.

<br/>

## Gameplay
### Level Layout
> ***GameLogic**.GetGameLayout()*

The **Layout** of each level is loaded from a *.txt* file composed by **0** (empty spaces) and **X** (tiles). Due to the chosen pathfinding method, empty spaces are needed to reach a certain Node. Therefore, each layout is added an extra layer of zeros that will surround the whole grid.

For example, let's say we got the following level layout:
> XX
> <br/>
> XX

After adding the extra nodes, the final layout looks like this:
> 0000
> <br/>
> 0XX0
> <br/>
> 0XX0
> <br/>
> 0000

As to avoid nesting *fors* and reduce the overall complexity in the following steps, the grid is flattened into a **char[]**. Since we keep reference of the count of nodes in both axes (*_countX* and *_countY*), we can easily calculate each of the node's coordinates whenever it's required using the following equations:

`int x = id % _countX;`
<br/>
`int y = id / _countX;`
> *where **id** is the index in the char array.*

Taken the previous example grid, the following graphs would illustrate the grid by:

*IDs:*

> &nbsp;&nbsp;0 &nbsp;&nbsp;1 &nbsp;&nbsp;2 &nbsp;&nbsp;3
> <br/>
> &nbsp;&nbsp;4 &nbsp;&nbsp;5 &nbsp;&nbsp;6 &nbsp;&nbsp;7
> <br/>
> &nbsp;&nbsp;8 &nbsp;&nbsp;9 10 11
> <br/>
> 12 13 14 15

*Coordinates:*

> (0:0) (1:0) (2:0) (3:0)
> <br/>
> (0:1) (1:1) (2:1) (3:1)
> <br/>
> (0:2) (1:2) (2:2) (3:2)
> <br/>
> (0:3) (1:3) (2:3) (3:3)

<br/>

### Graph Creation
> ***GameLogic**.CreateGraph(char[] layout)*
> <br/>
> ***GameLogic**.GetOrCreateNode(int id, char[] layout, ref int unmatchedTile)*
> <br/>
> ***GameLogic**.SetNeighbours(int id, char[] layout, ref int unmatchedTile)*

A single for-loop will go from *0* to the total node count (*_countX * _countY*), in which the function **GetOrCreateNode** will be called and store each of the nodes in a **Node[]** called **_graph**. This function receives the *Node's Id*, the previously calculated *Layout* and the *Last Unmatched Tile* that represents the type of the last Tile (not empty Node) with no match created. The later is useful to create a solvable Grid.

Once the Node is created, in the **SetNeighbours** function, it will save a reference to its neighbours (either by retrieving them from **_graph** if these already exist, or creating them in the process). A node *N* is considered a neighbour of *A* if it's adjacent to *A*.

> 0XNX0
> <br/>
> 0NAN0
> <br/>
> 0XNX0
> <br/>
> *where **N** is a neighbour of **A**.*

<br/>

### Turns Calculation
> ***GameLogic**.GetTurns(Node from, Node to)*
> <br/>
> ***GameLogic**.GetMatch()*

The **Pathfinding** (or rather, the computation of the *shortest amount of turns* to reach a node) is calculated using the ***Dijkstra*** algorithm. The weight of each empty node will either be 0 or 1; it all comes down if reaching that node requires making a turn or not.

A *priority queue* is implemented. Aside from storing the *Node* and its *Weight*, this queue also saves the *Current Direction*. The later is useful to calculate changes in direction/nodes' weights.

This method is reused in the method **GetMatch**, which is applied to give hints to the player and check if there are no matches left.

<br/>

### Related Classes
- **[GameLogic](https://github.com/catherinesofio/mahjong/blob/main/Assets/Scripts/Game/GameLogic.cs)**
- **[Node](https://github.com/catherinesofio/mahjong/blob/main/Assets/Scripts/Game/Node.cs)**
- **[PriorityQueue](https://github.com/catherinesofio/mahjong/blob/main/Assets/Scripts/Game/PriorityQueue.cs)**

<br/>

## Managers
The following managers are all *Singletons* and persist during the whole execution of the program, as these are pretty much needed in every screen. Alongside the *Main Camera*, these are contained in the **Prefab_GeneralManagers**.

### [Event Manager](https://github.com/catherinesofio/mahjong/blob/main/Assets/Scripts/Managers/Events/EventManager.cs)
> *This class implements an enum **[EventId](https://github.com/catherinesofio/mahjong/blob/main/Assets/Scripts/Managers/Events/EventId.cs)** to identify the events.*

The **Event Manager** allows decoupled communications between classes whenever a certain event is thrown.

<br/>

### [Screen Manager](https://github.com/catherinesofio/mahjong/blob/main/Assets/Scripts/Managers/Screen/ScreenManager.cs)
> *This class implements an enum **[ScreenId](https://github.com/catherinesofio/mahjong/blob/main/Assets/Scripts/Managers/Screen/ScreenId.cs)** to identify the screens.*

The **Screen Manager** ~~otherwise known as Scene Manager, but that name was already taken by Unity lol~~ is the responsible for switching and loading the different screens. It loads scenes in an *asynchronous* fashion.

<br/>

### [Data Manager](https://github.com/catherinesofio/mahjong/blob/main/Assets/Scripts/Managers/DataManager.cs)

The **Data Manager** contains all the information that will be referenced in the program. For example, the saving and retrieving of *Player Data* (completed levels, highscores) is handled here.

Apart from the persistent data, it also contains references to easily customizable *Scriptable Objects* related to the configuration of the different levels (layout file, tile sprites, etc).

<br/>

### [Audio Manager](https://github.com/catherinesofio/mahjong/blob/main/Assets/Scripts/Managers/Audio/AudioManager.cs)
> *This class implements the enums **[SoundId](https://github.com/catherinesofio/mahjong/blob/main/Assets/Scripts/Managers/Audio/SoundId.cs)** and **[ScreenId](https://github.com/catherinesofio/mahjong/blob/main/Assets/Scripts/Managers/Screen/ScreenId.cs)** (because music varies by scene) to identify the audio.*

The **Audio Manager** contains 2 different *Audio Sources* for each channel (Sound and Music). Aside from playing audio, it's also in charge of dealing with the *Player Prefs* related to the toggling of any of those channels.

<br/>

## Player Data

It refers to the **Persistent Data**. As previously mentioned, the *[DataManager](https://github.com/catherinesofio/mahjong#data-manager)* is in charge of processing this data.

The **LevelDataModel** defines a serializable object that contains the *Level Id*, *Level Highscore* and *"hasJustGainedStar"* (which is just a boolean that determines whether to show or not an animation when the player goes to the menu right after having completed a level for the very first time).

The **PlayerDataModel** contains a list of *LevelDataModels*. Whenever a level is completed, a new instance of LevelDataModel will be created and it will be added to this list. Every time the highscore is surpassed, its related entry will be updated.

<br/>

### Related Classes
 - **[PlayerDataModel](https://github.com/catherinesofio/mahjong/blob/main/Assets/Scripts/Models/PlayerData/PlayerDataModel.cs)**
 - **[LevelDataModel](https://github.com/catherinesofio/mahjong/blob/main/Assets/Scripts/Models/PlayerData/LevelDataModel.cs)**

<br/>

## Scriptable Objects
> *The instances of these objects can be found in the **Assets/Data** folder.*

For an easier customization of visuals, layouts and audio, scriptable objects were created to store such information.

The **Audio Data** scriptable object contains 2 arrays, each representing the mapping of a certain *Audio Clip* to either a *SoundId* or a *ScreenId* (music). This object is used by the *[AudioManager](https://github.com/catherinesofio/mahjong#audio-manager)*.

While the **Level Model** serializable class contains the *Layout File Name* and a reference to a scriptable object of type *TilesData* (which defines the possible tile sprites of a Level), the **Levels Data** contains an array of each LevelModel and the *Path of the Folder* which contains the layout files. This data is referenced in the *[DataManager](https://github.com/catherinesofio/mahjong#data-manager)*, and is later accessed by *[GameLogic](https://github.com/catherinesofio/mahjong#level-layout)* when the level layout is loaded.

<br/>

### Related Classes
 - **[AudioData](https://github.com/catherinesofio/mahjong/blob/main/Assets/Scripts/Data/AudioData.cs)**
 - **[LevelModel](https://github.com/catherinesofio/mahjong/blob/main/Assets/Scripts/Models/LevelModel.cs)**
 - **[TilesData](https://github.com/catherinesofio/mahjong/blob/main/Assets/Scripts/Data/TilesData.cs)**
 - **[LevelsData](https://github.com/catherinesofio/mahjong/blob/main/Assets/Scripts/Data/LevelsData.cs)**

<hr>

Made by [Catalina Sofio Avogadro](https://www.linkedin.com/in/catalina-sofio-avogadro/).
