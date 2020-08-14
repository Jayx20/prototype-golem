using TiledSharp;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;

namespace Prototype_Golem
{
    public class Level
    {
        //TODO: allow different maps to use different tilesets
        //i should probably have some kind of way to lookup other properties to program into levels, or i can just put them inside the tmx file. (tilesets should already be this way but they just arent)
        static readonly string TILESET_NAME = "prototype1";
        public int MapWidth {get; private set;}
        public int MapHeight {get; private set;} //map width and height in tiles (not pixels) - for checking what tiles an entity is inside
        public int[] CollisionMap {get; private set;}
        public List<Entity> LevelEntities {get; set;}

        public int Columns {get; private set;}
        
        public Vector2 PlayerSpawn {get; private set;} = new Vector2(1, 1);
        public Vector2 CameraOrigin {get; private set;} = new Vector2(1, 1);
        

        public TmxTileset BaseTileset {get; private set;} //maybe todo allow for multiple tilesets to be used or just have a list so you have one for each layer
        TmxMap map;

        //returns the layers of the map in the order they are supposed to be drawn in
        //TODO: support more layers and figure out depth
        public TmxList<TmxLayer> GetLayers() {
            var layers = new TmxList<TmxLayer>();
            layers.Add(map.Layers["base"]);
            return layers;
        }
        public TmxList<TmxObject> Objects {
            get {
                if (map.ObjectGroups.Contains("objects"))
                    return map.ObjectGroups["objects"].Objects;
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
                            LevelEntities.Add(new Entities.Ruby(new Vector2((float)tmxObject.X/Game1.TILE_WIDTH, (float)(tmxObject.Y-tmxObject.Height)/Game1.TILE_WIDTH)));
                            break;
                        case "spawn":
                            PlayerSpawn = new Vector2((float)tmxObject.X/Game1.TILE_WIDTH, (float)tmxObject.Y/Game1.TILE_WIDTH);
                            CameraOrigin = new Vector2((float)-tmxObject.X/Game1.TILE_WIDTH, (float)-tmxObject.Y/Game1.TILE_WIDTH);
                            break;
                        default:
                            Console.Error.WriteLine($"Unknown object in map {tmxObject.Type}");
                            break;
                            
                    }
                }
            }
        }

        int[] MapToCollisionMap(TmxMap map) {
            var collisionLayer = map.Layers["collision"];
            int firstGid = map.Tilesets["collision"].FirstGid;

            int[] collisionMap = new int[map.Width*map.Height];

            int i = 0;
            foreach (TmxLayerTile tile in collisionLayer.Tiles) {
                if (tile.Gid != 0) collisionMap[i] = tile.Gid - firstGid;
                else collisionMap[i] = 0;
                i++;
            }

            return collisionMap;
        }
    }
}