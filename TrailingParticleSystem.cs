using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsteroidBlaster
{
    class TrailingParticleSystem : ParticleSystem
    {

        IParticleEmitter _emitter;

        public TrailingParticleSystem(Game game, IParticleEmitter emitter) : base(game, 2000)
        {
            _emitter = emitter;
        }


        protected override void InitializeConstants()
        {
            textureFilename = "circle";

            minNumParticles = 2;
            maxNumParticles = 5;

            blendState = BlendState.Additive;
            DrawOrder = AdditiveBlendDrawOrder;
        }

        protected override void InitializeParticle(ref Particle p, Vector2 where)
        {
            var velocity = _emitter.Velocity;

            var acceleration = Vector2.UnitY * 0;

            var scale = RandomHelper.NextFloat(0.2f, 0.5f);

            var lifetime = RandomHelper.NextFloat(0.1f, 0.3f);

            p.Initialize(where, velocity, acceleration, Color.Teal, scale: scale, lifetime: lifetime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            AddParticles(_emitter.Position);
        }

    }
}
