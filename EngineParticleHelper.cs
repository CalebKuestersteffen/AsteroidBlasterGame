using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace AsteroidBlaster
{
    public interface EngineParticleHelper
    {
        public bool EnginesActive { get; }
        public Vector2 ThrusterDirection { get; }
        public Vector2 ThrusterVelocity { get; }
        public Vector2 LeftEnginePosition { get; }
        public Vector2 RightEnginePosition { get; }

    }
}
