using TiledSharp;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;

namespace Prototype_Golem
{
    public class LevelHandler
    {
        //global variables 👻👻👻
        //perhaps I can rewrite the getters to just get from loadedLevel instead of setting them manually inside of Load()
        public static int MapWidth {get; private set;}
        public static int MapHeight {get; private set;} //map width and height in tiles (not pixels) - for checking what tiles an entity is inside
        public static int[] CollisionMap {get; private set;}

        public Point PlayerSpawn {get {return loadedLevel.PlayerSpawn;} }
        public Vector2 CameraOrigin {get {return loadedLevel.CameraOrigin;} }
        

        Dictionary<string, Level> levels = new Dictionary<string, Level>();

        Level loadedLevel;

        public void Load(string levelName) {
            if (levels.ContainsKey(levelName)) {
                levels.TryGetValue(levelName, out loadedLevel);
            }
            else {
                Level newLevel = new Level(levelName);
                levels.Add(levelName, newLevel);
                loadedLevel = newLevel;
            }
            MapWidth = loadedLevel.MapWidth;
            MapHeight = loadedLevel.MapHeight;
            CollisionMap = loadedLevel.CollisionMap;
        }

        //why do C# properties even exist
        public Level GetLevel() {
            return loadedLevel;
        }
    }
}