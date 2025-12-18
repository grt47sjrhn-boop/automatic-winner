using System;

namespace substrate_shared.Utils
{
    public static class ConsoleHelper
    {
        // Toggle this to enable/disable pauses globally
        public static bool AutoPause { get; set; } = true;

        public static void Pause()
        {
            if (AutoPause)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
}