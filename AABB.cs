using Microsoft.Xna.Framework;
using System;

namespace Prototype_Golem
{
    public class AABB : CollisionSystem
    {
        //right now while i am using floats for coordinates, bounding boxes should all be at least .001 smaller than a full block
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
            for(int i = 0; i < Game1.MapWidth*Game1.MapHeight; i++) {
                if (CollisionMask[i]) {
                    float selfLeft = offsetRight+Pos.X; float oldLeft = offsetRight+OldPos.X;
                    float selfTop = offsetDown+Pos.Y; float oldTop = offsetDown+OldPos.Y;
                    float selfRight = width+Pos.X; float oldRight = width+OldPos.X;
                    float selfBottom = height+Pos.Y; float oldBottom = height+OldPos.Y;
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
                        if (touchTop && touchLeft && touchBottom && touchRight) { //touching at all
                            Console.WriteLine("Bruh!");
                            if(oldBottom < tileTop && ((tileId&(int)CollisionDirections.TOP)!=0)) { //collided from tiles top
                                //throw new Exception("poop");
                                //Console.WriteLine("collided from tiles top!");
                                Pos = new Vector2(Pos.X, tileTop-height-0.001f);
                                Speed = new Vector2(Speed.X, 0);
                            }
                            else if (oldTop > tileBottom && ((tileId&(int)CollisionDirections.BOTTOM)!=0)) { //collided from tiles bottom
                                Console.WriteLine("collided from tiles bottom!");
                                Pos = new Vector2(Pos.X, tileBottom+0.001f);
                                Speed = new Vector2(Speed.X, 0);
                            }
                            else if (oldRight < tileLeft && ((tileId&(int)CollisionDirections.LEFT)!=0)) { //collided from tiles left
                                Console.WriteLine("collided from tiles left!");
                                Pos = new Vector2(tileLeft-width-0.001f, Pos.Y);
                                Speed = new Vector2(0, Speed.Y);
                            }
                            else if (oldLeft > tileRight && ((tileId&(int)CollisionDirections.RIGHT)!=0)) {//collided from tiles right
                                Console.WriteLine("collided from tiles right!");
                                Pos = new Vector2(tileRight+0.001f, Pos.Y);
                                Speed = new Vector2(0, Speed.Y);
                            }
                        }
                        //TODO: add platforms.
                        //possible todo, may need to address tunneling issues if they occur
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
                    if (y*Game1.MapWidth+x >= Game1.MapWidth*Game1.MapHeight || y*Game1.MapWidth+x < 0) continue;
                    CollisionMask[y*Game1.MapWidth+x] = true;
                }
            }
        }
    }
}