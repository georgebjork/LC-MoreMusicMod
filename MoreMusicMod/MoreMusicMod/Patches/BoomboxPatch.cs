using System.Collections;
using HarmonyLib;
using MoreMusicMod.Services;

namespace MoreMusicMod.Patches;

[HarmonyPatch(typeof(BoomboxItem))]
internal static class BoomboxPatch
{

    private static int CurrentIndex = 0;

    [HarmonyPatch(nameof(BoomboxItem.Start))]
    [HarmonyPostfix]
    static void Start(BoomboxItem __instance)
    {
        Plugin.LogInfo($"Is our music loaded: ${AudioLoaderService.IsLoaded}");
        if(AudioLoaderService.IsLoaded) 
        { 
            AudioLoaderService.AddClips(__instance);
            return;
        }
            
        AudioLoaderService.AllSongsLoaded += () => AudioLoaderService.AddClips(__instance);
    }


    [HarmonyPatch("StartMusic")]
    [HarmonyPrefix]
    static bool StartMusicPrefix(BoomboxItem __instance, ref bool startMusic, bool pitchDown)
    {
        if (startMusic)
        {
            // Set the clip to the next in the array based on the CurrentIndex
            __instance.boomboxAudio.clip = __instance.musicAudios[CurrentIndex];
            __instance.boomboxAudio.pitch = 1f;
            __instance.boomboxAudio.Play();

            // Increment and reset the index
            CurrentIndex = (CurrentIndex + 1) % __instance.musicAudios.Length;

            __instance.isBeingUsed = startMusic;
            __instance.isPlayingMusic = startMusic;

            return false; // Skip the original method
        }

        // Let the original method handle the case when startMusic is false
        return true;
    }
}