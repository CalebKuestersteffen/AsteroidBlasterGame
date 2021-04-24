using AsteroidBlaster.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AsteroidBlaster.Screens
{
    // The pause menu comes up over the top of the game,
    // giving the player options to resume or quit.
    public class GameOverMenuScreen : MenuScreen
    {
        private GameplayScreen gameplayScreen;
        TimeSpan newTimeSpan;
        TimeSpan oldTimeSpan;
        private string _upperText = "";
        private string _lowerText = "";

        bool updated = false;

        public GameOverMenuScreen(GameplayScreen gameScreen) : base("Game Over")
        {
            gameplayScreen = gameScreen;

            var restartGameMenuEntry = new MenuEntry("Restart Game");
            var quitGameMenuEntry = new MenuEntry("Quit Game");

            restartGameMenuEntry.Selected += RestartGameMenuEntrySelected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            MenuEntries.Add(restartGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }

        private void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to quit this game?";
            var confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }

        // This uses the loading screen to transition from the game back to the main menu screen.
        private void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
        }

        private void RestartGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to restart this game?";
            var confirmRestartMessageBox = new MessageBoxScreen(message);

            confirmRestartMessageBox.Accepted += ConfirmRestartMessageBoxAccepted;

            ScreenManager.AddScreen(confirmRestartMessageBox, ControllingPlayer);
        }

        private void ConfirmRestartMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            updated = false;
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }


        public override void Draw(GameTime gameTime)
        {
            newTimeSpan = TimeSpan.FromSeconds(gameplayScreen.TimeSurvived);
            oldTimeSpan = TimeSpan.FromSeconds(gameplayScreen.ScreenManager.SurvivalRecord);

            string newTime = newTimeSpan.ToString(@"mm\:ss");
            string bestTime = oldTimeSpan.ToString(@"mm\:ss");

            if (gameplayScreen.TimeSurvived > gameplayScreen.ScreenManager.SurvivalRecord && updated == false)
            {
                _upperText = "New Record!";
                _lowerText = $"You survived for {newTime}";
                gameplayScreen.ScreenManager.SurvivalRecord = gameplayScreen.TimeSurvived;
                updated = true;
            }
            else if (updated == false)
            {
                 _upperText = $"You survived for {newTime}";
                _lowerText = $"Your current record is {bestTime}";
            }







            base.Draw(gameTime);

            var graphics = ScreenManager.GraphicsDevice;
            var spriteBatch = ScreenManager.SpriteBatch;
            var font = ScreenManager.Font;

            var upperLinePosition = new Vector2(graphics.Viewport.Width / 2, graphics.Viewport.Height - (77 * graphics.Viewport.Height / 100));
            var upperLineOrigin = font.MeasureString(_upperText) / 2;

            var lowerLinePosition = new Vector2(graphics.Viewport.Width / 2, graphics.Viewport.Height - (70 * graphics.Viewport.Height / 100));
            var lowerLineOrigin = font.MeasureString(_lowerText) / 2;

            var textColor = Color.Cyan;
            const float textScale = 0.75f;

            spriteBatch.Begin();
            spriteBatch.DrawString(font, _upperText,upperLinePosition, textColor, 0, upperLineOrigin, textScale, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, _lowerText, lowerLinePosition, textColor, 0, lowerLineOrigin, textScale, SpriteEffects.None, 0);
            spriteBatch.End();
        }


    }
}
