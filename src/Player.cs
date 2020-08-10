using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Prototype_Golem
{
    public class Player:Entity
    {
        bool inMech;
        bool canJump = false;
        bool touchingGround = false;
        PlrInput input;

        public Player(Vector2 pos) {
            //spawns player outside of mech
            Pos = pos;
            Collide = true;
            Render = true;
            inMech = false;
            input = new PlrInput();
            TextRect = new Rectangle(0, 0, 32, 32);
            Collision = new AABB(1.875f, 1.875f,.0625f, .0625f); //the player is 2x2 with a pixel taken off an each side
        }
        public override void Update() {
            input.Update(Keyboard.GetState(), Mouse.GetState(), GamePad.GetState(PlayerIndex.One));

            //experimental gravity - not sure if i will have an acceleration variable
            Vector2 gravity = new Vector2(0, 0.05f); //

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
            if(input.Left.Held)  Speed += new Vector2(-.2f, 0);
            if(input.Right.Held) Speed += new Vector2(.2f, 0);
            if(input.Up.Held || input.Interact1.Held) gravity*=.5f; //
            if((input.Up.Pressed || input.Interact1.Pressed) && canJump) {
                canJump = false;
                Speed = new Vector2(Speed.X, -.52f);
                Game1.SoundEffects[(int)SFX.JUMP].CreateInstance().Play();
            } //
            //if(input.Down.Held) {Speed += new Vector2(0, .2f);}

            Speed += gravity;
            //Console.WriteLine($"pos = {Pos}");
        
        }

    }
}