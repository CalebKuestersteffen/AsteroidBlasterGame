using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace AsteroidBlaster
{
    public class AsteroidBlaster : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        
        private AsteroidSprite[] asteroids;
        private ShipSprite ship;
        private LaserSprite laser;
        private SpriteFont spriteFont;
        private int livesLeft;
        private int asteroidsShot;
        private bool destructionRendered = false;
        private SoundEffect asteroidDestruction;
        private SoundEffect shipDestruction;

        /// <summary>
        /// A game demonstrating collision detection
        /// </summary>
        public AsteroidBlaster()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Title = "Asteroid Blaster";
        }

        /// <summary>
        /// Initializes the game 
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            System.Random rand = new System.Random();

            // generate asteroids on the top half of the screen
            asteroids = new AsteroidSprite[]
            {
                new AsteroidSprite(this, new Vector2((float)rand.NextDouble() * GraphicsDevice.Viewport.Width, (float)rand.NextDouble() * GraphicsDevice.Viewport.Height/2)),
                new AsteroidSprite(this, new Vector2((float)rand.NextDouble() * GraphicsDevice.Viewport.Width, (float)rand.NextDouble() * GraphicsDevice.Viewport.Height/2)),
                new AsteroidSprite(this, new Vector2((float)rand.NextDouble() * GraphicsDevice.Viewport.Width, (float)rand.NextDouble() * GraphicsDevice.Viewport.Height/2)),
                new AsteroidSprite(this, new Vector2((float)rand.NextDouble() * GraphicsDevice.Viewport.Width, (float)rand.NextDouble() * GraphicsDevice.Viewport.Height/2)),
                new AsteroidSprite(this, new Vector2((float)rand.NextDouble() * GraphicsDevice.Viewport.Width, (float)rand.NextDouble() * GraphicsDevice.Viewport.Height/2)),
                new AsteroidSprite(this, new Vector2((float)rand.NextDouble() * GraphicsDevice.Viewport.Width, (float)rand.NextDouble() * GraphicsDevice.Viewport.Height/2)),
                new AsteroidSprite(this, new Vector2((float)rand.NextDouble() * GraphicsDevice.Viewport.Width, (float)rand.NextDouble() * GraphicsDevice.Viewport.Height/2)),
                new AsteroidSprite(this, new Vector2((float)rand.NextDouble() * GraphicsDevice.Viewport.Width, (float)rand.NextDouble() * GraphicsDevice.Viewport.Height/2)),
                new AsteroidSprite(this, new Vector2((float)rand.NextDouble() * GraphicsDevice.Viewport.Width, (float)rand.NextDouble() * GraphicsDevice.Viewport.Height/2)),
                new AsteroidSprite(this, new Vector2((float)rand.NextDouble() * GraphicsDevice.Viewport.Width, (float)rand.NextDouble() * GraphicsDevice.Viewport.Height/2)),
            };


            livesLeft = 3;
            ship = new ShipSprite(this);
            laser = new LaserSprite(this, ship);

            base.Initialize();
        }

        /// <summary>
        /// Loads content for the game
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            foreach (var asteroid in asteroids) asteroid.LoadContent(Content);
            ship.LoadContent(Content);
            laser.LoadContent(Content);
            asteroidDestruction = Content.Load<SoundEffect>("Explosion");
            shipDestruction = Content.Load<SoundEffect>("ShipExplosion");
            spriteFont = Content.Load<SpriteFont>("arial");
        }

        /// <summary>
        /// Updates the game world
        /// </summary>
        /// <param name="gameTime">The game time</param>
        protected override void Update(GameTime gameTime)
        {
            
            // escape or back button closes the game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            #region Update while player is alive

            // TODO: Add your update logic here
            if (ship.Destroyed == false)
            {
                ship.Update(gameTime);
                laser.Update(gameTime);

                // Detect and process collisions
                ship.Color = Color.White;
                foreach (var asteroid in asteroids)
                {
                    // Update the position of the asteroid
                    asteroid.Update(gameTime);

                    // Check if the asteroid has collided with the ship
                    if (!asteroid.Hit && asteroid.Bounds.CollidesWith(ship.Bounds))
                    {
                        ship.Color = Color.Red;
                        asteroid.Hit = true;
                        livesLeft--;
                        if (livesLeft == 0)
                        {
                            shipDestruction.Play();
                            ship.Color = Color.White;
                            ship.Destroyed = true;
                            ship.Update(gameTime);
                        }
                        else
                        {
                            asteroidDestruction.Play();
                        }
                    }
                    else if (!asteroid.Hit && asteroid.Bounds.CollidesWith(laser.Bounds))
                    {
                        asteroidDestruction.Play();
                        asteroid.Hit = true;
                        ship.LaserFired = false;
                    }
                }
            }
            #endregion

            #region Game Restart Process 
            // Game restart currently not implemented
            /*
            else
            {
                ship.Update(gameTime);
                if (ship.Restart == true)
                {
                    // restart game here
                }
            }
            */
            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the game world
        /// </summary>
        /// <param name="gameTime">The game time</param>
        protected override void Draw(GameTime gameTime)
        {
            #region Rendering while player lives
            if (destructionRendered == false)
            {
                GraphicsDevice.Clear(Color.Black);

                // TODO: Add your drawing code here
                spriteBatch.Begin();


                foreach (var asteroid in asteroids)
                {
                    asteroid.Draw(gameTime, spriteBatch);
                }

                ship.Draw(gameTime);
                spriteBatch.DrawString(spriteFont, $"Lives left: {livesLeft}", new Vector2(2, 2), Color.Teal);
                spriteBatch.DrawString(spriteFont, $"Time Survived: {gameTime.TotalGameTime.TotalSeconds.ToString("0s")}", new Vector2(500, 2), Color.Teal);

                laser.Draw(gameTime, spriteBatch);

                spriteBatch.End();

                base.Draw(gameTime);
            }
            #endregion

            //Stops rendering on player death animation
            if (ship.Destroyed == true)
            {
                destructionRendered = true;
            }
        }
    }
}
