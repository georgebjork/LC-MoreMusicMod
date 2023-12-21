using System;

namespace MoreMusicMod.Services;

public class ResourceLoaderService : BaseService {
    
    public static void GenerateFolders()
    {
        try
        {
            // Check if the dir exists first 
            if (System.IO.Directory.Exists(_music_directory))
            {
                Plugin.LogInfo($"Directory already exists at: {_music_directory}");
                return;
            }
                
            System.IO.Directory.CreateDirectory(_music_directory);
            Plugin.LogInfo($"Directory created at: {_music_directory}");
        }
        catch (Exception ex)
        {
            Plugin.LogError(ex);
            Plugin.LogError($"Something went wrong. Unable to create directory at {_music_directory}");
        }
    }
}