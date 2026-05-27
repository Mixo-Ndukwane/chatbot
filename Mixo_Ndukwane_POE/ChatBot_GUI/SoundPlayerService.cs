using System;
using System.IO;
using System.Media;

namespace ChatBot_GUI
{
    public class SoundPlayerService
    {
        public void PlayGreeting()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");
                if (File.Exists(path))
                {
                    var player = new SoundPlayer(path);
                    player.Load();
                    player.PlaySync();
                }
            }
            catch
            {
                // Silently ignore audio errors so the UI still loads
            }
        }
    }
}
