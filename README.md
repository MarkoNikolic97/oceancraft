# oceancraft
Project example for showcase


Hello,

This is an old project that I made in spare time that I found between classes and studying. It was made for fun without taking into account that it would be seen by anyone else, hence it is very unstructured as I was mostly experimenting and playing around with Unity. It is very unoptimized and I apologize for the unreadable and probably indecipherable code. If you are interested, of course, I could add comments and delete the unnecessary code such as Debug.Logs.
With all Debugging still in code the game is buggy to say the least, and as I said it is unoptimized as well. The User Interface is barebones and implemented to make testing possible. As you will see, there are a lot of classes that are there but still waiting for implementation.

I have made a short video showcasing the most important and implemented functions of the project in an attempt to save your valuable time. Of course if you do decide to explore the project in Unity I hope the video will also help you to make sense of this raw and unfinished project.

https://youtu.be/kRol1d4MjmY

Again, I apologize for this incompleteness, I set out to do something which I had neither the time nor resources for. As I said, all of my projects were for fun and unfortunately most of them are lost. This is one of the rare ones I saved, and probably most complicated.

Here is the basic structure of my code:
World is divided into Segments, segments into Chunks and finally chunks into Voxels

World:  

    Holds world data such as Segments, trees(not implemented) and other objects that belong to it.
    It handles chunk generation around the player, but Chunks implement their own Generation.
    It also implements BuidShip() method and ConstructionSiteRoutine()
    Holds the NoiseGeneration script with its settings.
    World should have been divided into its Model and Controller as the Player was.

Segment:

    Holds voxel data because of the limit on the size of the Matrix Array in Unity.
    It generates and updates chunks using NoiseGeneration script
    
Chunk: 

    Chunks implement basic operations on small number of voxels [16,16,16]
    This is needed for fast placement and destruction of blocks.
    
Voxel:

    Holds basic data of the single Voxel such as its position, its health and what block it actually is.
    
Player:

    Player Model
    
PlayerController:

    Implemets all player functionalities.
    
Ship:

    Stores ship data in voxels
    Implements ship physics
    
    
The idea actually was to make the parts of the whole world a static size and that is why the World has bounderies. As you cross them loading screen should appear for the next part of the planet. This makes it possible for example, to have World such as Deep_Ocean which would be immensely deep but smaller in horizontal dimensions.
This would also make possible for the "looping" of Worlds as to create the illusion of a spherical planet.    

Thank you very much for your time and this amazing opportunity.
I am very exited to hear from you,

Marko Nikolic

