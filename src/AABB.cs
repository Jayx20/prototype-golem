using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using static Prototype_Golem.Constants;

namespace Prototype_Golem
{
    public class AABB : CollisionSystem
    {
        //right now while i am using floats for coordinates, bounding boxes should all be at least .001 smaller than a full block
        //0, 0 being the top left, every 1 is a tile width. So a 1x1 thing would just be 0, 0, 1, 1
        int offsetDown; int offsetRight; //usually going to be 0, used if the bounding box is inside the entity
        int width;
        int height;

        public Rectangle SelfBounds {get {return new Rectangle(Pos.X+offsetRight, Pos.Y+offsetDown, width, height);}}
        public Rectangle HorizontalBounds {get {return new Rectangle(Pos.X+offsetRight, OldPos.Y+offsetDown, width, height);}}
        public Rectangle VerticalBounds {get {return new Rectangle(OldPos.X+offsetRight, Pos.Y+offsetDown, width, height);}}
        public Rectangle OldBounds {get {return new Rectangle(OldPos.X+offsetRight, OldPos.Y+offsetDown, width, height);}}

        public AABB(int width, int height, int offsetDown = 0, int offsetRight = 0) {
            this.width = width;
            this.height = height;
            this.offsetDown = offsetDown;
            this.offsetRight = offsetRight;
        }

        public AABB(Point topLeft, Point bottomRight) {
            this.width = bottomRight.X - topLeft.X;
            this.height = bottomRight.Y - topLeft.Y;
            this.offsetRight = topLeft.X;
            this.offsetDown = topLeft.Y;
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
                        Rectangle tileBounds = new Rectangle((i % LevelHandler.MapWidth)*TILE_WIDTH, (i / LevelHandler.MapWidth)*TILE_WIDTH, TILE_WIDTH, TILE_WIDTH);
                        if (tileId < 16) { //standard tile with no slope
                            if (!vertical)
                                if (HorizontalBounds.Intersects(tileBounds))
                                    ResolveHorizontalIntersect(tileId, tileBounds);
                            else
                                if (VerticalBounds.Intersects(tileBounds))
                                    ResolveVerticalIntersect(tileId, tileBounds);
                        } else throw new NotImplementedException("Tile collision type not implemented.");
                    }
                }
            }
        }

        void ResolveHorizontalIntersect(int tileId, Rectangle tileBounds) {
            //Check if we entered through the left or through the right and respond accordingly
            if (OldBounds.Right <= tileBounds.Left && ((tileId&(int)CollisionDirections.LEFT)!=0)) { //collided from tiles left
                Pos = new Point(tileBounds.Left-width-offsetRight, Pos.Y);
                Speed = new Vector2(0, Speed.Y);
                TouchedSides |= (int)CollisionDirections.LEFT;
            }
            else if (OldBounds.Left >= tileBounds.Right && ((tileId&(int)CollisionDirections.RIGHT)!=0)) {//collided from tiles right
                Pos = new Point(tileBounds.Right-offsetRight, Pos.Y);
                Speed = new Vector2(0, Speed.Y);
                TouchedSides |= (int)CollisionDirections.RIGHT;
            }
        }

        void ResolveVerticalIntersect(int tileId, Rectangle tileBounds) {
            if(OldBounds.Bottom <= tileBounds.Top && ((tileId&(int)CollisionDirections.TOP)!=0)) { //collided from tiles top
                Pos = new Point(Pos.X, tileBounds.Top-height-offsetDown);
                Speed = new Vector2(Speed.X, 0);
                TouchedSides |= (int)CollisionDirections.TOP;
            }
            else if (OldBounds.Top >= tileBounds.Bottom && ((tileId&(int)CollisionDirections.BOTTOM)!=0)) { //collided from tiles bottom
                Pos = new Point(Pos.X, tileBounds.Bottom-offsetDown);
                Speed = new Vector2(Speed.X, 0);
                TouchedSides |= (int)CollisionDirections.BOTTOM;
            }
        }

        protected override void UpdateCollisionMask()
        {
            int topLeftX = (int)(((float)offsetRight+(float)Pos.X)/TILE_WIDTH);
            int topLeftY = (int)(((float)offsetDown+(float)Pos.Y)/TILE_WIDTH);
            int bottomRightX = (int)Math.Ceiling(((float)width+(float)Pos.X)/TILE_WIDTH);
            int bottomRightY = (int)Math.Ceiling(((float)height+(float)Pos.Y)/TILE_WIDTH);

            for(int x = topLeftX; x <= bottomRightX; x++) {
                for (int y = topLeftY; y <= bottomRightY; y++) {
                    if (y*LevelHandler.MapWidth+x >= LevelHandler.MapWidth*LevelHandler.MapHeight || y*LevelHandler.MapWidth+x < 0) continue;
                    CollisionMask[y*LevelHandler.MapWidth+x] = true;
                }
            }

            for (int i = 0; i < CollisionMask.Length; i++) {
                CollisionMask[i] = true;
            }
        }
    }
}