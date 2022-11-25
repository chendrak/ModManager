using System;
using System.Collections.Generic;
using BepInEx;
using ModManager.Helpers;
using ModManager.UI.Base;
using UniverseLib.UI.Widgets.ScrollView;

namespace ModManager.UI;

public class ModListHandler : UiElementListHandler<PluginInfo, ModUICell>
{
    public ModListHandler(ScrollPool<ModUICell> scrollPool, Func<List<PluginInfo>> getEntriesMethod) : base(scrollPool,
        getEntriesMethod, null, null)
    {
        SetICell = SetComponentCell;
        OnCellClicked = OnModClicked;
    }

    private void OnModClicked(int index)
    {
        Log.Info($"ModListHandler.OnModClicked({index})");
        var mods = GetEntries();

        if (index < 0 || index >= mods.Count) return;

        var mod = mods[index];
        Log.Info($"Selected {mod.Metadata.Name}");
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