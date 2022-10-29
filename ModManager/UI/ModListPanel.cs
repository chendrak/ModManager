using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using ModManager.Helpers;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets.ScrollView;

namespace ModManager.UI;

public class ModListPanel : UniverseLib.UI.Panels.PanelBase
{
    private const int TitleFontSize = 32;
    private const int CloseButtonFontSize = 20;
    
    private ManualLogSource Logger => RogueGenesiaModManager.Log;

    public static ModListPanel Instance { get; private set; }

    public ModListPanel(UIBase owner) : base(owner)
    {
        Instance = this;
    }

    public override string Name => "Mods";
    public override int MinWidth => 640;
    public override int MinHeight => 480;

    public override Vector2 DefaultAnchorMin => new(0.5f, 1f);
    public override Vector2 DefaultAnchorMax => new(0.5f, 1f);

    public override Vector2 DefaultPosition => new(0 - MinWidth / 2, 0 + MinHeight / 2);

    public override bool CanDragAndResize => false;

    public ButtonRef CloseBtn { get; private set; }
    public ModListHandler ModList;

    private ScrollPool<ModUICell> _modScroll;

    protected override void ConstructPanelContent()
    {
        RogueGenesiaModManager.Log.LogInfo("ConstructPanelContent");

        #region TitleArea

        GameObject navbarPanel = UIFactory.CreateUIObject("MainNavbar", ContentRoot);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(navbarPanel, false, false, true, true, 5, 4, 4, 4, 4,
            TextAnchor.MiddleCenter);
        navbarPanel.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f);

        Text titleText = UIFactory.CreateLabel(navbarPanel.gameObject, "WindowTitle", Name, TextAnchor.MiddleCenter,
            fontSize: TitleFontSize);
        UIFactory.SetLayoutElement(titleText.gameObject, 50, 25, 9999, 0);
        titleText.font = GameResources.PixelFont;

        #endregion

        _modScroll = UIFactory.CreateScrollPool<ModUICell>(ContentRoot, "ModList", out GameObject compObj,
            out GameObject compContent, new Color(1.0f, 1.0f, 1.0f));
        UIFactory.SetLayoutElement(compObj, flexibleHeight: 9999);
        UIFactory.SetLayoutElement(compContent, flexibleHeight: 9999);

        ModList = new ModListHandler(_modScroll, GetModEntries);
        _modScroll.Initialize(ModList);

        CloseBtn = UIFactory.CreateButton(ContentRoot, "CloseBtn", "Close");
        CloseBtn.ButtonText.font = GameResources.PixelFont;
        CloseBtn.ButtonText.fontSize = CloseButtonFontSize;
        UIFactory.SetLayoutElement(CloseBtn.Component.gameObject, minWidth: 160, minHeight: 40, preferredWidth: 160,
            preferredHeight: 40, flexibleWidth: 9999, flexibleHeight: 0);

        CloseBtn.OnClick += UiManager.HideModList;
    }

    private List<PluginInfo> GetModEntries() => ModListController.PluginInfos;
}