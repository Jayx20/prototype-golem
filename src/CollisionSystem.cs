using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace Prototype_Golem
{
    public abstract class CollisionSystem
    {
        public bool[] CollisionMask {get; protected set;}

        public Vector2 Speed {get; set;}
        public Vector2 OldPos {get; set;}
        public Vector2 Pos {get; set;}
        public Queue<Entity> collidedEntities;
        public void CollisionUpdate(int[] collisionMap) {
            CollisionMask = new bool[Game1.MapWidth*Game1.MapHeight];
            UpdateCollisionMask();
            CollideTiles();
            CollideEntities();
        }

        protected abstract void UpdateCollisionMask();
        protected abstract void CollideTiles();
        protected abstract void CollideEntities();
        //stuff for reacting
    
        protected enum CollisionDirections {
            NONE = 0,
            TOP = 1,
            LEFT = 2,
            BOTTOM = 4,
            RIGHT = 8,
            ALL = 15,
        } 
    }
}