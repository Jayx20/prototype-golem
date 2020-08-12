using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace Prototype_Golem
{
    public abstract class CollisionSystem
    {
        public bool[] CollisionMask {get; protected set;}

        public int TouchedSides {get; protected set;} //tells the parent entity what sides of other objects it has touched for response
        public Vector2 Speed {get; set;}
        public Vector2 OldPos {get; set;}
        public Vector2 Pos {get; set;}
        public Queue<Entity> collidedEntities;
        public void CollisionUpdate(int[] collisionMap) {
            TouchedSides=0;
            CollisionMask = new bool[LevelHandler.MapWidth*LevelHandler.MapHeight];
            UpdateCollisionMask();
            CollideTiles();
            CollideEntities();
        }

        protected abstract void UpdateCollisionMask();
        protected abstract void CollideTiles();
        protected abstract void CollideEntities();
        //stuff for reacting
    
        public enum CollisionDirections {
            NONE = 0,
            TOP = 1,
            LEFT = 2,
            BOTTOM = 4,
            RIGHT = 8,
            ALL = 15,
        } 
    }
}