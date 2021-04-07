using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsteroidBlaster
{
    class FireworkParticleSystem : ParticleSystem
    {
        Color[] colors = new Color[]
        {
            /*
            Color.Teal,
            Color.CadetBlue,
            Color.Aqua,
            Color.Turquoise,
            Color.DarkTurquoise,
            Color.DarkCyan
            */
            Color.White,
            Color.LightGray,
            Color.Gray,
            Color.DarkGray,
            Color.Teal,
            Color.SlateGray
        };

        Color color;

        public FireworkParticleSystem(Game game, int maxExplosions) : base(game, maxExplosions * 25) { }

        protected override void InitializeConstants()
        {
            textureFilename = "particle";

            minNumParticles = 100;
            maxNumParticles = 150;

            blendState = BlendState.Additive;
            DrawOrder = AdditiveBlendDrawOrder;
        }

        protected override void InitializeParticle(ref Particle p, Vector2 where)
        {
            var velocity = RandomHelper.NextDirection() * RandomHelper.NextFloat(40, 200);

            var lifetime = RandomHelper.NextFloat(0.5f, 1.0f);

            var acceleration = -velocity / lifetime;

            var rotation = RandomHelper.NextFloat(0, MathHelper.TwoPi);

            var angularVelocity = RandomHelper.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);

            var scale = RandomHelper.NextFloat(30, 50);

            p.Initialize(where, velocity, acceleration, color, lifetime, rotation: rotation, angularVelocity: angularVelocity, scale: scale);
        }


        protected override void UpdateParticle(ref Particle particle, float dt)
        {
            base.UpdateParticle(ref particle, dt);

            float normalizedLifetime = particle.TimeSinceStart / particle.Lifetime;

            particle.Scale = 0.1f + 0.25f * normalizedLifetime;
        }


        public void PlaceFirework(Vector2 where)
        {
            color = colors[RandomHelper.Next(colors.Length)];
            AddParticles(where);
        }

    }
}