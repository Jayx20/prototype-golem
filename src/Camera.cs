using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Prototype_Golem
{
    public class Camera
    {

        public Vector2 Pos {get; set;} //Position of the camera
        public float Scalar {get; set;} //Zoom value

        public Camera() {
            Scalar = 1f;
        }

        public Camera(float x, float y, float scalar) {
            Pos = new Vector2(x*Game1.TILE_WIDTH,y*Game1.TILE_WIDTH);
            Scalar = scalar;
        }

        public Matrix getMatrix(GraphicsDevice graphicsDevice) {

            Matrix translationMatrix = Matrix.CreateTranslation( (int) (Pos.X), (int) (Pos.Y), 0);
            Matrix translationMatrix2 = Matrix.CreateTranslation(graphicsDevice.Viewport.Width*0.5f, graphicsDevice.Viewport.Height*0.5f, 1);
            Matrix scaleMatrix = Matrix.CreateScale(Scalar, Scalar, 1);

            Matrix matrix = translationMatrix*scaleMatrix*translationMatrix2; //they have to go in this order for the camera to scale around the center
            return matrix;
        }

        public void Update() {
            //TODO: delete this and make the camera follow the player
            if((Keyboard.GetState().IsKeyDown(Keys.Left) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft))
                && Pos.X+Constants.SCREEN_WIDTH*.5f < 0
            ) {
                Pos += new Vector2(Constants.CAMERA_SPEED,0);
            }
            
            if((Keyboard.GetState().IsKeyDown(Keys.Right) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight))
                && Pos.X-Constants.SCREEN_WIDTH*.5f > -LevelHandler.MapWidth*Game1.TILE_WIDTH
            ) {
                Pos += new Vector2(-Constants.CAMERA_SPEED,0);
            }
            
            if((Keyboard.GetState().IsKeyDown(Keys.Up) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadUp))
                && Pos.Y+Constants.SCREEN_HEIGHT*.5f < 0
            ) {
                Pos += new Vector2(0,Constants.CAMERA_SPEED);
            }
            
            if((Keyboard.GetState().IsKeyDown(Keys.Down) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown))
                && Pos.Y-Constants.SCREEN_HEIGHT*.5f > -LevelHandler.MapHeight*Game1.TILE_WIDTH
            ) {
                Pos += new Vector2(0,-Constants.CAMERA_SPEED);
            }

            if(Keyboard.GetState().IsKeyDown(Keys.OemPlus)) {
                Scalar += Constants.CAMERA_ZOOM_SPEED;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.OemMinus)) {
                Scalar -= Constants.CAMERA_ZOOM_SPEED;
            }
        }
    }
}