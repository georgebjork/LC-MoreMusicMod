using HarmonyLib;
using MoreMusicMod.Services;

namespace MoreMusicMod.Patches;

[HarmonyPatch(typeof(BoomboxItem))]
internal class BoomboxPatch
{

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
    [HarmonyPostfix]
    static void StartMusic(BoomboxItem __instance, bool startMusic)
    {
        if (startMusic) Plugin.LogInfo($"Playing {__instance.boomboxAudio.clip.name}");
    }
}