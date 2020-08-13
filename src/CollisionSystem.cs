using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace Prototype_Golem
{
    public abstract class CollisionSystem
    {
        public bool[] CollisionMask {get; protected set;}

        public bool Clip {get; set; } = true; //entities like buttons that you can walk through would be set to false.
        public int TouchedSides {get; protected set;} //tells the parent entity what sides of other objects it has touched for response
        public Vector2 Speed {get; set;}
        public Vector2 OldPos {get; set;}
        public Vector2 Pos {get; set;}
        public List<Entity> CollidedEntities {get; protected set;}
        
        public void PrepareUpdate() {
            CollisionMask = new bool[LevelHandler.MapWidth*LevelHandler.MapHeight];
            UpdateCollisionMask();
        }
        public void CollisionUpdate(int[] collisionMap, List<Entity> entities) {
            CollidedEntities = new List<Entity>();
            TouchedSides=0;
            if (Clip) CollideTiles();
            OldPos = Pos;
            CollideDetectEntities(entities);
            if (Clip) CollideResolveEntities();
        }

        protected abstract void UpdateCollisionMask();
        protected abstract void CollideTiles();
        protected abstract void CollideDetectEntities(List<Entity> entities);
        protected abstract void CollideResolveEntities();
        
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