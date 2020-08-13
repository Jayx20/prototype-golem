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

        public AABB(Point topLeft, Point bottomRight) {
            this.width = (float)bottomRight.X/Game1.TILE_WIDTH - (float)topLeft.X/Game1.TILE_WIDTH;
            this.height = (float)bottomRight.Y/Game1.TILE_WIDTH - (float)topLeft.Y/Game1.TILE_WIDTH;
            this.offsetRight = (float)topLeft.X/Game1.TILE_WIDTH;
            this.offsetDown = (float)topLeft.Y/Game1.TILE_WIDTH;            
        }

        protected override void CollideEntities()
        {
            
        }

        protected override void CollideTiles()
        {
            for(int i = 0; i < LevelHandler.MapWidth*LevelHandler.MapHeight; i++) {
                if (CollisionMask[i]) {
                    float selfLeft = offsetRight+Pos.X; float oldLeft = offsetRight+OldPos.X;
                    float selfTop = offsetDown+Pos.Y; float oldTop = offsetDown+OldPos.Y;
                    float selfRight = width+offsetRight+Pos.X; float oldRight = width+offsetRight+OldPos.X;
                    float selfBottom = height+offsetDown+Pos.Y; float oldBottom = height+offsetDown+OldPos.Y;
                    //for every tile the entity intersects
                    int tileId = LevelHandler.CollisionMap[i];
                    if(tileId == 0) continue; //air
                    float tileLeft = i % LevelHandler.MapWidth;
                    float tileTop = i / LevelHandler.MapWidth;
                    float tileRight = tileLeft + 1;
                    float tileBottom = tileTop + 1;
                    if(tileId < 16) { //standard tile with no slope or anything
                        bool touchTop = (selfBottom > tileTop);
                        bool touchLeft = (selfRight > tileLeft);
                        bool touchBottom = (selfTop < tileBottom);
                        bool touchRight = (selfLeft < tileRight);
                        if (touchTop && touchLeft && touchBottom && touchRight) { //touching at all
                            if(oldBottom < tileTop && ((tileId&(int)CollisionDirections.TOP)!=0)) { //collided from tiles top
                                Pos = new Vector2(Pos.X, tileTop-height-offsetDown-0.001f);
                                Speed = new Vector2(Speed.X, 0);
                                TouchedSides |= (int)CollisionDirections.TOP;
                            }
                            else if (oldTop > tileBottom && ((tileId&(int)CollisionDirections.BOTTOM)!=0)) { //collided from tiles bottom
                                Pos = new Vector2(Pos.X, tileBottom-offsetDown+0.001f);
                                Speed = new Vector2(Speed.X, 0);
                                TouchedSides |= (int)CollisionDirections.BOTTOM;
                            }
                            else if (oldRight < tileLeft && ((tileId&(int)CollisionDirections.LEFT)!=0)) { //collided from tiles left
                                Pos = new Vector2(tileLeft-width-offsetRight-0.001f, Pos.Y);
                                Speed = new Vector2(0, Speed.Y);
                                TouchedSides |= (int)CollisionDirections.LEFT;
                            }
                            else if (oldLeft > tileRight && ((tileId&(int)CollisionDirections.RIGHT)!=0)) {//collided from tiles right
                                Pos = new Vector2(tileRight-offsetRight+0.001f, Pos.Y);
                                Speed = new Vector2(0, Speed.Y);
                                TouchedSides |= (int)CollisionDirections.RIGHT;
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
            int bottomRightX = (int)Math.Ceiling(width+Pos.X);
            int bottomRightY = (int)Math.Ceiling(height+Pos.Y);

            for(int x = topLeftX; x <= bottomRightX; x++) {
                for (int y = topLeftY; y <= bottomRightY; y++) {
                    if (y*LevelHandler.MapWidth+x >= LevelHandler.MapWidth*LevelHandler.MapHeight || y*LevelHandler.MapWidth+x < 0) continue;
                    CollisionMask[y*LevelHandler.MapWidth+x] = true;
                }
            }
        }
    }
}