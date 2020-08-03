using Microsoft.Xna.Framework;

namespace Prototype_Golem
{
    public abstract class Entity
    {
        public static int mapWidth, mapHeight; //map width and height in tiles (not pixels) - for checking what tiles an entity is inside
        public bool Render {get; protected set;}
        public bool Collide {get; protected set;}
        //id for what spritemap to use later on
        public Rectangle TextRect {get; protected set;}

        public Vector2 Pos {get; protected set;} //idk if we will need a public setter later

        public abstract void Update(); //very important
    }
}