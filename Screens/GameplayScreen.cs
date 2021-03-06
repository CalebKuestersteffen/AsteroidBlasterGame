﻿using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using AsteroidBlaster.StateManagement;

namespace AsteroidBlaster.Screens
{
    // This screen implements the actual game logic. It is just a
    // placeholder to get the idea across: you'll probably want to
    // put some more interesting gameplay in here!
    public class GameplayScreen : GameScreen, IParticleEmitter, EngineParticleHelper
    {
        private ContentManager _content;
        private SpriteBatch spriteBatch;

        private Texture2D background;
        private Song backgroundMusic;

        private ExplosionParticleSystem _shipExplosion;
        private FireworkParticleSystem _asteroidDestruction;
        private TrailingParticleSystem _shipTrail;

        public EngineParticleSystem LeftEngineParticles { get; set; }
        public EngineParticleSystem RightEngineParticles { get; set; }

        private AsteroidSprite[] asteroids;
        private ShipSprite ship;
        private LaserSprite laser;
        private SpriteFont spriteFont;
        private int livesLeft;
        private float timeSurvived = 0;
        //private int asteroidsShot;
        //private bool destructionRendered = false;
        private SoundEffect asteroidDestruction;
        private SoundEffect shipDestruction;

        private readonly Random _random = new Random();

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }



        /// <summary>
        /// A value that is true if the player is currently moving the ship (engines are on)
        /// </summary>
        public bool EnginesActive { get; set; }
        /// <summary>
        /// The direction that the thrusters are currently facing
        /// </summary>
        public Vector2 ThrusterDirection { get; set; }
        /// <summary>
        /// The velocity of the thrusters
        /// </summary>
        public Vector2 ThrusterVelocity { get; set; }
        /// <summary>
        /// The position of the left engine
        /// </summary>
        public Vector2 LeftEnginePosition { get; set; }
        /// <summary>
        /// The position of the right engine
        /// </summary>
        public Vector2 RightEnginePosition { get; set; }

