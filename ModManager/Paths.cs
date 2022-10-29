using System.IO;

namespace ModManager;

public static class Paths
{
    public static string Assets = Path.Combine(BepInEx.Paths.PluginPath, MyPluginInfo.PLUGIN_NAME ,"Assets");
}