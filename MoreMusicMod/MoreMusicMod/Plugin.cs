using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MoreMusicMod.Patches;
using MoreMusicMod.Services;

namespace MoreMusicMod
{
    [BepInPlugin(Guid, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        
        private const string Guid = "georgebjork.MoreMusicMod";
        private const string Name = "MoreMusicMod";
        private const string Version = "1.0.2";
        
        private static Plugin _instance;
        
        private void Awake()
        {
            _instance = this;

            ResourceLoaderService.GenerateFolders();
            
            var harmony = new Harmony(Guid);
            harmony.PatchAll(typeof(BoomboxPatch));
            harmony.PatchAll(typeof(StartUp));
            
            // Plugin startup logic
            Logger.LogInfo($"Plugin {Guid} is loaded!");
        }
        
        
        // Logging functions to be used throughout 
        internal static void LogDebug(string message) => _instance.Log(message, LogLevel.Debug);
        internal static void LogInfo(string message) => _instance.Log(message, LogLevel.Info);
        internal static void LogWarning(string message) => _instance.Log(message, LogLevel.Warning);
        internal static void LogError(string message) => _instance.Log(message, LogLevel.Error);
        internal static void LogError(Exception ex) => _instance.Log($"{ex.Message}\n{ex.StackTrace}", LogLevel.Error);
        private void Log(string message, LogLevel logLevel) => Logger.Log(logLevel, message);
    }
}