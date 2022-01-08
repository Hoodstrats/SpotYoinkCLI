/* https://twitter.com/HoodStrats || https://github.com/Hoodstrats */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Hood.Core
{
  class Program
  {
    public static List<string> _filesInFolder;

    static void Main(string[] args)
    {
      Version ver = Assembly.GetExecutingAssembly().GetName().Version;
      string subVer = ver.ToString().Substring(0, 5);
      string[] lines = {
        $"Spotlight Wallpaper Grabber {subVer} - Hoodstrats\n",
        "This tool will Grab and Convert the Windows login Wallpapers.\n",
        "Press S to check if Spotlight has new Wallpapers and copy them.\n" ,
        "Press Q at any time if you want to Quit.\n"
      };
      //original console default is 120 x 30
      Console.SetWindowSize(105, 30);
      WriteToConsole(lines[0], ConsoleColor.Cyan, true);
      WriteToConsole(lines[1], ConsoleColor.Cyan, true);
      WriteToConsole(lines[2], ConsoleColor.Green);
      WriteToConsole(lines[3], ConsoleColor.Red);

      CheckForWindowsSpotlight();
    }
    static void CheckForWindowsSpotlight()
    {
      var pressed = Console.ReadKey(true);
      if (pressed.Key == ConsoleKey.S)
      {
        string genericApp = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string spotLightLocation = $@"{genericApp}\Appdata\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";

        WriteToConsole("\nSearching Windows Spotlight for new wallpapers...\n", ConsoleColor.Green);

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
      //the @ symbol ensures that windows recognizes the "/" symbol no matter the direction
      var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      string dest = $@"{userFolder}\Pictures\Spotlight";

      if (Directory.Exists(dest))
      {
        WriteToConsole($"Looks like you've already setup a {dest} folder. Press C to proceed.\n", ConsoleColor.Red);
      }
      else
      {
        WriteToConsole($"This will create a new folder located at {dest}. Press C to proceed.\n", ConsoleColor.Red);
      }

      var pressed = Console.ReadKey(true);

      if (pressed.Key == ConsoleKey.C)
      {
        //creates new folder if it doesn't already exist
        Directory.CreateDirectory(dest);

        //here we check for files, if there are files we compare dates to see if app already used
        CheckIfCanContinue(dest);

        WriteToConsole("Looks like there are new Wallpapers to copy.\n", ConsoleColor.Green);

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
    static void CheckIfCanContinue(string dest)
    {
      if (Directory.GetFiles(dest).Length != 0)
      {
        //populate current list of files and grab last index
        var lastFile = GetLastFileIndex(dest);

        //check if the last file's creation date is the same as current
        if (GetIfUsedToday(_filesInFolder[lastFile]))
        {
          WriteToConsole($"It seems like you've already used the app today...\n", ConsoleColor.Red);
          WriteToConsole($"Try again tommorow. Spotlight only updates so many times...\n", ConsoleColor.Red);

          //delay the code execution so the message can come up
          int milliseconds = 3000;
          Thread.Sleep(milliseconds);

          Environment.Exit(0);
        }
      }
    }
    //loop through the files we copied over and check their dimensions, delete the ones that don't meet criteria
    private static void DeleteGarbage(string createdFolder)
    {
      WriteToConsole("Deleting the Garbage...\n", ConsoleColor.Red);

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
      WriteToConsole($"Deleted {filesToDelete.Count} files...\n", ConsoleColor.Yellow);
      foreach (string s in filesToDelete)
      {
        File.Delete(s);
      }

      RenameFiles(createdFolder);
    }

    //organize the files after we've combed through and rename with date
    private static void RenameFiles(string createdFolder)
    {
      WriteToConsole("Renaming files...\n", ConsoleColor.Red);

      string[] files = Directory.GetFiles(createdFolder);

      int index = 0;

      foreach (string f in files)
      {
        //check original filename without renaming which should be series of random chars
        var named = Path.GetFileNameWithoutExtension(f);
        //avoid incrementing the index with the unnamed raw files before actually creating   
        if (named.Contains("Wallpaper"))
        {
          //think about making this equivalent to the number of files in the folder and set that as the current int before ++
          //also pass the name of that last file down to do a string comparison to see if we continue with the operation
          index++;
        }
      }

      foreach (string s in files)
      {
        //check original filename without renaming which should be series of random chars
        var named = Path.GetFileNameWithoutExtension(s);

        //turn the date into a var that we can use to compare files
        //add a '_' to string or some split indicator so we can use it to filter out files
        //check to see if the date has already been used, if it has then abort operation
        //always check the last file
        var date = DateTime.Now.ToString("MMM-d-yyyy");
        string newName = $"Wallpaper_{index}_{date}.jpg";
        string newFile = Path.Combine(createdFolder, newName);

        //if the file already has wallpaper in it that means it's already been renamed skip it
        if (!named.Contains("Wallpaper"))
        {
          File.Copy(s, newFile, true);

          var createdFile = Path.GetFileNameWithoutExtension(newFile);

          WriteToConsole(createdFile, ConsoleColor.Red);

          index++;
        }
      }
      WriteToConsole($"\nOperation done. Enjoy your {files.Length} new Wallpapers.\n", ConsoleColor.Yellow);
      WriteToConsole("Press any key to Quit.\n", ConsoleColor.Red);
      Console.ReadKey(true);
      Environment.Exit(0);
    }

    //returns the last index of JPG files in the folder
    private static int GetLastFileIndex(string createdFolder)
    {
      _filesInFolder = new List<string>();
      foreach (string s in Directory.GetFiles(createdFolder, "*.jpg"))
      {
        _filesInFolder.Add(s);
      }

      //grab the last entry in the list and make the current index that
      return _filesInFolder.Count - 1;
    }

    //make a method that returns the creation date of the last file in the folder
    //if the date matches today's date then we've already used the program so stop operation/delete files grabbed 
    static bool GetIfUsedToday(string path)
    {
      //get file name raw so we can split at _
      var named = Path.GetFileNameWithoutExtension(path);
      var split = named.LastIndexOf("_") + 1;
      var sub = named.Substring(split);
      //substring split here 
      if (sub == DateTime.Now.ToString("MMM-d-yyyy"))
      {
        return true;
      }
      return false;
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
