using BepInEx;
using ModManager.Helpers;
using ModManager.UI.Base;
using ModManager.UI.Extensions;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace ModManager.UI;

public class ModUICell : BaseUICell<PluginInfo>
{
    private const int TitleFontSize = 20;
    private const int DescriptionFontSize = 16;
    private const int DetailButtonFontSize = 16;

    private Text TitleText { get; set; }
    private Text DescriptionText { get; set; }
    private ButtonRef DetailsButton { get; set; }

    // ICell
    public override float DefaultHeight => 25f;

    public override GameObject CreateContent(GameObject parent)
    {
        UIRoot = UIFactory.CreateUIObject("ModCell", parent);
        UIFactory.SetLayoutGroup<VerticalLayoutGroup>(UIRoot, true,
            false, true, true, 2, 0, 0, 0, 0,
            TextAnchor.MiddleCenter);
        
        UIRoot.SetActive(false);

        var contentWrapper = UIFactory.CreateHorizontalGroup(UIRoot, "ContentWrapper", true,
            false, true, true, 2, default,
            Color.clear, TextAnchor.MiddleCenter);
        Rect = UIRoot.GetComponent<RectTransform>();
        Rect.anchorMin = new Vector2(0, 1);
        Rect.anchorMax = new Vector2(0, 1);
        Rect.pivot = new Vector2(0.5f, 1);
        Rect.sizeDelta = new Vector2(25, 25);

        GameObject textWrappers = UIFactory.CreateVerticalGroup(contentWrapper,
            name: "TextWrapper", forceWidth: true, forceHeight: false,
            childControlWidth: true, childControlHeight: true,
            spacing: 2, padding: default, bgColor: Color.clear,
            childAlignment: TextAnchor.MiddleLeft
        );

        TitleText = UIFactory.CreateLabel(textWrappers, "TitleText", "Mod Title", fontSize: TitleFontSize);
        UIFactory.SetLayoutElement(TitleText.gameObject, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);
        TitleText.horizontalOverflow = HorizontalWrapMode.Wrap;
        TitleText.font = GameResources.PixelFont;
        TitleText.color = Color.white;

        DescriptionText = UIFactory.CreateLabel(textWrappers, "DescriptionText", "Mod Description", fontSize: DescriptionFontSize);
        UIFactory.SetLayoutElement(DescriptionText.gameObject, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);
        DescriptionText.horizontalOverflow = HorizontalWrapMode.Wrap;
        DescriptionText.font = GameResources.PixelFont;

        DetailsButton = UIFactory.CreateButton(contentWrapper, "ModDetailsButton", "?");
        DetailsButton.ButtonText.fontSize = DetailButtonFontSize;
        DetailsButton.ButtonText.font = GameResources.PixelFont;
        
        UIFactory.SetLayoutElement(DetailsButton.Component.gameObject, flexibleWidth: 0, minWidth: 25, minHeight: 25, flexibleHeight: 0);
        
        DetailsButton.GameObject.SetActive(false);
        
        UIRoot.RemoveBackgroundFromElements("ModCell", "TextWrapper", "ContentWrapper");
        
        var menuLine = UIFactory.CreateUIObject("ElementLine", UIRoot);
        var img = menuLine.AddComponent<Image>();
        img.color = GameResources.DefaultGray;
        
        UIFactory.SetLayoutElement(menuLine, minHeight: 2, flexibleWidth: 9999);

        return UIRoot;
    }

    public override void Bind(PluginInfo obj)
    {
        base.Bind(obj);
        var md = obj.Metadata;
        TitleText.text = $"{md.Name} v{md.Version}";

        if (obj.Instance is RogueGenesiaMod rogueGenesiaMod)
        {
            DescriptionText.text = rogueGenesiaMod.ModDescription();
            DetailsButton.GameObject.SetActive(rogueGenesiaMod.SupportsDetailButtonClick());
            DetailsButton.OnClick = () => { rogueGenesiaMod.OnDetailButtonClicked(UiManager.ModListPanel!.ContentRoot); };
        }
        else
        {
            DescriptionText.text = "No description provided";
        }
    }
}