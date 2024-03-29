using System.IO;
using UnityEngine;

namespace ModManager.Helpers;

public static class SpriteHelper
{
    public static Texture2D LoadPNGIntoTexture(string filePath) {
        Log.Info($"Loading PNG from {filePath}");
        Texture2D tex = null;

        if (File.Exists(filePath)) {
            var fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2, TextureFormat.RGBA32, true, false);
            ImageConversion.LoadImage(tex, fileData);
            
            if (tex != null)
            {
                // Set FilterMode so the images don't end up blurry
                tex.filterMode = FilterMode.Point;
            }
            else
            {
                Log.Error($"Texture couldn't be loaded: {filePath}");
            }
        }
        else
        {
            Log.Error($"Texture file does not exist: {filePath}");
        }

        return tex;
    }

    public static Sprite LoadSpriteFromFile(string filePath)
    {
        var tex = LoadPNGIntoTexture(filePath);
        return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
    }
}