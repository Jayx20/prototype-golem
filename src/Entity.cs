using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Prototype_Golem
{
    public abstract class Entity
    {
        public bool Render {get; protected set;}
        public bool Collide {get; protected set;}
        public bool Delete {get; protected set;} //for when an entity wants to delete itself
        
        public SpriteEffects Effects {get; protected set;} = SpriteEffects.None;
        public CollisionSystem Collision {get; protected set;}

        public TextureID TextID {get; protected set;}
        public Rectangle TextRect {get; protected set;}

        public Vector2 Pos {get; set;}
        public Vector2 Speed {get; set;}

        public abstract void Update(); //very important
        public void Move() {
            Pos += Speed;
        }

        //overwrite for behaviors when colliding with certain entites
        public virtual void CollideWith(Entity entity) {
            //do nothing when it collides with anything
        }
    }
}