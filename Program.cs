/* https://twitter.com/HoodStrats || https://github.com/Hoodstrats */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Hood.Core
{
  class Program
  {
    static void Main(string[] args)
    {
      string[] lines = {
        "Spotlight Wallpaper Grabber - Hoodstrats" + "\n" ,
        "You know those cool wallpapers that Windows has on the login screen? Lets Yoink them."+"\n",
        "Press S to check if Spotlight has new Wallpapers and copy them." + "\n" ,
        "Press Q at any time if you want to Quit." + "\n"
      };
      //original console default is 120 x 30
      Console.SetWindowSize(105, 30);
      WriteToConsole(lines[0], ConsoleColor.Yellow, true);
      WriteToConsole(lines[1], ConsoleColor.Yellow, true);
      WriteToConsole(lines[2], ConsoleColor.Green);
      WriteToConsole(lines[3], ConsoleColor.Red);

      SearchFolder();
    }
    static void SearchFolder()
    {
      var pressed = Console.ReadKey(true);
      if (pressed.Key == ConsoleKey.S)
      {
        string genericApp = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string spotLightLocation = @$"{genericApp}\Appdata\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";

        WriteToConsole("\n" + "Searching Windows Spotlight and looking for new wallpapers..." + "\n", ConsoleColor.Green);

        if (Directory.Exists(spotLightLocation))
        {
          CopyFolder(spotLightLocation);
        }
        else
        {
          WriteToConsole("Oops, apparently you don't have Windows spotlight?", ConsoleColor.Red);
        }
      }
      else if (pressed.Key == ConsoleKey.Q)
      {
        Environment.Exit(0);
      }
    }

    private static void CopyFolder(string spotLightLocation)
    {
      //right here make it check the amount of time since last checked for update
      //write this into a text file 
      WriteToConsole("Looks like you have new Wallpapers to copy." + "\n", ConsoleColor.Green);
      //the @ symbol ensures that windows recognizes the "/" symbol no matter the direction
      var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      string dest = @$"{userFolder}\Pictures\Spotlight";

      WriteToConsole(@$"This will create a new folder located at {dest}. Press C to proceed." + "\n", ConsoleColor.Red);
      WriteToConsole(@$"Press Q to quit this application." + "\n", ConsoleColor.Red);

      var pressed = Console.ReadKey(true);

      if (pressed.Key == ConsoleKey.C)
      {
        //creates new folder if it doesn't already exist
        Directory.CreateDirectory(dest);

        //create identifier for each file currently in the folder before doing anything
        //CreateHashes(dest);

        string[] files = Directory.GetFiles(spotLightLocation);

        //copy the files over from the MSOFT directory and attach append jpg to the end
        foreach (string s in files)
        {
          string fileName = Path.GetFileName(s);
          string destFile = Path.Combine(dest, fileName + ".jpg");
          File.Copy(s, destFile, true);
        }
        DeleteGarbage(dest);
      }
      else if (pressed.Key == ConsoleKey.Q)
      {
        Environment.Exit(0);
      }
    }
    //cycle through the current pics and create hashes for each item
    private static void CreateHashes(string dest)
    {
      string hashTable = "hashtable.txt";
      string hashLoc = Path.Combine(dest, hashTable);
      if (!File.Exists(hashLoc))
      {
        File.Create(hashLoc);
      }
      string[] files = Directory.GetFiles(dest);
      foreach (string s in files)
      {
        string hash = Guid.NewGuid().ToString();
        string id = $"{hash},\n";
        File.WriteAllText(hashLoc, id);
      }
    }

    //loop through the files we copied over and check their dimensions, delete the ones that don't meet criteria
    private static void DeleteGarbage(string createdFolder)
    {
      WriteToConsole("Deleting the Garbage..." + "\n", ConsoleColor.Red);

      string[] files = Directory.GetFiles(createdFolder);

      List<string> filesToDelete = new List<string>();
      
      //cycle through files grabbed and filter out the non 1080p
      //there's a lot of different mobile/tablet resolutions to be filtered
      foreach (string s in files)
      {
        using (Image img = Image.FromFile(s))
        {
          var h = img.Height;

          if (h != 1080)
          {
            filesToDelete.Add(s);
          }
        }
      }
      foreach (string s in filesToDelete)
      {
        File.Delete(s);
      }

      RenameFiles(createdFolder);
    }

    //organize the files after we've combed through and rename with date
    private static void RenameFiles(string createdFolder)
    {
      WriteToConsole("Renaming files..." + "\n", ConsoleColor.Red);

      string[] files = Directory.GetFiles(createdFolder);

      int index = 0;

      foreach (string s in files)
      {
        string newName = "Wallpaper" + index + " " + DateTime.Now.ToString("MMM-d-yyyy") + ".jpg";
        string newFile = Path.Combine(createdFolder, newName);

        var named = Path.GetFileNameWithoutExtension(s);

        //if the file already has wallpaper in it that means it's already been renamed skip it
        if (!named.Contains("Wallpaper"))
        {
          File.Move(s, newFile, true);

          var createdFile = Path.GetFileNameWithoutExtension(newFile);

          WriteToConsole(createdFile, ConsoleColor.Red);

          index++;
        }
      }
      WriteToConsole("\n" + "Done enjoy your new Wallpapers." + "\n", ConsoleColor.Green);
      WriteToConsole("Press any key to Quit." + "\n", ConsoleColor.Red);
      Console.ReadKey(true);
      Environment.Exit(0);
    }
    static void WriteToConsole(string line, ConsoleColor color, bool centered = false)
    {
      Console.ForegroundColor = color;

      if (centered)
        Console.SetCursorPosition((Console.WindowWidth - line.Length) / 2, Console.CursorTop);

      Console.WriteLine(line);
      Console.ResetColor();
    }

  }
}
