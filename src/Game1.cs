using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using System.Collections.Generic;
using System;

using TiledSharp;

namespace Prototype_Golem
{

    public class Game1 : Game
    {
        public static readonly int TILE_WIDTH = 32; //you can change this to make funny graphical things happen and test flexibility
        public static List<SoundEffect> SoundEffects {get; private set;}

        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;

        Camera camera = new Camera();

        Dictionary<TextureID, Texture2D> textureDict = new Dictionary<TextureID, Texture2D>();

        List<Entity> gameEntities = new List<Entity>();
        //does not include entities bound to the map

        LevelHandler levelHandler = new LevelHandler();


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
            camera = new Camera(-21.5f, -25, 1f); //nice starting position for the camera

            gameEntities.Add(new Player(new Vector2(24,31)));

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
            

        }

        protected override void Update(GameTime gameTime)
        {
            if(Keyboard.GetState().IsKeyDown(Keys.J)) { levelHandler.Load("test1"); gameEntities[0].Pos = new Vector2(16, 32);}
            if(Keyboard.GetState().IsKeyDown(Keys.K)) { levelHandler.Load("test2"); gameEntities[0].Pos = new Vector2(24, 31);}

            Level level = levelHandler.GetLevel();
            List<Entity> entities = new List<Entity>();
            entities.AddRange(gameEntities);
            if (level.LevelEntities.Count > 0) entities.AddRange(level.LevelEntities);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Console.WriteLine($"FPS: {1 / gameTime.ElapsedGameTime.TotalSeconds}");

            // Update logic here

            //sort of todo: bounds on the zoom and maybe change movement speed. however this will be pointless in the final product where camera zoom and position is determined from the players state
            //TODO: input class to at the very least add multiple buttons for one thing for controller support
            //additionally, an input class would make this look less dumb.
            if(Keyboard.GetState().IsKeyDown(Keys.Left)) {
                camera.Pos += new Vector2(Constants.CAMERA_SPEED,0);
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Right)) {
                camera.Pos += new Vector2(-Constants.CAMERA_SPEED,0);
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Up)) {
                camera.Pos += new Vector2(0,Constants.CAMERA_SPEED);
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Down)) {
                camera.Pos += new Vector2(0,-Constants.CAMERA_SPEED);
            }

            if(Keyboard.GetState().IsKeyDown(Keys.OemPlus)) {
                camera.Scalar += Constants.CAMERA_ZOOM_SPEED;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.OemMinus)) {
                camera.Scalar -= Constants.CAMERA_ZOOM_SPEED;
            }

            foreach (Entity entity in entities) {
                if (entity.Collide) entity.Collision.OldPos = entity.Pos;
                entity.Update();
                entity.Move();
                if (entity.Collide) {
                    entity.Collision.Pos = entity.Pos;
                    entity.Collision.Speed = entity.Speed;
                    entity.Collision.CollisionUpdate(level.CollisionMap); //add pos and speed as refs or out or something
                    entity.Pos = entity.Collision.Pos;
                    entity.Speed = entity.Collision.Speed;
                }
                
            }

            Console.WriteLine($"Player Pos: {entities[0].Pos.X}, {entities[0].Pos.Y}");
            Console.WriteLine($"Camera Pos: {camera.Pos.X}, {camera.Pos.Y}, Scale: {camera.Scalar}");
            
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


            TmxLayer baseLayer = level.GetLayers()[0];

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
                    Rectangle destRect = new Rectangle((int)(entity.Pos.X*TILE_WIDTH), (int)(entity.Pos.Y*TILE_WIDTH), entity.TextRect.Width, entity.TextRect.Height);
                    Rectangle sourceRect = entity.TextRect;
                    spriteBatch.Draw(entityTexture, destRect, sourceRect, Color.White, 0f, new Vector2(0,0), entity.Effects, 0f);
                }
            } 

            spriteBatch.End();

            base.Draw(gameTime);
        }


    }
}
