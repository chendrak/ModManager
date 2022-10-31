using System.IO;
using UnityEngine;

namespace ModManager.Helpers;

public static class ModManagerResource
{
    public static readonly Sprite CogwheelIcon =
        SpriteHelper.LoadSpriteFromFile(Path.Combine(Paths.PluginPath, "cogwheel.png"));
}