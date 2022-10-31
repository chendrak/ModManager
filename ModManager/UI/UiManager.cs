#nullable enable
using System.IO;
using BepInEx.Logging;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using RogueGenesia.UI;
using ModManager.Extensions;
using ModManager.Helpers;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.UI;

namespace ModManager.UI;

[HarmonyPatch]
internal static class UiManagerMainMenuHooks
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static void OnMainMenuManagerStartCalled(MainMenuManager __instance)
    {
        RogueGenesiaModManager.Log.LogInfo($"OnMainMenuManagerStartCalled({__instance.GetHashCode()})");

        UiManager.mainMenuManager = __instance;

        var buttons = __instance.MainMenu.GetComponentsInChildren<Button>();
        if (buttons != null)
        {
            foreach (var button in buttons)
            {
                switch (button.name)
                {
                    case "Play":
                        button.gameObject.GetOrAddComponent<UiManager.PlayButtonVisibilityListener>();
                        break;
                    case "Join Discord":
                        UiManager.buttonTemplate = button;
                        break;
                }
            }
        }
    }
}

internal static class UiManager
{
    private static ManualLogSource Logger => RogueGenesiaModManager.Log;

    public static UIBase UiBase { get; private set; }

    public static ModListPanel? ModListPanel { get; set; }

    private static Button? ModButton { get; set; }

    internal static Button? buttonTemplate;

    internal static MainMenuManager? mainMenuManager;

    internal static void Initialize()
    {
        ClassInjector.RegisterTypeInIl2Cpp<PlayButtonVisibilityListener>();

        const float startupDelay = 0f;
        UniverseLib.Config.UniverseLibConfig config = new()
        {
            Disable_EventSystem_Override = false, // or null
            Force_Unlock_Mouse = false, // or null
            Allow_UI_Selection_Outside_UIBase = true,
            Unhollowed_Modules_Folder = Path.Combine(BepInEx.Paths.BepInExRootPath, "interop") // or null
        };

        Universe.Init(startupDelay, OnInitialized, LogHandler, config);
    }

    static void OnInitialized()
    {
        Logger.LogInfo("UIManager.OnInitialized");
        UiBase = UniversalUI.RegisterUI(MyPluginInfo.PLUGIN_GUID, UiUpdate);
    }

    internal static void CreateModButton(GameObject parent)
    {
        if (buttonTemplate)
        {
            Logger.LogInfo($"Got the template for the mod button!");

            var buttonTemplateParent = buttonTemplate.transform.parent.gameObject;

            Logger.LogInfo($"buttonTemplateParent.name: {buttonTemplateParent.name}");

            ModButton = Object.Instantiate(buttonTemplate, parent.transform, false);
            ModButton.name = "ModsButton";

            var buttonText = ModButton.GetComponentInChildren<Text>();
            if (buttonText) buttonText.text = "Mods";

            // Update image
            var image = ModButton.GetChildComponentByName<Image>("Discord");
            if (image)
            {
                image.name = "ModImage";
                var iconSprite = SpriteHelper.LoadSpriteFromFile(Path.Combine(Paths.Assets, "icon.png"));
                image.sprite = iconSprite;
            }
            else
            {
                Logger.LogInfo("No image!");
            }

            // Reposition button
            var transForm = ModButton.transform.TryCast<RectTransform>();
            if (transForm != null)
            {
                var position = transForm.position;
                var size = transForm.sizeDelta;
                var newPosition = new Vector2(position.x, position.y + size.y);
                transForm.position = newPosition;

                Logger.LogInfo($"Mod button position: x: {newPosition.x}, y: {newPosition.y} ");
            }

            // Set click listener
            ModButton.onClick = new Button.ButtonClickedEvent();
            ModButton.onClick.AddListener(ShowModList);
        }
        else
        {
            Logger.LogError($"NO MOD BUTTON TEMPLATE FOUND :(");
        }
    }

    static void LogHandler(string message, LogType type)
    {
        Logger.LogInfo(message);
    }

    internal static void CreateAllPanels()
    {
        CreateModListPanel();
    }

    private static void CreateModListPanel()
    {
        ModListPanel = new ModListPanel(UiBase);
        ModListPanel.SetActive(false);
    }

    public static void ShowModList()
    {
        if (ModListPanel == null) CreateModListPanel();
        ModListPanel.SetActive(true);
    }

    public static void HideModList()
    {
        ModListPanel.SetActive(false);
    }

    static void UiUpdate()
    {
        // Called once per frame when your UI is being displayed.
    }

    public class PlayButtonVisibilityListener : MonoBehaviour
    {
        private void OnEnable()
        {
            if (ModButton == null)
            {
                CreateModButton(UiBase.RootObject);
            }

            ModButton.OnEnable();
            ModButton.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            ModButton.OnDisable();
            ModButton.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            buttonTemplate = null;
            mainMenuManager = null;
        }
    }
}