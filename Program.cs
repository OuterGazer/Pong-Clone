using System;

namespace PongGame
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (PongGame game = new PongGame())
            {
                game.Run();
            }
        }
    }
}

