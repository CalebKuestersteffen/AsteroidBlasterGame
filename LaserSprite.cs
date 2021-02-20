using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using AsteroidBlaster.Collisions;

namespace AsteroidBlaster
{
    class LaserSprite
    {
        const int LASER_SPEED = 750;

        private Vector2 position;
        private Vector2 velocity;

        private Direction direction;
        private Direction firingDirection = Direction.Up;

        private Texture2D texture;

        private bool rotated = false;

        private int animationFrame = 0;

        private bool recharged = false;

        private bool shipDestroyed = false;

        private BoundingRectangle bounds;
        private BoundingRectangle rotatedBounds;

        private Game game;

        private ShipSprite ship;

        /// <summary>
        /// The firing state of the laser
        /// </summary>
        public bool fired = false;

        /// <summary>
        /// The state of the laser
        /// </summary>
        public bool active;

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
        /// Creates the Laser sprite
        /// </summary>
        /// <param name="game"> The game instance </param>
        /// <param name="ship"> The ship </param>
        public LaserSprite(Game game, ShipSprite ship)
        {
            this.game = game;
            this.ship = ship;
            this.position = ship.Position + new Vector2(0, -10);
            this.fired = ship.LaserFired;
            this.shipDestroyed = ship.Destroyed;
            this.bounds = new BoundingRectangle(position, 39, 10);
            this.rotatedBounds = new BoundingRectangle(position, 10, 39);
        }

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("SpaceGameAtlas");
        }

        /// <summary>
        /// Updates the sprite's position and bounds
        /// </summary>
        /// <param name="gameTime"> The GameTime </param>
        public void Update(GameTime gameTime)
        {
            if (shipDestroyed == false)
            {
                float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

                var viewport = game.GraphicsDevice.Viewport;
                float windowRatio = ((float)viewport.Height / (float)viewport.Width);

                direction = ship.Direction;
                fired = ship.LaserFired;
                recharged = ship.ShotCharged;
                shipDestroyed = ship.Destroyed;

                #region Laser firing logic
                // the laser is firing
                if (fired == true && active == false)
                {
                    active = true;
                    if (direction == Direction.Up)
                    {
                        velocity = -Vector2.UnitY * (LASER_SPEED * windowRatio);
                        rotated = false;
                        firingDirection = Direction.Up;
                    }
                    if (direction == Direction.Down)
                    {
                        velocity = Vector2.UnitY * (LASER_SPEED * windowRatio);
                        rotated = false;
                        firingDirection = Direction.Down;
                    }
                    if (direction == Direction.Left)
                    {
                        velocity = -Vector2.UnitX * LASER_SPEED;
                        rotated = true;
                        firingDirection = Direction.Left;
                    }
                    if (direction == Direction.Right)
                    {
                        velocity = Vector2.UnitX * LASER_SPEED;
                        rotated = true;
                        firingDirection = Direction.Right;
                    }
                    #endregion
                }
                // the laser is in the charging animation
                if (recharged == false && active == false)
                {
                    // integer division should handle decimal truncation
                    animationFrame = (int)(ship.chargeTime / (ship.rechargeDelay * 0.1));
                }
                // the laser has collided with an asteroid
                if (fired == false && active == true)
                {
                    active = false;
                }
                #region Update position and bounds
                // update position of the laser
                if (active == true && fired == true) {
                    position += velocity * t;
                }
                // register when the laser is not on the screen
                if ((position.X > viewport.Width - 5) || (position.X < 5) || (position.Y > viewport.Height - 5) || (position.Y < 5))
                {
                    active = false;
                    fired = false;
                    ship.LaserFired = false;
                }
                if (active == false && fired == false)
                {
                    switch (direction)
                    {
                        case Direction.Up:
                            position = ship.Position + new Vector2(0, -10 - (animationFrame / 2));
                            break;
                        case Direction.Down:
                            position = ship.Position + new Vector2(0, 10 + (animationFrame / 2));
                            break;
                        case Direction.Left:
                            position = ship.Position + new Vector2(-10 - (animationFrame / 2), 0);
                            break;
                        case Direction.Right:
                            position = ship.Position + new Vector2(10 + (animationFrame / 2), 0);
                            break;
                    }
                }

                // update the bounds of the laser
                bounds.X = position.X - 19;
                bounds.Y = position.Y - 5;
                rotatedBounds.X = position.X - 5;
                rotatedBounds.Y = position.Y - 19;
                #endregion
            }

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (shipDestroyed == false)
            {
                SpriteEffects spriteEffects = SpriteEffects.None; //(flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                var source = new Rectangle(0, 187, 79, (animationFrame * 2));// sprite is 79x20 pixels at 1f scale. 

                if (active == false)
                {
                    switch (direction)
                    {
                        case Direction.Up:
                            spriteBatch.Draw(texture, position, source, Color.White, (float)Math.PI * 0f, new Vector2(39, 10), 0.5f, spriteEffects, 0);
                            break;
                        case Direction.Down:
                            spriteBatch.Draw(texture, position, source, Color.White, (float)Math.PI * 1f, new Vector2(39, 10), 0.5f, spriteEffects, 0);
                            break;
                        case Direction.Left:
                            spriteBatch.Draw(texture, position, source, Color.White, (float)Math.PI * 1.5f, new Vector2(39, 10), 0.5f, spriteEffects, 0);
                            break;
                        case Direction.Right:
                            spriteBatch.Draw(texture, position, source, Color.White, (float)Math.PI * 0.5f, new Vector2(39, 10), 0.5f, spriteEffects, 0);
                            break;
                    }
                }
                else
                {
                    switch (firingDirection)
                    {
                        case Direction.Up:
                            spriteBatch.Draw(texture, position, source, Color.White, (float)Math.PI * 0f, new Vector2(39, 10), 0.5f, spriteEffects, 0);
                            break;
                        case Direction.Down:
                            spriteBatch.Draw(texture, position, source, Color.White, (float)Math.PI * 1f, new Vector2(39, 10), 0.5f, spriteEffects, 0);
                            break;
                        case Direction.Left:
                            spriteBatch.Draw(texture, position, source, Color.White, (float)Math.PI * 1.5f, new Vector2(39, 10), 0.5f, spriteEffects, 0);
                            break;
                        case Direction.Right:
                            spriteBatch.Draw(texture, position, source, Color.White, (float)Math.PI * 0.5f, new Vector2(39, 10), 0.5f, spriteEffects, 0);
                            break;
                    }
                }

            }
        }


    }

}
