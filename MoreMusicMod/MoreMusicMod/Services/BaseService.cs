using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using MoreMusicMod.Configuration;
using UnityEngine;

namespace MoreMusicMod.Services;

public class BaseService : MonoBehaviour
{
    // Instance of our service
    private static BaseService _instance;
    
    // Here for any service that needs to access the music directory
    protected static readonly string _music_directory = Path.Combine(Paths.BepInExRootPath, Settings.PLUGINS_DIRECTORY, Settings.SONGS_FOLDER);
    
    // Current coroutines
    private static readonly List<Coroutine> _active_coroutines = new();
    
    // This will be used in any derived service if it has to start a service
    protected new static Coroutine StartCoroutine(IEnumerator routine)
    {
        if (_instance != null) return ((MonoBehaviour)_instance).StartCoroutine(routine);
        
        _instance = new GameObject("Shared Coroutine Starter").AddComponent<BaseService>();
        DontDestroyOnLoad(_instance);

        return ((MonoBehaviour)_instance).StartCoroutine(routine);
    }

    
    protected static void HandleCoroutineList(List<IEnumerator> coroutines, Action onComplete)
    {
        StartCoroutine(RunCoroutineList(coroutines, onComplete));
    }
    
    private static IEnumerator RunCoroutineList(List<IEnumerator> coroutineList, Action onComplete)
    {
        foreach (var routine in coroutineList)
        {
            var rv = StartCoroutine(routine);
            _active_coroutines.Add(rv);
        }
        
        foreach (var coroutine in _active_coroutines)
        {
            yield return coroutine;
        }
        
        _active_coroutines.Clear();
        Plugin.LogInfo($"ALL DONE!! {_active_coroutines.Count}");
        
        onComplete?.Invoke();
    }

    /*protected static void HandleCoroutineList(List<IEnumerator> coroutines)
    {
        foreach (var routine in coroutines)
        {
            var rv = StartCoroutine(routine);
            _coroutines.Add(rv);
        }

        StartCoroutine(WaitForAllCoroutines());
    }
    
    private static IEnumerator WaitForAllCoroutines()
    {
        foreach (var coroutine in _coroutines)
        {
            yield return coroutine;
        }
    }*/
}