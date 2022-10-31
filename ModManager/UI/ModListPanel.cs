using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using ModManager.Helpers;
using ModManager.UI.Extensions;
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
    public ModListHandler ModList { get; private set; }

    private ScrollPool<ModUICell> _modScroll;

    protected override void ConstructPanelContent()
    {
        RogueGenesiaModManager.Log.LogInfo("ConstructPanelContent");
        
        #region TitleArea
        GameObject navbarPanel = UIFactory.CreateUIObject("MainNavbar", ContentRoot);
        UIFactory.SetLayoutGroup<VerticalLayoutGroup>(navbarPanel, true, false, true, true, 2, 5, 5, 6, 6,
            TextAnchor.MiddleCenter);

        Text titleText = UIFactory.CreateLabel(navbarPanel.gameObject, "WindowTitle", Name, TextAnchor.MiddleCenter,
            fontSize: TitleFontSize);
        UIFactory.SetLayoutElement(titleText.gameObject, 50, 25, 9999, 0);
        titleText.font = GameResources.PixelFont;

        var menuLine = UIFactory.CreateUIObject("MenuLine", navbarPanel);
        var img = menuLine.AddComponent<Image>();
        img.color = GameResources.DefaultGray;

        UIFactory.SetLayoutElement(menuLine, minHeight: 2, flexibleWidth: 9999);
        #endregion

        #region Scrollable List
        _modScroll = UIFactory.CreateScrollPool<ModUICell>(ContentRoot, "ModList", out GameObject compObj,
            out GameObject compContent);
        
        ModList = new ModListHandler(_modScroll, GetModEntries);
        _modScroll.Initialize(ModList);

        UIFactory.SetLayoutGroup<VerticalLayoutGroup>(compContent, spacing: 2, padTop: 0, padBottom: 0, padLeft: 8, padRight: 8);
        #endregion

        this.RemoveBackgroundFromElements("Content", "Background", "ModList", "Viewport");

        #region Close Button 
        CloseBtn = UIFactory.CreateButton(ContentRoot, "CloseBtn", "Close", Color.white);
        CloseBtn.ButtonText.font = GameResources.PixelFont;
        CloseBtn.ButtonText.fontSize = CloseButtonFontSize;
        UIFactory.SetLayoutElement(CloseBtn.Component.gameObject, minWidth: 160, minHeight: 40, preferredWidth: 160,
            preferredHeight: 40, flexibleWidth: 9999, flexibleHeight: 0);

        CloseBtn.OnClick += UiManager.HideModList;

        UiHelper.SetBackgroundSprite(UIRoot, GameResources.DefaultButtonSprite);
        CloseBtn.Component.image.sprite = GameResources.DefaultButtonSprite;
        CloseBtn.Component.spriteState = GameResources.DefaultSpriteState;
        #endregion
        
        var bgImages = UIRoot.GetComponentsInChildren<Image>();
        if (bgImages != null)
        {
            foreach (var bgImage in bgImages)
            {
                Logger.LogInfo($"Image: {bgImage.name}, Color: #{UiHelper.ToHex(bgImage.color)}");
            }
        }
    }

    private List<PluginInfo> GetModEntries() => ModListController.PluginInfos;
    // private List<PluginInfo> GetModEntries()
    // {
    //     var entries = ModListController.PluginInfos;
    //     var result = new List<PluginInfo>();
    //
    //     for (int i = 0; i < 10; i++)
    //     {
    //         result.AddRange(entries);
    //     }
    //
    //     return result;
    // }
}