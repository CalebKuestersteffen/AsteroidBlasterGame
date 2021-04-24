using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsteroidBlaster
{
    public class EngineParticleSystem : ParticleSystem
    {

        EngineParticleHelper _helper;

        public EngineParticleSystem(Game game, EngineParticleHelper helper) : base(game, 4000)
        {
            _helper = helper;
        }

        protected override void InitializeConstants()
        {
            textureFilename = "circle";

            minNumParticles = 10;
            maxNumParticles = 20;

            blendState = BlendState.Additive;
            DrawOrder = AdditiveBlendDrawOrder;
        }

        protected override void InitializeParticle(ref Particle p, Vector2 where)
        {
            var velocity = _helper.ThrusterVelocity;

            var acceleration = Vector2.UnitY * 0;

            var scale = RandomHelper.NextFloat(0.3f, 0.4f);

            var lifetime = RandomHelper.NextFloat(0.01f, 0.06f);

            p.Initialize(where, velocity, acceleration, Color.Teal, scale: scale, lifetime: lifetime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            AddParticles(_helper.LeftEnginePosition);
            AddParticles(_helper.RightEnginePosition);
        }

    }
}