        public float TimeSurvived
        {
            get
            {
                return timeSurvived;
            }
        }

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back }, true);
        }

        // Load graphics content for the game
        public override void Activate()
        {
            /////SoundEffect.MasterVolume = (float)OptionsMenuScreen.soundEffectVolume;
            System.Random rand = new System.Random();

            // generate asteroids on the top half of the screen
            asteroids = new AsteroidSprite[]
            {
                new AsteroidSprite(ScreenManager.Game, new Vector2((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Width, ((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Height/2) - 300)),
                new AsteroidSprite(ScreenManager.Game, new Vector2((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Width, ((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Height/2) - 300)),
                new AsteroidSprite(ScreenManager.Game, new Vector2((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Width, ((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Height/2) - 300)),
                new AsteroidSprite(ScreenManager.Game, new Vector2((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Width, ((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Height/2) - 300)),
                new AsteroidSprite(ScreenManager.Game, new Vector2((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Width, ((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Height/2) - 300)),
                new AsteroidSprite(ScreenManager.Game, new Vector2((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Width, ((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Height/2) - 300)),
                new AsteroidSprite(ScreenManager.Game, new Vector2((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Width, ((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Height/2) - 300)),
                new AsteroidSprite(ScreenManager.Game, new Vector2((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Width, ((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Height/2) - 300)),
                new AsteroidSprite(ScreenManager.Game, new Vector2((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Width, ((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Height/2) - 300)),
                new AsteroidSprite(ScreenManager.Game, new Vector2((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Width, ((float)rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Height/2) - 300)),
            };


            livesLeft = 3;
            ship = new ShipSprite(ScreenManager.Game);
            laser = new LaserSprite(ScreenManager.Game, ship);


            EnginesActive = ship.EnginesActive;
            ThrusterDirection = ship.ThrusterDirection;
            ThrusterVelocity = ship.ThrusterVelocity;
            LeftEnginePosition = ship.LeftEnginePosition;
            RightEnginePosition = ship.RightEnginePosition;


            spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);

            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            foreach (var asteroid in asteroids) asteroid.LoadContent(_content);
            ship.LoadContent(_content);
            laser.LoadContent(_content);
            background = _content.Load<Texture2D>("NebulousSpace_2560x1440");
            asteroidDestruction = _content.Load<SoundEffect>("Explosion");
            shipDestruction = _content.Load<SoundEffect>("ShipExplosion");
            spriteFont = _content.Load<SpriteFont>("arial");
            backgroundMusic = _content.Load<Song>("BackgroundMusic");

            // A real game would probably have more content than ScreenManager.Game sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            _shipExplosion = new ExplosionParticleSystem(ScreenManager.Game, 20);
            ScreenManager.Game.Components.Add(_shipExplosion);

            _asteroidDestruction = new FireworkParticleSystem(ScreenManager.Game, 20);
            ScreenManager.Game.Components.Add(_asteroidDestruction);

            _shipTrail = new TrailingParticleSystem(ScreenManager.Game, this);
            //ScreenManager.Game.Components.Add(_shipTrail);

            LeftEngineParticles = new EngineParticleSystem(ScreenManager.Game, this);
            ScreenManager.Game.Components.Add(LeftEngineParticles);
            
            RightEngineParticles = new EngineParticleSystem(ScreenManager.Game, this);            
            ScreenManager.Game.Components.Add(RightEngineParticles);

            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;
        }


        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Unload()
        {
            _content.Unload();
        }

        // This method checks the GameScreen.IsActive property, so the game will
        // stop updating when the pause menu is active, or if you tab away to a different application.
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            else
                _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {

                #region Update while player is alive

                // TODO: Add your update logic here
                if (ship.Destroyed == false)
                {
                    ship.Update(gameTime);
                    laser.Update(gameTime);
                    timeSurvived += (float) gameTime.ElapsedGameTime.TotalSeconds;

                    EnginesActive = ship.EnginesActive;
                    ThrusterDirection = ship.ThrusterDirection;
                    ThrusterVelocity = ship.ThrusterVelocity;
                    LeftEnginePosition = ship.LeftEnginePosition;
                    RightEnginePosition = ship.RightEnginePosition;

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
                                _shipExplosion.PlaceExplosion(ship.ShipPosition);
                                ship.Color = Color.White;
                                ship.Destroyed = true;
                                ship.Update(gameTime);

                                if (ship.Destroyed == true) ScreenManager.AddScreen(new GameOverMenuScreen(this), ControllingPlayer);


                                //ScreenManager.Game.Components.Remove(_shipTrail);
                                ScreenManager.Game.Components.Remove(LeftEngineParticles);
                                ScreenManager.Game.Components.Remove(RightEngineParticles);
                            }
                            else
                            {
                                asteroidDestruction.Play();
                                _asteroidDestruction.PlaceFirework(asteroid.Position);
                            }
                        }
                        else if (!asteroid.Hit && asteroid.Bounds.CollidesWith(laser.Bounds))
                        {
                            asteroidDestruction.Play();
                            _asteroidDestruction.PlaceFirework(asteroid.Position);
                            asteroid.Hit = true;
                            ship.LaserFired = false;
                        }
                    }

                    Velocity = ship.ShipPosition - Position;
                    Position = ship.ShipPosition;
                }
                #endregion


            }

            base.Update(gameTime, otherScreenHasFocus, false);
        }

        // Unlike the Update method, ScreenManager.Game will only be called when the gameplay screen is active.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];
            var gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (_pauseAction.Occurred(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(this), ControllingPlayer);
            }
        }

        public override void Draw(GameTime gameTime)
        {

            #region Rendering while player lives
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);


            // Calculate Offset Vector for Background
            float playerX = ship.ShipPosition.X;//MathHelper.Clamp(ship.Position.X, 300, 2560);
            float playerY = ship.ShipPosition.Y;//MathHelper.Clamp(ship.Position.Y, 300, 1440);

            float offsetX = (-800 - playerX);
            float offsetY = (-300 - playerY);

            Matrix transform = Matrix.CreateTranslation(offsetX * 1.0f, offsetY * 1.0f, 0);

            // TODO: Add your drawing code here
            spriteBatch.Begin(transformMatrix: transform);
            //spriteBatch.Begin();
            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.Begin();
            foreach (var asteroid in asteroids)
            {
                asteroid.Draw(gameTime, spriteBatch);
            }

            ship.Draw(gameTime);
            spriteBatch.DrawString(spriteFont, $"Lives left: {livesLeft}", new Vector2(2, 2), Color.Teal);
            spriteBatch.DrawString(spriteFont, $"Time Survived: {timeSurvived.ToString("0s")}", new Vector2(500, 2), Color.Teal);

            laser.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
            #endregion
        }
    }
}
