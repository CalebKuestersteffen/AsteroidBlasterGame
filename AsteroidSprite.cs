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

    public class AsteroidSprite
    {

        //private const float ANIMATION_SPEED = 0.1f;
        //private double animationTimer;

        System.Random rand = new System.Random();

        private int animationFrame = 1;
        private Vector2 position;
        private Vector2 velocity;
        private Texture2D texture;
        private Texture2D hitboxDebug;
        private BoundingCircle bounds;
        private short driftSpeed = 75;
        private float asteroidSpeed = 175f;
        private short delay = 0;
        private short delayMax = 1;
        private short respawnTimer = 0;
        private short respawnDelay = 200;
        private float driftDirection = 0;
        private Game game;

        public Vector2 Position
        {
            get { return position;  }
        }

        /// <summary>
        /// Tracks if the asteroid has been hit
        /// </summary>
        public bool Hit { get; set; } = false;

        /// <summary>
        /// The bounding volume of the sprite
        /// </summary>
        public BoundingCircle Bounds => bounds;

        /// <summary>
        /// Creates a new asteroid sprite
        /// </summary>
        /// <param name="position">The position of the sprite in the game</param>
        public AsteroidSprite(Game game, Vector2 position)
        {
            this.game = game;
            this.position = position;
            this.bounds = new BoundingCircle(position + new Vector2(20, 20), 20);
            driftDirection = RandomHelper.NextFloat(-1,1);
        }

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("SpaceGameAtlas");
            hitboxDebug = content.Load<Texture2D>("HitBoxDebug");
        }

        /// <summary>
        /// Updates the asteroids position and bounds
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            var viewport = game.GraphicsDevice.Viewport;
            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

            velocity = new Vector2(-driftSpeed*driftDirection, asteroidSpeed);
            delay = 0;

            position += velocity * t;

            if (position.Y > viewport.Height)
            {
                driftDirection = RandomHelper.NextFloat(-1, 1);
                position.Y = 0;
            }
            if (position.X < 0)
            {
                driftDirection = RandomHelper.NextFloat(-1, 1);
                position.X = viewport.Width;
            }
            if (position.X > viewport.Width)
            {
                driftDirection = RandomHelper.NextFloat(-1, 1);
                position.X = 0;
            }

            bounds.Center = position + new Vector2(20, 20);

            if (Hit)
            {
                respawnTimer++;
                if (respawnTimer > respawnDelay)
                {
                    Hit = false;
                    position = (new Vector2((float)rand.NextDouble() * viewport.Width, ((float)rand.NextDouble() * viewport.Height / 8) - 100));
                    bounds.Center = position + new Vector2(20, 20);
                }
            }
        }

        /// <summary>
        /// Draws the animated sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Hit) return;

            #region Animation Code
            /*
            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if(animationTimer > ANIMATION_SPEED)
            {
                animationFrame++;
                if (animationFrame > 7) animationFrame = 0;
                animationTimer -= ANIMATION_SPEED;
            }
            */

            /*
            var debug = new Rectangle(80, 80 + animationFrame * 80, 80, 80);
            spriteBatch.Draw(hitboxDebug, position + new Vector2(0, 0), debug, Color.White, 0f, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            */

            #endregion

            var source = new Rectangle(80, 80 + animationFrame * 80, 80, 80);

            spriteBatch.Draw(texture, position, source, Color.White, 0f, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
        }
    }
}
