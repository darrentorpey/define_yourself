using System;

namespace DefineYourself
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ClientGame game = new ClientGame())
            {
                game.Run();
            }
        }
    }
}

