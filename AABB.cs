using Microsoft.Xna.Framework;
using System;

namespace Prototype_Golem
{
    public class AABB : CollisionSystem
    {
        //0, 0 being the top left, every 1 is a tile width. So a 1x1 thing would just be 0, 0, 1, 1
        float offsetDown; float offsetRight; //usually going to be 0, used if the bounding box is inside the entity
        float width;
        float height;

        public AABB(float width, float height, float offsetDown = 0, float offsetRight = 0) {
            this.width = width;
            this.height = height;
            this.offsetDown = offsetDown;
            this.offsetRight = offsetRight;
        }

        protected override void CollideEntities()
        {
            
        }

        protected override void CollideTiles()
        {
            float selfLeft = offsetRight+Pos.X;
            float selfTop = offsetDown+Pos.Y;
            float selfRight = width+Pos.X;
            float selfBottom = width+Pos.Y;
            for(int i = 0; i < Game1.MapWidth*Game1.MapHeight; i++) {
                if (CollisionMask[i]) {
                    //for every tile the entity intersects
                    int tileId = Game1.CollisionMap[i];
                    if(tileId == 0) continue; //air
                    float tileLeft = i % Game1.MapWidth;
                    float tileTop = i / Game1.MapWidth;
                    float tileRight = tileLeft + 1;
                    float tileBottom = tileTop + 1;
                    if(tileId < 16) { //standard tile with no slope or anything
                        bool touchTop = (selfBottom > tileTop);
                        bool touchLeft = (selfRight > tileLeft);
                        bool touchBottom = (selfTop < tileBottom);
                        bool touchRight = (selfLeft < tileRight);
                        if (touchTop && touchLeft && touchBottom && touchRight) {
                            Console.WriteLine("Bruh!");
                        }
                        //TODO: add platforms. I am thinking you could take into account the players location during the previous frame but idk.
                    }
                    else {
                        throw new NotImplementedException();
                    }
                }
            }
        }

        protected override void UpdateCollisionMask()
        {
            int topLeftX = (int)(offsetRight+Pos.X);
            int topLeftY = (int)(offsetDown+Pos.Y);
            int bottomRightX = (int)(width+Pos.X);
            int bottomRightY = (int)(width+Pos.Y);

            for(int x = topLeftX; x <= bottomRightX; x++) {
                for (int y = topLeftY; y <= bottomRightY; y++) {
                    if (y*Game1.MapWidth+x > Game1.MapWidth*Game1.MapHeight || y*Game1.MapWidth+x < 0) continue;
                    CollisionMask[y*Game1.MapWidth+x] = true;
                }
            }
        }
    }
}