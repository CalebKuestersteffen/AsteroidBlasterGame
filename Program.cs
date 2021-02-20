using System;

namespace AsteroidBlaster
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new AsteroidBlaster())
                game.Run();
        }
    }
}
