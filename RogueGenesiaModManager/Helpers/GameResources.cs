#nullable enable
using System.Linq;
using UnityEngine;
using UniverseLib;
using UniverseLib.UI;

namespace ModManager.Helpers;

public static class GameResources
{
    private static Font _pixelFont = null!;

    private static T? GetResourceByName<T>(string resourceName) where T : Object
    {
        var objects = RuntimeHelper.FindObjectsOfTypeAll<T>();
        return objects?.FirstOrDefault(obj => obj.name == resourceName);
    }
    
    public static Font PixelFont
    {
        get
        {
            if (_pixelFont == null)
            {
                _pixelFont = GetResourceByName<Font>("pixelplay") ?? UniversalUI.DefaultFont;
            }
            
            return _pixelFont;
        }
    }
}