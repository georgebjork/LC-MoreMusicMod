using System;
using System.IO;
using System.Linq;
using MoreMusicMod.Configuration;

namespace MoreMusicMod.Services;

public class ResourceLoaderService : BaseService {
    
    
    
    public static void GenerateFolders()
    {
        try
        {
            // Check if the dir exists first 
            if (Directory.Exists(_music_directory))
            {
                Plugin.LogInfo($"Directory already exists at: {_music_directory}");
                return;
            }
                
            Directory.CreateDirectory(_music_directory);
            Plugin.LogInfo($"Directory created at: {_music_directory}");
            
            // Check if any other folders in the directory have _music_directory
            CheckFoldersForMusicDirectory();
        }
        catch (Exception ex)
        {
            Plugin.LogError(ex);
            Plugin.LogError($"Something went wrong. Unable to create directory at {_music_directory}");
        }
    }

    private static void CheckFoldersForMusicDirectory()
    {
        try
        {
            // Get all folder names in the current directory
            var folders = Directory.GetDirectories(_plugins_directory).ToList();
            
            // Exclude the music dir we created
            folders.Remove(_music_directory);

            foreach (var folder in folders)
            {
                // Check the folder if it contains the right pattern
                if (CheckFolder(folder))
                {
                    MoveFilesFromFolderToMusicDirectory(Path.Combine(folder, Settings.SONGS_FOLDER));
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., access denied)
            Plugin.LogError("An error occurred: " + ex.Message);
        }
    }


    private static bool CheckFolder(string folder)
    {
        return Directory.Exists(Path.Combine(folder, Settings.SONGS_FOLDER));
    }

    private static void MoveFilesFromFolderToMusicDirectory(string folder)
    {
        var files = Directory.GetFiles(folder);
        Plugin.LogInfo($"Moving from {folder}, {files.Length} files found.");

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            var destinationFile = Path.Combine(_music_directory, fileName);
            
            File.Copy(file, destinationFile);
            Plugin.LogInfo($"Moved {file} to {destinationFile}");
        }
    }
}


