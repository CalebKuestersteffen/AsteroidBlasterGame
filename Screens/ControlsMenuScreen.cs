using System;
using System.Collections.Generic;
using System.Text;

namespace AsteroidBlaster.Screens
{
    class ControlsMenuScreen : MenuScreen
    {
        private readonly MenuEntry _movementMenuEntry;
        private readonly MenuEntry _firingMenuEntry;
        private readonly MenuEntry _pauseMenuEntry;

        public ControlsMenuScreen() : base("Controls")
        {
            _movementMenuEntry = new MenuEntry("Movement: wasd");
            _firingMenuEntry = new MenuEntry("Firing: space bar");
            _pauseMenuEntry = new MenuEntry("Pause: backspace");

            var back = new MenuEntry("Back");

            back.Selected += OnCancel;

            MenuEntries.Add(_movementMenuEntry);
            MenuEntries.Add(_firingMenuEntry);
            MenuEntries.Add(_pauseMenuEntry);
            MenuEntries.Add(back);
        }

    }
}
