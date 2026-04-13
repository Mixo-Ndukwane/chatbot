
using System;
using System.IO;
using System.Media;
using System.Threading;
using System.Xml.Linq;
public class SoundPlayerService
{
    public void PlayGreeting()
    {

        try
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");

            if (File.Exists(path))
            {
                SoundPlayer player = new SoundPlayer(path);
                player.Load();
                player.PlaySync();
            }
            else
            {
                Console.WriteLine("Audio file not found: " + path);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error playing audio: " + ex.Message);
        }
    }
}