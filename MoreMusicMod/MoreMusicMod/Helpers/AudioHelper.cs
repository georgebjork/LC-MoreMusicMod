using System.IO;
using UnityEngine;

namespace MoreMusicMod.Helpers;

public class AudioHelper
{
    public static AudioType GetAudioType(string path)
    {
        var extension = Path.GetExtension(path).ToLower();

        if (extension == ".wav")
            return AudioType.WAV;
        if (extension == ".ogg")
            return AudioType.OGGVORBIS;
        if (extension == ".mp3")
            return AudioType.MPEG;

        Plugin.LogError($"Unsupported extension type: {extension}");
        return AudioType.UNKNOWN;
    }
}