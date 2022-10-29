using BepInEx;
using ModManager.Helpers;
using ModManager.UI.Base;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
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
        UIRoot = UIFactory.CreateHorizontalGroup(parent, "ModCell", true,
            false, true, true, 2, default,
            new Color(0.11f, 0.11f, 0.11f), TextAnchor.MiddleCenter);
        Rect = UIRoot.GetComponent<RectTransform>();
        Rect.anchorMin = new Vector2(0, 1);
        Rect.anchorMax = new Vector2(0, 1);
        Rect.pivot = new Vector2(0.5f, 1);
        Rect.sizeDelta = new Vector2(25, 25);
        UIFactory.SetLayoutElement(UIRoot, minWidth: 100, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);

        UIRoot.SetActive(false);

        GameObject textWrappers = UIFactory.CreateVerticalGroup(UIRoot.gameObject,
            name: "TextWrapper", forceWidth: true, forceHeight: false,
            childControlWidth: true, childControlHeight: true,
            spacing: 2, padding: default, bgColor: new Color(0.11f, 0.11f, 0.11f),
            childAlignment: TextAnchor.MiddleLeft
        );

        TitleText = UIFactory.CreateLabel(textWrappers, "TitleText", "Mod Title", fontSize: TitleFontSize);
        UIFactory.SetLayoutElement(TitleText.gameObject, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);
        TitleText.horizontalOverflow = HorizontalWrapMode.Overflow;
        TitleText.font = GameResources.PixelFont;
        
        DescriptionText = UIFactory.CreateLabel(textWrappers, "DescriptionText", "Mod Description", fontSize: DescriptionFontSize);
        UIFactory.SetLayoutElement(DescriptionText.gameObject, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);
        DescriptionText.horizontalOverflow = HorizontalWrapMode.Overflow;
        DescriptionText.font = GameResources.PixelFont;

        DetailsButton = UIFactory.CreateButton(UIRoot, "ModDetailsButton", "?");
        DetailsButton.ButtonText.fontSize = DetailButtonFontSize;
        DetailsButton.ButtonText.font = GameResources.PixelFont;
        
        UIFactory.SetLayoutElement(DetailsButton.Component.gameObject, flexibleWidth: 0, minWidth: 25, minHeight: 25, flexibleHeight: 0);
        
        DetailsButton.GameObject.SetActive(false);
        
        Color normal = new(0.11f, 0.11f, 0.11f);
        Color highlight = new(0.16f, 0.16f, 0.16f);
        Color pressed = new(0.05f, 0.05f, 0.05f);
        Color disabled = new(1, 1, 1, 0);
        RuntimeHelper.SetColorBlock(DetailsButton.Component, normal, highlight, pressed, disabled);

        return UIRoot;
    }

    private void OnDetailButtonClicked(RogueGenesiaMod data)
    {
    }

    public override void Bind(PluginInfo obj)
    {
        base.Bind(obj);
        var md = obj.Metadata;
        TitleText.text = $"{md.Name} v{md.Version}";

        if (obj.Instance is RogueGenesiaMod rogueGenesiaMod)
        {
            DescriptionText.text = rogueGenesiaMod.ModDescription();
            DetailsButton.GameObject.SetActive(rogueGenesiaMod.HasDialog());
            DetailsButton.OnClick = () => { OnDetailButtonClicked(rogueGenesiaMod); };
        }
        else
        {
            DescriptionText.text = "No description provided";
        }
    }
}