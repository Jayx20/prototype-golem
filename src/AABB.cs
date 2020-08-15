using System.Collections.Generic;
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

        public RectangleF SelfBounds {get {return new RectangleF(Pos.X+offsetRight, Pos.Y+offsetDown, width, height);}}
        public RectangleF HorizontalBounds {get {return new RectangleF(Pos.X+offsetRight, OldPos.Y+offsetDown, width, height);}}
        public RectangleF VerticalBounds {get {return new RectangleF(OldPos.X+offsetRight, Pos.Y+offsetDown, width, height);}}
        public RectangleF OldBounds {get {return new RectangleF(OldPos.X+offsetRight, OldPos.Y+offsetDown, width, height);}}

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

        protected override void CollideDetectEntities(List<Entity> entities)
        {
            foreach (Entity entity in entities) {
                if (entity.Collision == this) continue;
                for (int i = 0; i<CollisionMask.Length; i++) {
                    if (CollisionMask[i] && entity.Collision.CollisionMask[i]) {
                        //self and entity do have some tiles in common
                        //colliding against another rectangle
                        if(entity.Collision is AABB) {
                            if(SelfBounds.Intersects(((AABB)entity.Collision).SelfBounds)) {
                                CollidedEntities.Add(entity);
                                break;
                            }
                        } else
                            throw new NotImplementedException(); //nothing else than AABB exists atm.
                        
                    }
                }
            }
        }

        protected override void CollideResolveEntities() {
            foreach (Entity entity in CollidedEntities) {
                if(entity.Collision is AABB) {
                    if(entity.Collision.Clip) {
                        //resolve the collision
                        throw new NotImplementedException(); //no clipping entities besides player yet.

                    } //if the other entity doesn't clip you dont have to resolve anything
                } else
                    throw new NotImplementedException();
            }
        }

        protected override void CollideTiles()
        {
            for (int e = 0; e < 2; e++) { //two checks: horizontal, then vertical
                bool vertical = (e==1); //check horizontal first, then vertical
                for (int i = 0; i < LevelHandler.MapWidth*LevelHandler.MapHeight; i++) {
                    //go through every tile
                    if (CollisionMask[i]) { //proceed if the entity could be intersecting it
                        int tileId = LevelHandler.CollisionMap[i]; //Store the collision value for this tile
                        if (tileId == 0) continue; //Tile does not collide, proceed to check other tiles
                        RectangleF tileBounds = new RectangleF(i % LevelHandler.MapWidth, i / LevelHandler.MapWidth, 1, 1);
                        if (tileId < 16) { //standard tile with no slope
                            if (!vertical) { //horizontal
                                if (HorizontalBounds.Intersects(tileBounds)) {
                                    //Check if we entered through the left or through the right and respond accordingly
                                    if (OldBounds.Right < tileBounds.Left && ((tileId&(int)CollisionDirections.LEFT)!=0)) { //collided from tiles left
                                        Pos = new Vector2(tileBounds.Left-width-offsetRight-Constants.COLLISION_PUSH_DISTANCE, Pos.Y);
                                        Speed = new Vector2(0, Speed.Y);
                                        TouchedSides |= (int)CollisionDirections.LEFT;
                                    }
                                    else if (OldBounds.Left > tileBounds.Right && ((tileId&(int)CollisionDirections.RIGHT)!=0)) {//collided from tiles right
                                        Pos = new Vector2(tileBounds.Right-offsetRight+Constants.COLLISION_PUSH_DISTANCE, Pos.Y);
                                        Speed = new Vector2(0, Speed.Y);
                                        TouchedSides |= (int)CollisionDirections.RIGHT;
                                    }
                                }
                            } else { //vertical
                                if (VerticalBounds.Intersects(tileBounds)) {
                                    if(OldBounds.Bottom < tileBounds.Top && ((tileId&(int)CollisionDirections.TOP)!=0)) { //collided from tiles top
                                        Pos = new Vector2(Pos.X, tileBounds.Top-height-offsetDown-Constants.COLLISION_PUSH_DISTANCE);
                                        Speed = new Vector2(Speed.X, 0);
                                        TouchedSides |= (int)CollisionDirections.TOP;
                                    }
                                    else if (OldBounds.Top > tileBounds.Bottom && ((tileId&(int)CollisionDirections.BOTTOM)!=0)) { //collided from tiles bottom
                                        Pos = new Vector2(Pos.X, tileBounds.Bottom-offsetDown+Constants.COLLISION_PUSH_DISTANCE);
                                        Speed = new Vector2(Speed.X, 0);
                                        TouchedSides |= (int)CollisionDirections.BOTTOM;
                                    }
                                }
                            }

                        } else throw new NotImplementedException("Tile collision type not implemented.");
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