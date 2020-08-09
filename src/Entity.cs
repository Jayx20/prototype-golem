using Microsoft.Xna.Framework;

namespace Prototype_Golem
{
    public abstract class Entity
    {
        public bool Render {get; protected set;}
        public bool Collide {get; protected set;}
        
        public CollisionSystem Collision {get; protected set;}

        //id for what spritemap to use later on
        public Rectangle TextRect {get; protected set;}

        public Vector2 Pos {get; set;}
        public Vector2 Speed {get; set;}

        public abstract void Update(); //very important
        public void Move() {
            Pos += Speed;
        }
    }
}