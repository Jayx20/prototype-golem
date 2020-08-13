using Microsoft.Xna.Framework;

namespace Prototype_Golem.Entities
{
    public class Ruby : Entity
    {
        const int FRAMES_PER_ANIMATION_STEP = 10;
        const int NUMBER_OF_FRAMES = 6;
        
        public Ruby(Vector2 pos) {
            //spawns player outside of mech
            Pos = pos;
            Collide = true;
            Render = true;
            TextID = TextureID.ENTITIES_1;
            TextRect = new Rectangle(0, 0, 24, 24);
            Collision = new AABB(new Point(0, 0), new Point(24, 24)); //1x1
            Collision.Clip = false;
        }
        public override void Update()
        {
            AnimationUpdate();
        }

        int animationFrame = 0;
        void AnimationUpdate() {
            animationFrame++;
            if (animationFrame >= FRAMES_PER_ANIMATION_STEP) {
                animationFrame = 0;
                if (TextRect.X < NUMBER_OF_FRAMES*24) //6 is the number of frames in the walking animation
                    TextRect = new Rectangle(TextRect.X+24, 0, 24, 24);
                else
                    TextRect = new Rectangle(0, 0, 24, 24);
            }
        }

        public override void CollideWith(Entity entity) {
            if(entity is Player) {
                Delete = true;
                Game1.SoundEffects[(int)SFX.PICKUP].CreateInstance().Play();
            }
        }
    }
}