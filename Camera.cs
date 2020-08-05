using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Prototype_Golem
{
    public class Camera
    {

        public Vector2 Pos {get; set;} //Position of the camera
        public float Scalar {get; set;} //Zoom value

        public Camera() {
            Scalar = 1;
        }

        public Camera(float x, float y, float scalar) {
            Pos = new Vector2(x,y);
            Scalar = scalar;
        }

        public Matrix getMatrix(GraphicsDevice graphicsDevice) {

            Matrix translationMatrix = Matrix.CreateTranslation( (int) (Pos.X * Game1.TILE_WIDTH), (int) (Pos.Y * Game1.TILE_WIDTH), 0);
            Matrix translationMatrix2 = Matrix.CreateTranslation(graphicsDevice.Viewport.Width*0.5f, graphicsDevice.Viewport.Height*0.5f, 1);
            Matrix scaleMatrix = Matrix.CreateScale(Scalar, Scalar, 1);

            Matrix matrix = translationMatrix*scaleMatrix*translationMatrix2; //they have to go in this order for the camera to scale around the center
            return matrix;
        }
    }
}