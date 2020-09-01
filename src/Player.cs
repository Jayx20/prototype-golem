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

        //Timing variables
        int jumpPressTimer;
        int coyoteTimer; //time since touching ground

        AnimationState animationState;
        bool facingLeft;

        public Player(Point pos) {
            //spawns player outside of mech
            Pos = pos;
            Collide = true;
            Render = true;
            inMech = false;
            input = new PlrInput();
            TextID = TextureID.PLAYER;
            TextRect = new Rectangle(0, 0, 32, 64);
            animationState = AnimationState.STILL_RIGHT;
            Collision = new AABB(new Point(8, 16), new Point(24, 62)); //the player is 1x2
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

            if (touchingGround) { //add nice timing and stuff later for responsiveness
                canJump = true;
                coyoteTimer = 0;
            }
            else if(!touchingGround && coyoteTimer < Constants.COYOTE_TIME)
                coyoteTimer++;
            else if (!touchingGround && coyoteTimer >= Constants.COYOTE_TIME)
                canJump = false;
            
            //Movement left and right while on the ground (and midair for now)
            if(input.Left.Held && !input.Right.Held && Speed.X > -Constants.PLR_MAX_SPEED) { //go left
                float newXVelocity = Speed.X;
                if (Speed.X < Constants.PLR_ACCELERATION) //if we are already turning to the left
                    newXVelocity += Math.Max(-Constants.PLR_ACCELERATION, -(Constants.PLR_MAX_SPEED+Speed.X));
                else //if we are still going right and trying to turn, turn a little faster
                    newXVelocity += 2*-Constants.PLR_ACCELERATION;
                
                newXVelocity *= MathF.Pow(1f-Constants.ACCELERATION_DAMPING, 10f);
                Speed += new Vector2(newXVelocity, 0);
            }
            else if(input.Right.Held && !input.Left.Held) { //go right
                float newXVelocity = Speed.X;
                if (Speed.X > -Constants.PLR_ACCELERATION) //if we are already turning to the right
                    newXVelocity += Math.Min(Constants.PLR_ACCELERATION, Constants.PLR_MAX_SPEED-Speed.X);
                else //if we are still going left and trying to turn, turn a little faster
                    newXVelocity += 2*Constants.PLR_ACCELERATION;
                
                newXVelocity *= MathF.Pow(1f-Constants.ACCELERATION_DAMPING, 10f);
                Speed += new Vector2(newXVelocity, 0);
            }
            else if ((!input.Left.Held && !input.Right.Held) || (input.Left.Held && input.Right.Held)) {SlowBy(Constants.PLR_FRICTION);}

            //Holding up in the air
            if((!input.Up.Held && !input.Interact1.Held) && Speed.Y < 0) Speed*=new Vector2(1, Constants.JUMP_REDUCTION_MULTIPLIER);
            
            //Jumping
            if (jumpPressTimer > 0) jumpPressTimer--;

            if(input.Up.Pressed || input.Interact1.Pressed) {
                if (canJump) {
                    Jump();
                }
                else
                    jumpPressTimer = Constants.JUMP_REMEMBER_TIME;
            }
            else if (touchingGround && jumpPressTimer > 0) Jump();

            Speed += gravity;
            AnimationUpdate();
        
            Console.WriteLine($"Speed: {Speed}");

        }

        void Jump() {
            canJump = false;
            Speed = new Vector2(Speed.X, -Constants.JUMP_FORCE);
            Game1.SoundEffects[(int)SFX.JUMP].CreateInstance().Play();
        }

        void SlowBy(float slowAmount) {
            slowAmount = 1-Math.Abs(slowAmount);
            if (Speed.X == 0) return;
            Vector2 newSpeed;
            bool velocityNeg = (Speed.X < 0); //true if negative
            if (velocityNeg)
                newSpeed = new Vector2(Speed.X*slowAmount, Speed.Y);
            else
                newSpeed = new Vector2(Speed.X*slowAmount, Speed.Y);
            
            Speed = newSpeed;
            if (Math.Abs(Speed.X) < Constants.STOP_SPEED) Speed = new Vector2(0, Speed.Y); 
            // if ((velocityNeg && newSpeed.X < 0) || (!velocityNeg && newSpeed.X > 0)) //returns false if we changed the sign of Speed.X
            //     Speed = newSpeed;
            // else
            //     Speed = new Vector2(0, Speed.Y);
        }

        void AnimationUpdate() {
            AnimationState oldAnimationState = animationState;
            if (input.Left.Held && !input.Right.Held) facingLeft = true;
            else if (input.Right.Held && !input.Left.Held) facingLeft = false;

            //these are just naturally going to be complicated with a lot of logic not sure how else to deal with it
            if(input.Left.Held && Speed.X < 0 && touchingGround) animationState = AnimationState.WALKING_LEFT; //walking left
            else if(input.Right.Held && Speed.X > 0 && touchingGround) animationState = AnimationState.WALKING_RIGHT; //walking right
            else if (!touchingGround) {
                //in midair
                if (facingLeft) animationState = AnimationState.AIR_STRAFE_LEFT;
                else if (!facingLeft) animationState = AnimationState.AIR_STRAFE_RIGHT;
                TextRect = new Rectangle(0, 64, 32 ,64); animationFrame = 0;
            }
            else { //player is still on ground
                if (facingLeft) animationState = AnimationState.STILL_LEFT;
                else if (!facingLeft) animationState = AnimationState.STILL_RIGHT;
                TextRect = new Rectangle(0, 0, 32 ,64); animationFrame = 0;
            }

            //TODO: midair animations and movement
            
            //if facing left mirror the image
            if (animationState == AnimationState.STILL_LEFT || animationState == AnimationState.WALKING_LEFT || animationState == AnimationState.AIR_STRAFE_LEFT) {
                Effects = SpriteEffects.FlipHorizontally;
            } else Effects = SpriteEffects.None;
            
            //if walking, change the image each frame
            if (animationState == AnimationState.WALKING_LEFT || animationState == AnimationState.WALKING_RIGHT) {
                WalkingLoop();
            }
        }

        int animationFrame = 0;
        void WalkingLoop() {
            animationFrame++;
            if(animationFrame >= Constants.FRAMES_PER_ANIMATION_STEP) {
                animationFrame = 0;
                if (TextRect.X < 5*32) //6 is the number of frames in the walking animation
                    TextRect = new Rectangle(TextRect.X+32, 0, 32, 64);
                else
                    TextRect = new Rectangle(0, 0, 32, 64);
            }
        }

        enum AnimationState {
            STILL_LEFT,
            STILL_RIGHT,
            WALKING_LEFT,
            WALKING_RIGHT,
            AIR_STRAFE_LEFT,
            AIR_STRAFE_RIGHT,
        }
    }
}