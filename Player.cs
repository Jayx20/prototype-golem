using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Prototype_Golem
{
    public class Player:Entity
    {
        bool inMech;
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

            Speed = new Vector2();
            if(input.Left.Held) {Speed += new Vector2(-.2f, 0);}
            if(input.Right.Held) {Speed += new Vector2(.2f, 0);}
            if(input.Up.Held) {Speed += new Vector2(0, -.2f);} //up is negative and down is positive in this idk if i should render backwards or just leave it
            if(input.Down.Held) {Speed += new Vector2(0, .2f);}

            Console.WriteLine($"pos = {Pos}");
            
        }

    }
}