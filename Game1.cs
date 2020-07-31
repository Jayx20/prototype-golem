using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

using TiledSharp;

namespace Prototype_Golem
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;

        Camera camera = new Camera();
        TmxMap testmap1;
        Dictionary<string, Texture2D> textureDict = new Dictionary<string, Texture2D>();

        List<Entity> entities = new List<Entity>();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Initialization logic here
            camera = new Camera(-330, -460, 2.5f); //nice starting position for the camera

            entities.Add(new Player(new Vector2(250,464)));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            testmap1 = new TmxMap("Maps/test1.tmx");

            Texture2D prototexture = Content.Load<Texture2D>("Images/prototype1"); //this variable will dissapear so the stupid name is fine the actual data is saved in the dictionary
            Texture2D entitytextures = Content.Load<Texture2D>("Images/entities_map");

            textureDict.Add("prototype1", prototexture);
            textureDict.Add("entities", entitytextures);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update logic here

            //sort of todo: bounds on the zoom and maybe change movement speed. however this will be pointless in the final product where camera zoom and position is determined from the players state
            //TODO: input class to at the very least add multiple buttons for one thing for controller support
            //additionally, an input class would make this look less dumb.
            if(Keyboard.GetState().IsKeyDown(Keys.Left)) {
                camera.Pos += new Vector2(5.0f,0);
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Right)) {
                camera.Pos += new Vector2(-5.0f,0);
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Up)) {
                camera.Pos += new Vector2(0,5.0f);
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Down)) {
                camera.Pos += new Vector2(0,-5.0f);
            }

            if(Keyboard.GetState().IsKeyDown(Keys.OemPlus)) {
                camera.Scalar += 0.05f;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.OemMinus)) {
                camera.Scalar -= 0.05f;
            }

            foreach (Entity entity in entities) {
                entity.Update();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            TmxTileset baseTileset = testmap1.Tilesets["prototype1"];
            int columns = baseTileset.Columns ?? -1;
            if (columns == -1) {
                throw new System.ArgumentException("HOW");
            } 

            //i should probably make a class or something that combines the TmxTileset and the actual Texture2D or just find a way to load the texture from the tileset instead of from monogames content manager
            //TODO: make this less stupid

            Texture2D tilesetTexture;
            Texture2D entitiesTexture;

            if (!textureDict.TryGetValue("prototype1", out tilesetTexture)) { //"prototype1" would be a variable after we find what tileset the map uses
                //crashes if loading the image failed
                throw new System.ArgumentException("Loading tilemap texture failed.");
            }
            if (!textureDict.TryGetValue("entities", out entitiesTexture)) {
                //crashes if loading the image failed
                throw new System.ArgumentException("Loading entities texture failed.");
            }


            TmxLayer baseLayer = testmap1.Layers["base"];

            spriteBatch.Begin(transformMatrix: camera.getMatrix(_graphics.GraphicsDevice),samplerState: SamplerState.PointClamp); //PointClamp makes scaling tiles look quite nice

            var tiles = baseLayer.Tiles;
            foreach(TmxLayerTile tile in tiles) {
                if (tile.Gid == 0) continue; //air
                
                Rectangle destRect = new Rectangle(tile.X*testmap1.TileWidth, tile.Y*testmap1.TileHeight, testmap1.TileWidth, testmap1.TileHeight); //where on the screen the tile is going to be drawn, based on the position of the tile

                //destRect.Location += camera.Pos.ToPoint(); this was how i moved the tile but i just move the entire spritebatch now
                //may be worth putting this back if I want to check if i need to cull the tiles and skip all this math

                int textureId = tile.Gid - baseTileset.FirstGid; 

                //funny math that has to do with 2d arrays and stuff
                    int textureX = textureId % columns;
                    int textureY = textureId / columns;

                Rectangle sourceRect = new Rectangle(textureX*testmap1.TileWidth, textureY*testmap1.TileHeight, testmap1.TileWidth, testmap1.TileHeight);
                spriteBatch.Draw(tilesetTexture, destRect, sourceRect, Color.White);
            }

            //TODO: depth in some way so sprites can be drawn in a different order like entities can be underneath some tiles
            foreach(Entity entity in entities) {
                if (entity is ITextured) {
                    ITextured entityT = entity as ITextured;
                    Rectangle destRect = new Rectangle((int)entity.Pos.X, (int)entity.Pos.Y, entityT.TextRect.Width, entityT.TextRect.Height);
                    Rectangle sourceRect = entityT.TextRect;
                    spriteBatch.Draw(entitiesTexture, destRect, sourceRect, Color.White);
                }
            } 

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
