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
    public class ShipSprite
    {
        const short SHIP_SPEED = 500;
        const float SPEED_CAP = 300;

        /// <summary>
        /// The game instance
        /// </summary>
        public Game game;

        private GamePadState gamePadState;

        private KeyboardState priorKeyboardState;
        private KeyboardState currentKeyboardState;

        private Texture2D texture;
        //private Texture2D hitboxDebug;

        private Vector2 position;
        private Vector2 velocity;

        private BoundingRectangle bounds = new BoundingRectangle(new Vector2(400 - 41, 440 - 20), 82, 40);
        private BoundingRectangle rotatedBounds = new BoundingRectangle(new Vector2(400 - 20, 440 - 41), 40, 82);

        private short animationFrame = 0;

        private bool rotated = false;

        private SoundEffect laserShooting;
        private SoundEffect laserCharge;

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
        /// If the user has selected to restart the game
        /// </summary>
        public bool Restart = false;

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
        public Vector2 Position {
            get
            {
                return position;
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
            this.position = new Vector2(400, 440);
            this.Direction = Direction.Up;
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
            // Apply keyboard movement

            if (Destroyed == false)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W))
                {
                    acceleration += -Vector2.UnitY * (SHIP_SPEED * windowRatio);
                    rotated = false;
                    Direction = Direction.Up;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.S))
                {
                    acceleration += Vector2.UnitY * (SHIP_SPEED * windowRatio);
                    rotated = false;
                    Direction = Direction.Down;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Left) || currentKeyboardState.IsKeyDown(Keys.A))
                {
                    acceleration += -Vector2.UnitX * SHIP_SPEED;
                    rotated = true;
                    Direction = Direction.Left;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Right) || currentKeyboardState.IsKeyDown(Keys.D))
                {
                    acceleration += Vector2.UnitX * SHIP_SPEED;
                    rotated = true;
                    Direction = Direction.Right;
                }

                // update position of the ship
                velocity += acceleration * t;

                #region Speed Capping
                if (velocity.X > SPEED_CAP)
                {
                    velocity.X = SPEED_CAP;
                }
                else if (velocity.X < -SPEED_CAP)
                {
                    velocity.X = -SPEED_CAP;
                }
                if (velocity.Y > (SPEED_CAP * windowRatio))
                {
                    velocity.Y = (SPEED_CAP * windowRatio);
                }
                else if (velocity.Y < -(SPEED_CAP * windowRatio))
                {
                    velocity.Y = -(SPEED_CAP * windowRatio);
                }
                #endregion

                position += velocity * t;

                // keep the ship within the game window
                if (position.X > viewport.Width - 20) position.X = viewport.Width - 20;
                else if (position.X < 20) position.X = 20;
                if (position.Y > viewport.Height - 20) position.Y = viewport.Height - 20;
                else if (position.Y < 20) position.Y = 20;
            }
            else
            {
                if (currentKeyboardState.IsKeyDown(Keys.R) &&
                    priorKeyboardState.IsKeyUp(Keys.R))
                {
                    Restart = true;
                }
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

            bounds.X = position.X - 41;
            bounds.Y = position.Y - 20;
            rotatedBounds.X = position.X - 20;
            rotatedBounds.Y = position.Y - 41;

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

        }

        /// <summary>
        /// Draws the sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // flip effect uneccessary with current movement mechanics because of ship symmetry
            SpriteEffects spriteEffects = SpriteEffects.None; //(flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            var source = new Rectangle(36, animationFrame * 80, 164, 80);// sprite is 164x80 pixels at 1f scale

            /*
            var debug = new Rectangle(36, animationFrame * 80, 164, 80);
            spriteBatch.Draw(hitboxDebug, position + new Vector2(19, 0), debug, Color, 0f, new Vector2(120, 40), 0.5f, spriteEffects, 0);
            */

            switch (Direction)
            {
                case Direction.Up:
                    spriteBatch.Draw(texture, position, source, Color, (float)Math.PI * 0f, new Vector2(82, 40), 0.5f, spriteEffects, 0);
                    break;
                case Direction.Down:
                    spriteBatch.Draw(texture, position, source, Color, (float)Math.PI * 1f, new Vector2(82, 40), 0.5f, spriteEffects, 0);
                    break;
                case Direction.Left:
                    spriteBatch.Draw(texture, position, source, Color, (float)Math.PI * 1.5f, new Vector2(82, 40), 0.5f, spriteEffects, 0);
                    break;
                case Direction.Right:
                    spriteBatch.Draw(texture, position, source, Color, (float)Math.PI * 0.5f, new Vector2(82, 40), 0.5f, spriteEffects, 0);
                    break;
            }

        }
    }
}
