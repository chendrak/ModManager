using System.IO;
using System.Reflection;

namespace ModManager;

public static class Paths
{
    public static readonly string PluginPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ??
                                               Path.Combine(BepInEx.Paths.PluginPath, MyPluginInfo.PLUGIN_NAME);

    public static string Assets = Path.Combine(PluginPath ,"Assets");
}