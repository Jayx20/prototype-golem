using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using System.Collections.Generic;
using System;

using TiledSharp;

using static Prototype_Golem.Constants;

namespace Prototype_Golem
{

    public class Game1 : Game
    {
        public static List<SoundEffect> SoundEffects {get; private set;}

        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;

        Camera camera = new Camera();

        Dictionary<TextureID, Texture2D> textureDict = new Dictionary<TextureID, Texture2D>();

        List<Entity> gameEntities = new List<Entity>();
        //does not include entities bound to the map

        LevelHandler levelHandler = new LevelHandler();

        //debug
        static Player player;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = Constants.SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = Constants.SCREEN_HEIGHT;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Initialization logic here
            camera = new Camera(-29f, -24f, 1f); //nice starting position for the camera

            gameEntities.Add(new Player(new Point(24*TILE_WIDTH,31*TILE_WIDTH))); player = (Player)gameEntities[0];
            //gameEntities.Add(new Entities.Ruby(new Vector2(54.5f,8f))); ruby = (Entities.Ruby)gameEntities[1];

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            levelHandler.Load("test2");
            //these need to run every time a map is loaded
            SoundEffects = new List<SoundEffect>();

            textureDict.Add(TextureID.PROTOTYPE, Content.Load<Texture2D>("Images/prototype1"));
            textureDict.Add(TextureID.ENTITIES_1, Content.Load<Texture2D>("Images/entities_map"));
            textureDict.Add(TextureID.PLAYER, Content.Load<Texture2D>("Images/player"));

            SoundEffects.Insert((int)SFX.JUMP, Content.Load<SoundEffect>("Sounds/woosh"));
            SoundEffects.Insert((int)SFX.THUMP, Content.Load<SoundEffect>("Sounds/thump"));
            SoundEffects.Insert((int)SFX.PICKUP, Content.Load<SoundEffect>("Sounds/pickup"));
            

        }

        protected override void Update(GameTime gameTime)
        {
            if(Keyboard.GetState().IsKeyDown(Keys.J)) { levelHandler.Load("test1"); gameEntities[0].Pos = new Point(16*TILE_WIDTH, 32*TILE_WIDTH); camera = new Camera(-20f, -25f, 1f);}
            if(Keyboard.GetState().IsKeyDown(Keys.K)) { levelHandler.Load("test2"); gameEntities[0].Pos = new Point(24*TILE_WIDTH, 31*TILE_WIDTH); camera = new Camera(-29f, -24f, 1f);}
            if(Keyboard.GetState().IsKeyDown(Keys.L)) { levelHandler.Load("test3"); gameEntities[0].Pos = levelHandler.PlayerSpawn; camera = new Camera(levelHandler.CameraOrigin.X, levelHandler.CameraOrigin.Y, 1f);}
            

            Level level = levelHandler.GetLevel();
            List<Entity> entities = new List<Entity>();
            entities.AddRange(gameEntities);
            if (level.LevelEntities.Count > 0) entities.AddRange(level.LevelEntities);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            camera.Update();

            // Update logic here
            //i have created a monstrosity
            foreach (Entity entity in entities) {
                if (entity.Collide) entity.Collision.OldPos = entity.Pos;
                entity.Update();
                entity.Move();
                if (entity.Collide) {
                    entity.Collision.Pos = entity.Pos;
                    entity.Collision.Speed = entity.Speed;
                    entity.Collision.PrepareUpdate();
                }
            }
            foreach (Entity entity in entities) {
                if (entity.Collide) {
                    entity.Collision.CollisionUpdate(level.CollisionMap, entities);
                    entity.Pos = entity.Collision.Pos;
                    entity.Speed = entity.Collision.Speed;
                    foreach(Entity collidedEntity in entity.Collision.CollidedEntities) {
                        entity.CollideWith(collidedEntity);
                    }
                }
            }
            for (int i = 0; i<gameEntities.Count; i++) {
                if (gameEntities[i].Delete) gameEntities.RemoveAt(i);
            }
            for (int i = 0; i<levelHandler.GetLevel().LevelEntities.Count; i++) {
                if (levelHandler.GetLevel().LevelEntities[i].Delete) levelHandler.GetLevel().LevelEntities.RemoveAt(i);
            }

            Console.WriteLine($"Player Pos: {entities[0].Pos.X}, {entities[0].Pos.Y}");
            Console.WriteLine($"Camera Pos: {camera.Pos.X/TILE_WIDTH}, {camera.Pos.Y/TILE_WIDTH}, Scale: {camera.Scalar}");
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Level level = levelHandler.GetLevel();
            List<Entity> entities = new List<Entity>();
            entities.AddRange(gameEntities);
            if (level.LevelEntities.Count > 0) entities.AddRange(level.LevelEntities);
            
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //i should probably make a class or something that combines the TmxTileset and the actual Texture2D or just find a way to load the texture from the tileset instead of from monogames content manager
            //TODO: make this less stupid
            //TODO: see above im just putting it again so its a higher priority

            Texture2D tilesetTexture;

            if (!textureDict.TryGetValue(TextureID.PROTOTYPE, out tilesetTexture)) { //"prototype1" would be a variable after we find what tileset the map uses
                //crashes if loading the image failed
                throw new System.ArgumentException("Loading tilemap texture failed.");
            }


            TmxLayer baseLayer = level.GetLayers()["base"];

            spriteBatch.Begin(transformMatrix: camera.getMatrix(_graphics.GraphicsDevice),samplerState: SamplerState.PointClamp); //PointClamp makes scaling tiles look quite nice

            var tiles = baseLayer.Tiles;
            foreach(TmxLayerTile tile in tiles) {
                if (tile.Gid == 0) continue; //air
                
                Rectangle destRect = new Rectangle(tile.X*TILE_WIDTH, tile.Y*TILE_WIDTH, TILE_WIDTH, TILE_WIDTH); //where on the screen the tile is going to be drawn, based on the position of the tile

                //destRect.Location += camera.Pos.ToPoint(); this was how i moved the tile but i just move the entire spritebatch now
                //may be worth putting this back if I want to check if i need to cull the tiles and skip all this math

                int textureId = tile.Gid - level.BaseTileset.FirstGid; 

                //funny math that has to do with 2d arrays and stuff
                    int textureX = textureId % level.Columns;
                    int textureY = textureId / level.Columns;

                Rectangle sourceRect = new Rectangle(textureX*TILE_WIDTH, textureY*TILE_WIDTH, TILE_WIDTH, TILE_WIDTH);
                spriteBatch.Draw(tilesetTexture, destRect, sourceRect, Color.White);
            }

            //TODO: depth in some way so sprites can be drawn in a different order like entities can be underneath some tiles
            foreach(Entity entity in entities) {
                if (entity.Render) {
                    Texture2D entityTexture;
                    textureDict.TryGetValue(entity.TextID, out entityTexture);
                    Rectangle destRect = new Rectangle(entity.Pos.X, entity.Pos.Y, entity.TextRect.Width, entity.TextRect.Height);
                    Rectangle sourceRect = entity.TextRect;
                    spriteBatch.Draw(entityTexture, destRect, sourceRect, Color.White, 0f, new Vector2(0,0), entity.Effects, 0f);
                }
            } 

            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
