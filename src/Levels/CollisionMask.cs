using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static Prototype_Golem.Constants;

namespace Prototype_Golem.Levels
{
    public class CollisionMask
    {
        public static CollisionMask[] maskList {get; private set;}
        
        public const int MASK_IMAGE_WIDTH = 4;
        public const int MASK_IMAGE_HEIGHT = 4;
        public const string MASK_IMAGE_NAME = "Images/collisionMasks";

        public static void InitMaskList(Texture2D maskImage) {
            if (TILE_WIDTH != 32)
                //i am fully aware this is a really stupid way to handle this but im assuming TILE_WIDTH is just gonna stay as 32
                //if it is changed to 16 just use a short
                throw new System.Exception("TILE_WIDTH is not 32. Refactor CollisionMask.cs to use not ints.");

            maskList = new CollisionMask[MASK_IMAGE_WIDTH*MASK_IMAGE_HEIGHT];
            
            if (maskImage.Width/TILE_WIDTH != MASK_IMAGE_WIDTH || maskImage.Height/TILE_WIDTH != MASK_IMAGE_HEIGHT)
                throw new System.Exception($"Collision mask image has incorrect dimensions. Expected dimensions: {MASK_IMAGE_WIDTH*TILE_WIDTH}x{MASK_IMAGE_HEIGHT*Constants.TILE_WIDTH}");

            Color[] rawImageData = new Color[maskImage.Width * maskImage.Height];
            
            maskImage.GetData<Color>(rawImageData);

            for (int i = 0; i<MASK_IMAGE_WIDTH*MASK_IMAGE_HEIGHT; i++) {
                maskList[i] = new CollisionMask(rawImageData, i%MASK_IMAGE_WIDTH, i/MASK_IMAGE_WIDTH, maskImage.Width);
            }

            if (DEBUG_PRINT) {
                Console.WriteLine("CollisionMask Debug Data:");
                for (int mask = 0; mask<maskList.Length; mask++) {
                    Console.WriteLine($"Mask number {mask}:");
                    for(int column = 0; column<TILE_WIDTH; column++) {
                        string columnBinary = Convert.ToString(maskList[mask].columns[column], toBase: 2);
                        Console.WriteLine($"\t{columnBinary}");
                    }
                }
            }
        }

        CollisionMask(Color[] rawImageData, int maskX, int maskY, int imageWidth) {
            maskX *= TILE_WIDTH;
            maskY *= TILE_WIDTH;
            for (int x = 0; x<TILE_WIDTH; x++) {
                //for each column...
                for (int y = 0; y<TILE_WIDTH; y++) {
                    //for each pixel...
                    if (rawImageData[ (maskY+y)*imageWidth + x+maskX ].A >= 127) { //if the pixel has a solid color
                        int bit = 1 << y; //will give the 1st bit, 2nd bit, whatever bit position Y is
                        columns[x] |= bit; //turns that bit on
                    }
                }
            }
        }

        int[] columns = new int[TILE_WIDTH];
    }
}