﻿using System;

namespace GSMXtended.Demo {
    public static class Program {
        [STAThread]
        static void Main() {
            using (var game = new ExampleGame())
                game.Run();
        }
    }
}
