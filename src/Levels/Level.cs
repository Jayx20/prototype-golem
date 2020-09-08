using TiledSharp;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using static Prototype_Golem.Constants;

namespace Prototype_Golem.Levels
{
    public class Level
    {
        //TODO: allow different maps to use different tilesets
        //i should probably have some kind of way to lookup other properties to program into levels, or i can just put them inside the tmx file. (tilesets should already be this way but they just arent)
        public const string TILESET_NAME = "prototype1";
        public const string BASE_LAYER_NAME = "base";
        public const string OBJECTS_GROUP_NAME = "objects";
        public const string COLLISION_MASK_LAYER = "collisionMasks";
        public const string COLLISION_FLAG_LAYER = "collisionFlags";
        public int MapWidth {get; private set;}
        public int MapHeight {get; private set;} //map width and height in tiles (not pixels) - for checking what tiles an entity is inside
        public TileCollideInfo[] CollisionMap {get; private set;}
        public List<Entity> LevelEntities {get; set;}

        public int Columns {get; private set;}
        
        public Point PlayerSpawn {get; private set;} = new Point(TILE_WIDTH, TILE_WIDTH);
        public Vector2 CameraOrigin {get; private set;} = new Vector2(1, 1);
        

        public TmxTileset BaseTileset {get; private set;} //maybe todo allow for multiple tilesets to be used or just have a list so you have one for each layer
        TmxMap map;

        //returns the layers of the map in the order they are supposed to be drawn in
        //TODO: support more layers and figure out depth
        public TmxList<TmxLayer> GetLayers() {
            var layers = new TmxList<TmxLayer>();
            layers.Add(map.Layers[BASE_LAYER_NAME]);
            return layers;
        }
        public TmxList<TmxObject> Objects {
            get {
                if (map.ObjectGroups.Contains(OBJECTS_GROUP_NAME))
                    return map.ObjectGroups[OBJECTS_GROUP_NAME].Objects;
                else
                    return null;
            }
        }
        public Level(string mapName) {
            map = new TmxMap("Maps/"+mapName+".tmx");
            MapWidth = map.Width;
            MapHeight = map.Height;
            CollisionMap = MapToCollisionMap(map);
            BaseTileset = map.Tilesets[TILESET_NAME];
            Columns = BaseTileset.Columns ?? -1;
            if (Columns == -1) {
                throw new System.ArgumentException("Failed to load map "+mapName+".");
            }

            LevelEntities = new List<Entity>();
            
            if(Objects != null) {
                foreach(TmxObject tmxObject in Objects) {
                    //no idea how to do this except a massive switch statement for every single type of entity....
                    switch (tmxObject.Type) {
                        case "ruby":
                            //LevelEntities.Add(new Entities.Ruby(new Point((int)tmxObject.X, (int)(tmxObject.Y-tmxObject.Height))));
                            break;
                        case "spawn":
                            PlayerSpawn = new Point((int)tmxObject.X, (int)tmxObject.Y);
                            CameraOrigin = new Vector2((float)-tmxObject.X/TILE_WIDTH, (float)-tmxObject.Y/TILE_WIDTH);
                            break;
                        default:
                            Console.Error.WriteLine($"Unknown object in map {tmxObject.Type}");
                            break;
                        //TODO: implement a flexible way to add entities
                    }
                }
            }
        }

        TileCollideInfo[] MapToCollisionMap(TmxMap map) {
            var collisionMaskLayer = map.Layers[COLLISION_MASK_LAYER];
            int firstMaskGid = map.Tilesets[COLLISION_MASK_LAYER].FirstGid;

            var collisionFlagLayer = map.Layers[COLLISION_FLAG_LAYER];
            int firstFlagGid = map.Tilesets[COLLISION_FLAG_LAYER].FirstGid;

            TileCollideInfo[] collisionMap = new TileCollideInfo[map.Width*map.Height];

            for (int i = 0; i<map.Width*map.Height; i++) {
                int tileMaskID = collisionMaskLayer.Tiles[i].Gid;
                int tileFlagID = collisionFlagLayer.Tiles[i].Gid;
                if (tileFlagID != 0) collisionMap[i] = new TileCollideInfo(tileMaskID - firstMaskGid, tileFlagID - firstFlagGid);
                else collisionMap[i] = new TileCollideInfo(0, 0);
            }

            return collisionMap;
        }
    }
}