using System;

namespace RockRaidersProto
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {

        private static RockRaiderProto _game;

        public static RockRaiderProto RockRaiders
        {
            get
            {
                return _game;
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            _game = new RockRaiderProto();
            _game.Run();
        }
    }
}
