using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using ModManager.UI.Base;
using UniverseLib.UI.Widgets.ScrollView;

namespace ModManager.UI;

public class ModListHandler : UiElementListHandler<PluginInfo, ModUICell>
{
    private ManualLogSource Logger => RogueGenesiaModManager.Log;
    
    public ModListHandler(ScrollPool<ModUICell> scrollPool, Func<List<PluginInfo>> getEntriesMethod) : base(scrollPool,
        getEntriesMethod, null, null)
    {
        SetICell = SetComponentCell;
        OnCellClicked = OnModClicked;
    }

    private void OnModClicked(int index)
    {
        Logger.LogInfo($"ModListHandler.OnModClicked({index})");
        var mods = GetEntries();

        if (index < 0 || index >= mods.Count) return;

        var mod = mods[index];
        Logger.LogInfo($"Selected {mod.Metadata.Name}");
    }

    private void SetComponentCell(ModUICell cell, int index)
    {
        var entries = GetEntries();
        cell.Enable();

        var mod = entries[index];
        cell.Bind(mod);
    }

    public override void OnCellBorrowed(ModUICell cell) {}
}