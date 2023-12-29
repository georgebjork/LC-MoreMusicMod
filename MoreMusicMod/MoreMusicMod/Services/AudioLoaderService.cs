using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MoreMusicMod.Helpers;
using UnityEngine;
using UnityEngine.Networking;

namespace MoreMusicMod.Services;

public class AudioLoaderService : BaseService
{
    // Our trigger action for when songs are loaded
    public static event Action AllSongsLoaded;
    
    // List of our music clips
    private static readonly List<AudioClip> _clips = new();
    
    // Hold the names of our songs at the dir
    private static string[] _songs_at_directory;
    
    // Is our songs loaded
    public static bool IsLoaded = false;
    
    // Should we use default songs or not.
    public static bool UseDefaultSongs => IsLoaded == false && _songs_at_directory.Length <= 0;

    public static int ClipsLength => _clips.Count; 

    

    public static void LoadAllSongs()
    {
        // Get our files to start.
        GetFilesAtDirectory();
        Plugin.LogInfo("Attempting to load songs...");

        try
        {
            var coroutines = new List<IEnumerator>();
            foreach (var track in _songs_at_directory)
            {
                coroutines.Add(LoadAudioClip(track));
            }

            HandleCoroutineList(coroutines, OnAllSongsLoaded);
            
        }
        catch (Exception ex)
        {
            Plugin.LogError($"There was an error loading audio tracks. Exception: {ex}");
        }
    }
    
    
    private static void GetFilesAtDirectory()
    {
        // Check if this has already been loaded
        if(IsLoaded) { return; }
        
        // Get the files at that dir
        _songs_at_directory = Directory.GetFiles(_music_directory);

        if (_songs_at_directory.Length <= 0)
        {
            Plugin.LogWarning($"No songs where found at {_music_directory}");
            return;
        }
        
        Plugin.LogInfo("Songs found!");
    }


    private static IEnumerator LoadAudioClip(string filePath)
    {
        Plugin.LogInfo($"Loading track: {Path.GetFileName(filePath)}");
        
        
        var audioType = AudioHelper.GetAudioType(filePath);
        if (audioType == AudioType.UNKNOWN)
        {
            // This file type is unsupported. End this routine.
            Plugin.LogError($"Failed to load AudioClip from {filePath}. Unsupported file extension. Supported types are: MPEG, WAV, and OGGVORBIS ");
            yield break;
        }
        
        // Stream it from disk
        var clip = StreamAudioClipFromDisk(filePath, audioType);
        
        // Something went wrong so quit out.
        if(clip is null) {yield break;}    
        
        // Add the clip
        clip.name = Path.GetFileName(filePath);
        _clips.Add(clip);
        Plugin.LogInfo("Clip added.");
    }

    
    private static AudioClip StreamAudioClipFromDisk(string filePath, AudioType audioType)
    {
        // Init a loader and stream from disk
        using var loader = UnityWebRequestMultimedia.GetAudioClip(filePath, audioType);
        ((DownloadHandlerAudioClip)loader.downloadHandler).streamAudio = true;

        // Send the request and set a timeout of 10
        loader.timeout = 10;
        loader.SendWebRequest();

        // Wait for the request to complete.
        while (!loader.isDone)
        {
            if (loader.error == null || loader.timeout <= 10) continue;
            
            Plugin.LogError($"Error loading clip: {Path.GetFileName(filePath)}: {loader.error}");
            return null;
        }

        // Get the downloaded AudioClip
        var clip = DownloadHandlerAudioClip.GetContent(loader);
        return clip;
    }
    
    public static void AddClips(BoomboxItem __instance)
    {
        Plugin.LogInfo("Adding clips!");
            
        __instance.musicAudios = UseDefaultSongs ? __instance.musicAudios.Concat(_clips).ToArray() : _clips.ToArray();

        Plugin.LogInfo($"{_clips.Count}/{_songs_at_directory.Length} where loaded!!");
    }
    
    private static void OnAllSongsLoaded()
    {
        AllSongsLoaded?.Invoke();
        AllSongsLoaded = null;
        IsLoaded = true;
    }
}