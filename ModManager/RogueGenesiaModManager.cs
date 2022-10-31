using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ModManager.UI;
using UnityEngine;

namespace ModManager
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class RogueGenesiaModManager : RogueGenesiaMod
    {
        internal new static ManualLogSource Log;

        public override void Load()
        {
            Log = base.Log;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            UiManager.Initialize();
        }

        public override string ModDescription() => "A Mod for Rogue Genesia that can help manage other Mods for Rogue Genesia";

        public override bool SupportsDetailButtonClick() => false;

        public override void OnDetailButtonClicked(GameObject modManagerDialog)
        {
            Log.LogInfo("Detail button clicked");
        }
    }
}
