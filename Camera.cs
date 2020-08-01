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
            
            //Console.WriteLine($"Cam: {(int) Pos.X}, {(int) Pos.Y}");

            Matrix translationMatrix = Matrix.CreateTranslation( (int) Pos.X, (int) Pos.Y, 0);
            Matrix translationMatrix2 = Matrix.CreateTranslation(graphicsDevice.Viewport.Width*0.5f, graphicsDevice.Viewport.Height*0.5f, 1);
            Matrix scaleMatrix = Matrix.CreateScale(Scalar, Scalar, 1);

            Matrix matrix = translationMatrix*scaleMatrix*translationMatrix2; //they have to go in this order for the camera to scale around the center
            //M41 and M42 (the two translation numbers) need to be rounded to prevent weird graphical glitch 
            matrix.M41 = (int)matrix.M41;
            matrix.M42 = (int)matrix.M42;
            return matrix;
        }
    }
}