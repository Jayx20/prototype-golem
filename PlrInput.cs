using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Prototype_Golem
{
    public struct Button {
        // Press will only be true on the first frame a button is pressed.
        // Hold will be true at any time a button is pressed. (including the first frame)
        public bool Pressed;
        public bool Held;

        public Button(bool held, bool pressed) {
            Held = held;
            Pressed = pressed;
        }
    }

    public class PlrInput
    {

        //im so sorry to anybody who reads this
        public void Update(KeyboardState keyboard, MouseState mouse, GamePadState gamepad) {
            
            PlrInput oldInput = (PlrInput) this.MemberwiseClone(); //copying the old input to use it
            CursorPos = mouse.Position.ToVector2();
            //TODO: add logic for gamepad as the mouse later - i would guess if the joystick is not in a deadzone it should just override mouse

            //left
            if (keyboard.IsKeyDown(Keys.A) || gamepad.ThumbSticks.Left.X < 0) { //assumiung deadzones are on be default
                if(!oldInput.Left.Held) Left = new Button(true, true); //if it wasnt being touched before, this is the first frame so it was pressed.
                else Left = new Button(true, false);
            } else Left = new Button(false, false);
            //right
            if (keyboard.IsKeyDown(Keys.D) || gamepad.ThumbSticks.Left.X > 0) {
                if(!oldInput.Right.Held) Right = new Button(true, true);
                else Right = new Button(true, false);
            } else Right = new Button(false, false);
            //down
            if (keyboard.IsKeyDown(Keys.S) || gamepad.ThumbSticks.Left.Y < 0) {
                if(!oldInput.Down.Held) Down = new Button(true, true);
                else Down = new Button(true, false);
            } else Down = new Button(false, false);
            //up
            if (keyboard.IsKeyDown(Keys.W) || gamepad.ThumbSticks.Left.Y > 0) {
                if(!oldInput.Up.Held) Up = new Button(true, true);
                else Up = new Button(true, false);
            } else Up = new Button(false, false);

            //fire1
            if (mouse.LeftButton == ButtonState.Pressed || gamepad.IsButtonDown(Buttons.LeftShoulder)) {
                if(!oldInput.Fire1.Held) Fire1 = new Button(true, true);
                else Fire1 = new Button(true, false);
            } else Fire1 = new Button(false, false);
            //fire2
            if (mouse.RightButton == ButtonState.Pressed || gamepad.IsButtonDown(Buttons.RightShoulder)) {
                if(!oldInput.Fire2.Held) Fire2 = new Button(true, true);
                else Fire2 = new Button(true, false);
            } else Fire2 = new Button(false, false);

            //special1
            if (keyboard.IsKeyDown(Keys.Space) || gamepad.IsButtonDown(Buttons.RightShoulder)) {
                if(!oldInput.Special1.Held) Special1 = new Button(true, true);
                else Special1 = new Button(true, false);
            } else Special1 = new Button(false, false);
            //special2
            if (keyboard.IsKeyDown(Keys.LeftShift) || gamepad.IsButtonDown(Buttons.LeftShoulder)) {
                if(!oldInput.Special2.Held) Special2 = new Button(true, true);
                else Special2 = new Button(true, false);
            } else Special2 = new Button(false, false);

            //interact1
            if (keyboard.IsKeyDown(Keys.E) || gamepad.IsButtonDown(Buttons.A)) {
                if(!oldInput.Interact1.Held) Interact1 = new Button(true, true);
                else Interact1 = new Button(true, false);
            } else Interact1 = new Button(false, false);
            //interact2
            if (keyboard.IsKeyDown(Keys.Q) || gamepad.IsButtonDown(Buttons.B)) {
                if(!oldInput.Interact2.Held) Interact2 = new Button(true, true);
                else Interact2 = new Button(true, false);
            } else Interact2 = new Button(false, false);
            //reload
            if (keyboard.IsKeyDown(Keys.R) || gamepad.IsButtonDown(Buttons.X)) {
                if(!oldInput.Reload.Held) Reload = new Button(true, true);
                else Reload = new Button(true, false);
            } else Reload = new Button(false, false);
            //mech
            if (keyboard.IsKeyDown(Keys.F) || gamepad.IsButtonDown(Buttons.Y)) {
                if(!oldInput.Mech.Held) Mech = new Button(true, true);
                else Mech = new Button(true, false);
            } else Mech = new Button(false, false);

        }
        /*
            Left joystick on the controller.
            left - A on the keyboard. Move to the left.
            right - D on the keyboard. Move to the right.
            down - S on the keyboard. Move down.
            up - W on the keyboard. Move up/jump.
        */
        public Button Left {get; private set;}
        public Button Right {get; private set;}
        public Button Down {get; private set;}
        public Button Up {get; private set;}

        /*
            Both fire keys are used in combat.
            Fire1 - left click/bumper.
            Fire2 - right click/bumper. 
        */
        public Button Fire1 {get; private set;}
        public Button Fire2 {get; private set;}

        // cursor is either the mouse or the right joystick. (joystick positions are mapped to a circle around the player for aiming)
        public Vector2 CursorPos {get; private set;}

        /*
            Special buttons:
            special1 - Space or right trigger.
            special2 - Leftshift or left trigger. 
        */
        public Button Special1 {get; private set;}
        public Button Special2 {get; private set;}

        /*
            Interaction buttons:
            interact1 - E key or A button. used for interacting
            interact2 - Q key or B button. also used for interacting
            reload - R key or X button. self explanatory
            mech - F key or Y button. gets in and out of the mech
        */
        public Button Interact1 {get; private set;}
        public Button Interact2 {get; private set;}
        public Button Reload {get; private set;}
        public Button Mech {get; private set;}
    }
    
}