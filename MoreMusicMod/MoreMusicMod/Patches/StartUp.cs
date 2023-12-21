using HarmonyLib;
using MoreMusicMod.Services;

namespace MoreMusicMod.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal class StartUp
{
    [HarmonyPatch("Awake")]
    [HarmonyPrefix]
    static void Prefix()
    {
        AudioLoaderService.LoadAllSongs();
        Plugin.LogInfo($"Is using default songs: {AudioLoaderService.UseDefaultSongs}");
    }
}