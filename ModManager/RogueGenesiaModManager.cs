using System.Reflection;
using BepInEx;
using HarmonyLib;
using ModManager.UI;
using UnityEngine;

namespace ModManager
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class RogueGenesiaModManager : RogueGenesiaMod
    {
        public override void Load()
        {
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
