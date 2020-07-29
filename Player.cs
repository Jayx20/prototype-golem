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
            Clip = true;
            inMech = false;
            input = new PlrInput();
        }
        public override void Update() {
            input.Update(Keyboard.GetState(), Mouse.GetState(), GamePad.GetState(PlayerIndex.One));

            if(input.Left.Held) {Pos += new Vector2(-.5f, 0);}
            if(input.Right.Held) {Pos += new Vector2(.5f, 0);}
            if(input.Up.Held) {Pos += new Vector2(0, .5f);}
            if(input.Down.Held) {Pos += new Vector2(0, -.5f);}

            Console.WriteLine($"pos = {Pos}");
            
        }

    }
}