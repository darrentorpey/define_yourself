using System;

namespace IntroGame
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (IntroGame game = new IntroGame())
            {
                game.Run();
            }
        }
    }
}

