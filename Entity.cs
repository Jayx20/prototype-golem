using Microsoft.Xna.Framework;

namespace Prototype_Golem
{
    public abstract class Entity
    {
        public Vector2 Pos {get; protected set;} //idk if we will need a public setter later
        public bool Clip {get; protected set;} //same here

        public abstract void Update(); //very important
    }
}