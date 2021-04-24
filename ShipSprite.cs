using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using AsteroidBlaster.Collisions;

namespace AsteroidBlaster
{
    public enum Direction
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }

    /// <summary>
    /// A class representing a ship
    /// </summary>
    public class ShipSprite : EngineParticleHelper
    {
        const short SHIP_SPEED = 500;
        const float SPEED_CAP = 300;

        /// <summary>
        /// The game instance
        /// </summary>
        public Game game;

        private SpriteBatch spriteBatch;

        private GamePadState gamePadState;

        private KeyboardState priorKeyboardState;
        private KeyboardState currentKeyboardState;

        private Texture2D texture;
        //private Texture2D hitboxDebug;

        private Vector2 shipPosition;
        private Vector2 shipVelocity;

        private float accelerationRatio = 2.0f;

        private BoundingRectangle bounds = new BoundingRectangle(new Vector2(400 - 41, 440 - 20), 82, 40);
        private BoundingRectangle rotatedBounds = new BoundingRectangle(new Vector2(400 - 20, 440 - 41), 40, 82);

        private short animationFrame = 0;

        private bool rotated = false;

        private SoundEffect laserShooting;
        private SoundEffect laserCharge;


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

        /// <summary>
        /// The current charge time of the laser
        /// </summary>
        public double chargeTime { get; set; } = 0;

        /// <summary>
        /// The recharge delay of the laser
        /// </summary>
        public double rechargeDelay { get; set; } = 2;

        /// <summary>
        /// The status of the ships laser systems
        /// </summary>
        public bool ShotCharged { get; private set; } = false;

        /// <summary>
        /// The status of the laser weapon
        /// </summary>
        public bool LaserFired { get; set; } = false;

        /// <summary>
        /// The direction the ship is facing
        /// </summary>
        public Direction Direction;

        /// <summary>
        /// The status of the ship
        /// </summary>
        public bool Destroyed { get; set; } = false;

        /// <summary>
        /// The position of the ship sprite
        /// </summary>
        public Vector2 ShipPosition {
            get
            {
                return shipPosition;
            }
        }

        /// <summary>
        /// The bounding volume of the sprite
        /// </summary>
        public BoundingRectangle Bounds
        {
            get
            {
                if (rotated) return rotatedBounds;
                else return bounds;
            }
        }

        /// <summary>
        /// The color to blend with the ship
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Creates the ship sprite
        /// </summary>
        /// <param name="game"></param>
        public ShipSprite(Game game)
        {
            this.game = game;
            this.spriteBatch = new SpriteBatch(this.game.GraphicsDevice);
            this.shipPosition = new Vector2(400, 440);
            this.Direction = Direction.Up;

            this.ThrusterDirection = new Vector2(0, 1);// starts facing down
            this.ThrusterVelocity = new Vector2(100, 100);
            this.LeftEnginePosition = shipPosition - new Vector2(-15, 14);
            this.RightEnginePosition = shipPosition - new Vector2(15, 14);
        }

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("SpaceGameAtlas");
            laserCharge = content.Load<SoundEffect>("LaserCharge");
            laserShooting = content.Load<SoundEffect>("LaserShooting");
            //hitboxDebug = content.Load<Texture2D>("HitBoxDebug");
        }

        /// <summary>
        /// Updates the sprite's position based on user input
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        public void Update(GameTime gameTime)
        {
            gamePadState = GamePad.GetState(0);

            priorKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 acceleration = new Vector2(0, 0);
            var viewport = game.GraphicsDevice.Viewport;
            float windowRatio = ((float)viewport.Height / (float)viewport.Width);

            #region Gamepad Input
            // Apply the gamepad movement with inverted Y axis

            /*
            position += gamePadState.ThumbSticks.Left * new Vector2(1, -1);
            if (gamePadState.ThumbSticks.Left.X < 0) flipped = true;
            if (gamePadState.ThumbSticks.Left.X > 0) flipped = false;
            */

            #endregion

            #region Keyboard Input

            if (Destroyed == false)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W))
                {
                    acceleration += -Vector2.UnitY * (SHIP_SPEED * windowRatio);
                    rotated = false;
                    Direction = Direction.Up;
                    EnginesActive = true;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.S))
                {
                    acceleration += Vector2.UnitY * (SHIP_SPEED * windowRatio);
                    rotated = false;
                    Direction = Direction.Down;
                    EnginesActive = true;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Left) || currentKeyboardState.IsKeyDown(Keys.A))
                {
                    acceleration += -Vector2.UnitX * SHIP_SPEED;
                    rotated = true;
                    Direction = Direction.Left;
                    EnginesActive = true;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Right) || currentKeyboardState.IsKeyDown(Keys.D))
                {
                    acceleration += Vector2.UnitX * SHIP_SPEED;
                    rotated = true;
                    Direction = Direction.Right;
                    EnginesActive = true;
                }

                // update position of the ship
                shipVelocity += acceleration * accelerationRatio * t;

                #region Speed Capping
                if (shipVelocity.X > SPEED_CAP)
                {
                    shipVelocity.X = SPEED_CAP;
                }
                else if (shipVelocity.X < -SPEED_CAP)
                {
                    shipVelocity.X = -SPEED_CAP;
                }
                if (shipVelocity.Y > (SPEED_CAP * windowRatio))
                {
                    shipVelocity.Y = (SPEED_CAP * windowRatio);
                }
                else if (shipVelocity.Y < -(SPEED_CAP * windowRatio))
                {
                    shipVelocity.Y = -(SPEED_CAP * windowRatio);
                }
                #endregion

                #region Update Ship Direction
                //ship is moving either up or down
                if (Math.Abs(shipVelocity.Y) > Math.Abs(shipVelocity.X))
                {
                    if(shipVelocity.Y < 0)
                    {
                        Direction = Direction.Up;
                    }
                    else
                    {
                        Direction = Direction.Down;
                    }
                }
                //ship is moving either left or right
                else if (Math.Abs(shipVelocity.X) > Math.Abs(shipVelocity.Y))
                {
                    if (shipVelocity.X < 0)
                    {
                        Direction = Direction.Left;
                    }
                    else
                    {
                        Direction = Direction.Right;
                    }
                }
                #endregion

                shipPosition += shipVelocity * t;

                #region Game Bounds
                // keep the ship within the game window
                if (shipPosition.X > viewport.Width - 20) shipPosition.X = viewport.Width - 20;
                else if (shipPosition.X < 20) shipPosition.X = 20;
                if (shipPosition.Y > viewport.Height - 20) shipPosition.Y = viewport.Height - 20;
                else if (shipPosition.Y < 20) shipPosition.Y = 20;
                #endregion
            }
            
            #endregion

            #region Laser firing input

            // Laser firing input (mapped to spacebar)
            if (priorKeyboardState.IsKeyUp(Keys.Space) && 
                currentKeyboardState.IsKeyDown(Keys.Space) && ShotCharged == true)
            {
                ShotCharged = false;
                LaserFired = true;
                laserShooting.Play();
            }

            if (ShotCharged == false)
            {
                if (chargeTime > rechargeDelay)
                {
                    chargeTime = 0;
                    ShotCharged = true;
                    LaserFired = false;
                }
                else
                {
                    if (chargeTime > 0.3 && chargeTime < 0.4)
                    {
                        laserCharge.Play(0.1f, 0, 0);
                    }
                    chargeTime += gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            #endregion

            #region Update bounds

            bounds.X = shipPosition.X - 41;
            bounds.Y = shipPosition.Y - 20;
            rotatedBounds.X = shipPosition.X - 20;
            rotatedBounds.Y = shipPosition.Y - 41;

            #endregion

            #region Player Death

            if (Destroyed)
            {
                animationFrame = 1;
            }
            else
            {
                animationFrame = 0;
            }

            #endregion

            #region Engine Particles

            // Update the thruster direction and positions using the new ship rotation
            switch (Direction)
            {
                case Direction.Up:
                    ThrusterDirection = new Vector2(0, 1);
                    LeftEnginePosition = shipPosition - new Vector2(15, -14);
                    RightEnginePosition = shipPosition - new Vector2(-15, -14);
                    ThrusterVelocity = (new Vector2(100, shipVelocity.Y + 100) * ThrusterDirection) * (1 / windowRatio);
                    break;
                case Direction.Down:
                    ThrusterDirection = new Vector2(0, -1);
                    LeftEnginePosition = shipPosition - new Vector2(-15, 14);
                    RightEnginePosition = shipPosition - new Vector2(15, 14);
                    ThrusterVelocity = (new Vector2(100, shipVelocity.Y - 100) * ThrusterDirection) * (1 / windowRatio);;
                    break;
                case Direction.Left:
                    ThrusterDirection = new Vector2(1, 0);
                    LeftEnginePosition = shipPosition - new Vector2(-14, -15);
                    RightEnginePosition = shipPosition - new Vector2(-14, 15);
                    ThrusterVelocity = (new Vector2(shipVelocity.X + 100, 100) * ThrusterDirection);
                    break;
                case Direction.Right:
                    ThrusterDirection = new Vector2(-1, 0);
                    LeftEnginePosition = shipPosition - new Vector2(14, 15);
                    RightEnginePosition = shipPosition - new Vector2(14, -15);
                    ThrusterVelocity = (new Vector2(shipVelocity.X - 100, 100) * ThrusterDirection);
                    break;

            }

            // Change Thrust Direction
            ThrusterVelocity = (new Vector2(100,100) * ThrusterDirection);

            // Reset the engine state
            EnginesActive = false;

            #endregion
        }

        /// <summary>
        /// Draws the sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime)
        {
            // flip effect uneccessary with current movement mechanics because of ship symmetry
            SpriteEffects spriteEffects = SpriteEffects.None; //(flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            var source = new Rectangle(36, animationFrame * 80, 164, 80);// sprite is 164x80 pixels at 1f scale

            spriteBatch.Begin();

            #region Debug
            /*
            var debug = new Rectangle(36, animationFrame * 80, 164, 80);
            spriteBatch.Draw(hitboxDebug, position + new Vector2(19, 0), debug, Color, 0f, new Vector2(120, 40), 0.5f, spriteEffects, 0);
            */
            #endregion

            switch (Direction)
            {
                case Direction.Up:
                    spriteBatch.Draw(texture, shipPosition, source, Color, (float)Math.PI * 0f, new Vector2(82, 40), 0.5f, spriteEffects, 0);
                    break;
                case Direction.Down:
                    spriteBatch.Draw(texture, shipPosition, source, Color, (float)Math.PI * 1f, new Vector2(82, 40), 0.5f, spriteEffects, 0);
                    break;
                case Direction.Left:
                    spriteBatch.Draw(texture, shipPosition, source, Color, (float)Math.PI * 1.5f, new Vector2(82, 40), 0.5f, spriteEffects, 0);
                    break;
                case Direction.Right:
                    spriteBatch.Draw(texture, shipPosition, source, Color, (float)Math.PI * 0.5f, new Vector2(82, 40), 0.5f, spriteEffects, 0);
                    break;
            }

            spriteBatch.End();
        }
    }
}
