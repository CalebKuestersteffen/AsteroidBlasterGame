using Microsoft.Xna.Framework;
using AsteroidBlaster.StateManagement;
using Microsoft.Xna.Framework.Graphics;

namespace AsteroidBlaster.Screens
{
    // The main menu screen is the first thing displayed when the game starts up.
    public class MainMenuScreen : MenuScreen
{
        private readonly string _menuControls = "Space to select\nWASD to navigate";
    
        public MainMenuScreen() : base("Asteroid Blaster")
        {
            var playGameMenuEntry = new MenuEntry("Play Game");
            var optionsMenuEntry = new MenuEntry("Options");
            var controlsMenuEntry = new MenuEntry("Controls");
            var exitMenuEntry = new MenuEntry("Exit");

            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            controlsMenuEntry.Selected += ControlsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(controlsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }

        private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        private void ControlsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new ControlsMenuScreen(), e.PlayerIndex);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit the game?";
            var confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var graphics = ScreenManager.GraphicsDevice;
            var spriteBatch = ScreenManager.SpriteBatch;
            var font = ScreenManager.Font;

            var menuControlsPosition = new Vector2(graphics.Viewport.Width / 10, graphics.Viewport.Height - (graphics.Viewport.Height / 20));
            var controlsOrigin = font.MeasureString(_menuControls) / 2;
            var controlsColor = Color.DarkCyan;
            const float menuScale = 0.5f;

            spriteBatch.Begin();
            spriteBatch.DrawString(font, _menuControls, menuControlsPosition, controlsColor, 0, controlsOrigin, menuScale, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
