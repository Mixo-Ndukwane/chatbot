using System;
using System.IO;
using System.Media;
using System.Threading;
using System.Xml.Linq;
public class SoundPlayerService // Class responsible for handling audio playback
{
    public void PlayGreeting() // Method to play a greeting sound
    {
        try
        {
            // Build the full path to the audio file (greeting.wav)
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");

            // Check if the audio file exists in the specified location
            if (File.Exists(path))
            {
                // Create a SoundPlayer object using the file path
                SoundPlayer player = new SoundPlayer(path);

                player.Load();     // Load the audio file into memory
                player.PlaySync(); // Play the audio synchronously (wait until it finishes)
            }
            else
            {
                // Display message if the file is not found
                Console.WriteLine("Audio file not found: " + path);
            }
        }
        catch (Exception ex)
        {
            // Handle any errors that occur during audio playback
            Console.WriteLine("Error playing audio: " + ex.Message);
        }
    }
}
