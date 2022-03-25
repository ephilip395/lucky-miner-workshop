﻿using System.Windows;

namespace Lucky.Controls {
    public class KbSystemMinButton : KbSystemButton {
        Window window;
        public KbSystemMinButton() {
            Click += delegate {
                if (window == null) {
                    window = Window.GetWindow(this);
                }
                Microsoft.Windows.Shell.SystemCommands.MinimizeWindow(window);
            };
        }
    }
}
