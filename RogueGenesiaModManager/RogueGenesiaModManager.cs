using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ModManager.UI;

namespace ModManager
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class RogueGenesiaModManager : RogueGenesiaMod
    {
        internal new static ManualLogSource Log;

        public override void Load()
        {
            Log = base.Log;
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loading! Patching!");
            
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            // AddComponent<UiKeyPressHandler>();

            UiManager.Initialize();
            
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        public override string ModDescription() => "A mod that allows helps with other mods";
    }
}
