using Microsoft.Xna.Framework;
using System;

namespace Prototype_Golem
{
    public abstract class Entity
    {
        public Drawable Drawable {get; protected set;}
        public bool Delete {get; protected set;} //for when an entity wants to delete itself
        public Point Pos {get; set;}

        public abstract void Update(); //very important

        //overwrite for behaviors when colliding with certain entites
        public virtual void CollideWith(Entity entity) {
            //do nothing when it collides with anything
        }
    }
}