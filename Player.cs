using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Prototype_Golem
{
    public class Player:Entity, ITextured
    {
        bool inMech;
        PlrInput input;

        public Rectangle TextRect {get; private set;}

        public Player(Vector2 pos) {
            //spawns player outside of mech
            Pos = pos;
            Clip = true;
            inMech = false;
            input = new PlrInput();
            TextRect = new Rectangle(0, 0, 32, 32);
        }
        public override void Update() {
            input.Update(Keyboard.GetState(), Mouse.GetState(), GamePad.GetState(PlayerIndex.One));

            if(input.Left.Held) {Pos += new Vector2(-1f, 0);}
            if(input.Right.Held) {Pos += new Vector2(1f, 0);}
            if(input.Up.Held) {Pos += new Vector2(0, -1f);} //up is negative and down is positive in this idk if i should render backwards or just leave it
            if(input.Down.Held) {Pos += new Vector2(0, 1f);}

            Console.WriteLine($"pos = {Pos}");
            
        }

    }
}