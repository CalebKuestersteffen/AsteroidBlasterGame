using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace AsteroidBlaster.Screens
{
    // The options screen is brought up over the top of the main menu
    // screen, and gives the user a chance to configure the game
    // in various hopefully useful ways.
    public class OptionsMenuScreen : MenuScreen
    {
        private enum Volume
        {
            oneHundred = 100,
            ninety = 90,
            eighty = 80,
            seventy = 70,
            sixty = 60,
            fifty = 50,
            fourty = 40,
            thirty = 30,
            twenty = 20,
            ten = 10,
            off = 0
        }

        private enum Resolution
        {
            _1080p = 1080,
            _720p = 720,
            _480p = 480
        }


        private readonly MenuEntry _soundEffectVolumeMenuEntry;
        private readonly MenuEntry _musicVolumeMenuEntry;
        private readonly MenuEntry _resolutionMenuEntry;

        private static Volume _currentSoundEffectVolume = Volume.oneHundred;
        private static Volume _currentMusicVolume = Volume.oneHundred;
        private static Resolution _currentResolution = Resolution._480p;

        public OptionsMenuScreen() : base("Options")
        {
            _soundEffectVolumeMenuEntry = new MenuEntry(string.Empty);
            _musicVolumeMenuEntry = new MenuEntry(string.Empty);
            _resolutionMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            var back = new MenuEntry("Back");

            _soundEffectVolumeMenuEntry.Selected += SoundEffectVolumeMenuEntrySelected;
            _musicVolumeMenuEntry.Selected += MusicMenuEntrySelected;
            _resolutionMenuEntry.Selected += ResolutionMenuEntrySelected;
            back.Selected += OnCancel;

            MenuEntries.Add(_soundEffectVolumeMenuEntry);
            MenuEntries.Add(_musicVolumeMenuEntry);
            //MenuEntries.Add(_resolutionMenuEntry);
            MenuEntries.Add(back);
        }

        // Fills in the latest values for the options screen menu text.
        private void SetMenuEntryText()
        {
            _soundEffectVolumeMenuEntry.Text = $"Sound Effect Volume: {(int)_currentSoundEffectVolume}%";
            _musicVolumeMenuEntry.Text = $"Music Volume: {(int)_currentMusicVolume}%";
            _resolutionMenuEntry.Text = $"Resolution: {(int)_currentResolution}p";
        }

        private void SoundEffectVolumeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _currentSoundEffectVolume -= 10;

            if (_currentSoundEffectVolume < Volume.off)
                _currentSoundEffectVolume = Volume.oneHundred;

            SetMenuEntryText();
            UpdateGameSettings();
        }

        private void MusicMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _currentMusicVolume -= 10;

            if (_currentMusicVolume < Volume.off)
                _currentMusicVolume = Volume.oneHundred;

            SetMenuEntryText();
            UpdateGameSettings();
        }

        private void ResolutionMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if(_currentResolution == Resolution._480p)
            {
                _currentResolution = Resolution._720p;
            }
            else if (_currentResolution == Resolution._720p)
            {
                _currentResolution = Resolution._1080p;
            }
            else if(_currentResolution == Resolution._1080p)
            {
                _currentResolution = Resolution._480p;
            }

            SetMenuEntryText();
            UpdateGameSettings();
        }

        private void UpdateGameSettings()
        {
            SoundEffect.MasterVolume = (float)((float)_currentSoundEffectVolume/100);
            MediaPlayer.Volume = (float)((float)_currentMusicVolume/100);
            

        }

    }
}
