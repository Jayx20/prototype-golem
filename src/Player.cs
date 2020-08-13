using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Prototype_Golem
{
    public class Player:Entity
    {
        bool inMech;
        bool canJump = false;
        bool touchingGround = false;
        PlrInput input;

        AnimationState animationState;

        public Player(Vector2 pos) {
            //spawns player outside of mech
            Pos = pos;
            Collide = true;
            Render = true;
            inMech = false;
            input = new PlrInput();
            TextID = TextureID.PLAYER;
            TextRect = new Rectangle(0, 0, 32, 64);
            animationState = AnimationState.STILL_RIGHT;
            Collision = new AABB(new Point(8, 16), new Point(25, 61)); //the player is 1x2
        }
        public override void Update() {
            input.Update(Keyboard.GetState(), Mouse.GetState(), GamePad.GetState(PlayerIndex.One));

            //experimental gravity - not sure if i will have an acceleration variable
            Vector2 gravity = new Vector2(0, Constants.GRAVITY_STRENGTH);

            //jumping logic
            bool wasTouchingGround = touchingGround;
            if ((Collision.TouchedSides&(int)CollisionSystem.CollisionDirections.TOP)!=0) touchingGround = true; else touchingGround = false;
            
            if(!wasTouchingGround && touchingGround) 
                Game1.SoundEffects[(int)SFX.THUMP].CreateInstance().Play();

            if (!canJump && touchingGround) { //add nice timing and stuff later for responsiveness
                canJump = true;
            }

            if(!touchingGround) canJump = false;

            Speed = new Vector2(0, Speed.Y); //reset X speed might change later
            if(input.Left.Held)  Speed += new Vector2(-Constants.MOVEMENT_SPEED, 0);
            if(input.Right.Held) Speed += new Vector2(Constants.MOVEMENT_SPEED, 0);
            if((input.Up.Held || input.Interact1.Held) && Speed.Y < 0) gravity*=Constants.GRAVITY_MULTIPLIER; //
            if((input.Up.Pressed || input.Interact1.Pressed) && canJump) {
                canJump = false;
                Speed = new Vector2(Speed.X, -Constants.JUMP_FORCE);
                Game1.SoundEffects[(int)SFX.JUMP].CreateInstance().Play();
            } //
            //if(input.Down.Held) {Speed += new Vector2(0, .2f);}

            Speed += gravity;
            //Console.WriteLine($"pos = {Pos}");

            AnimationUpdate();
        
        }

        void AnimationUpdate() {
            AnimationState oldAnimationState = animationState;

            if(input.Left.Held)
                Console.Write("hi");
            //these are just naturally going to be complicated with a lot of logic not sure how else to deal with it
            if(input.Left.Held && Speed.X < 0 && touchingGround) animationState = AnimationState.WALKING_LEFT; //walking left
            else if(input.Right.Held && Speed.X > 0 && touchingGround) animationState = AnimationState.WALKING_RIGHT; //walking right
            else { //player is still
                if (oldAnimationState == AnimationState.WALKING_LEFT) animationState = AnimationState.STILL_LEFT;
                else if (oldAnimationState == AnimationState.WALKING_RIGHT) animationState = AnimationState.STILL_RIGHT;
                TextRect = new Rectangle(0, 0, 32 ,64);
            }

            //TODO: midair animations and movement
            
            //if facing left mirror the image
            if (animationState == AnimationState.STILL_LEFT || animationState == AnimationState.WALKING_LEFT) {
                Effects = SpriteEffects.FlipHorizontally;
            } else Effects = SpriteEffects.None;
            
            //if walking, change the image each frame
            if (animationState == AnimationState.WALKING_LEFT || animationState == AnimationState.WALKING_RIGHT) {
                WalkingLoop();
            }
        }

        void WalkingLoop() {
            if (TextRect.X < 5*32) //6 is the number of frames in the walking animation
                TextRect = new Rectangle(TextRect.X+32, 0, 32, 64);
            else
                TextRect = new Rectangle(0, 0, 32, 64);
        }

        enum AnimationState {
            STILL_LEFT,
            STILL_RIGHT,
            WALKING_LEFT,
            WALKING_RIGHT,
        }
    }
}