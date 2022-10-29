using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Unity.IL2CPP;

namespace ModManager;

public static class ModListController
{
    public static List<PluginInfo> PluginInfos => IL2CPPChainloader.Instance.Plugins.Values.ToList();
}