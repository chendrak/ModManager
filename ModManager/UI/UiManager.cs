#nullable enable
using System.Collections;
using System.IO;
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
        Log.Info($"OnMainMenuManagerStartCalled({__instance.GetHashCode()})");
        UiManager._mainMenuManager = __instance;
    }
}

internal static class UiManager
{
    public static UIBase UiBase { get; private set; }

    public static ModListPanel? ModListPanel { get; set; }

    private static Button? ModButton { get; set; }

    internal static Button? _playButton;
    internal static Button? _buttonTemplate;

    internal static MainMenuManager? _mainMenuManager;

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
        UiBase = UniversalUI.RegisterUI(MyPluginInfo.PLUGIN_GUID, UiUpdate);
        CoroutineHelper.StartCoroutine(LocateButtons());
    }

    internal static void CreateModButton(GameObject parent)
    {
        if (_buttonTemplate)
        {
            ModButton = Object.Instantiate(_buttonTemplate, parent.transform, false);
            ModButton.name = "ModsButton";

            var buttonText = ModButton.GetComponentInChildren<Text>();
            if (buttonText) buttonText.text = "Mods";

            // Update image
            var image = ModButton.GetChildComponentByName<Image>("Discord");
            if (image)
            {
                image.name = "ModImage";
                image.sprite = ModManagerResource.CogwheelIcon;
            }
            else
            {
                Log.Warn("No image to replace :/");
            }

            // Reposition button
            var transForm = ModButton.transform.TryCast<RectTransform>();
            if (transForm != null)
            {
                var position = transForm.position;
                var size = transForm.sizeDelta;
                var newPosition = new Vector2(position.x, position.y + size.y);
                transForm.position = newPosition;
            }

            // Set click listener
            ModButton.onClick = new Button.ButtonClickedEvent();
            ModButton.onClick.AddListener(ShowModList);
            ModButton.gameObject.SetActive(false);
        }
        else
        {
            Log.Error($"NO MOD BUTTON TEMPLATE FOUND :(");
        }
    }

    static void LogHandler(string message, LogType type)
    {
        Log.Info(message);
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

    private static IEnumerator ShowModButton()
    {
        while (ModButton == null) yield return new WaitForSeconds(0.1f);
        ModButton.OnEnable();
        ModButton.gameObject.SetActive(true);
    }

    private static IEnumerator HideModButton()
    {
        while (ModButton == null) yield return new WaitForSeconds(0.1f);
        ModButton.OnDisable();
        ModButton.gameObject.SetActive(false);
    }

    public class PlayButtonVisibilityListener : MonoBehaviour
    {
        private void OnEnable()
        {
            CoroutineHelper.StartCoroutine(ShowModButton());
        }
        
        private void OnDisable()
        {
            CoroutineHelper.StartCoroutine(HideModButton());
        }

        private void OnDestroy()
        {
            _buttonTemplate = null;
            _playButton = null;
            _mainMenuManager = null;
        }
    }

    public static IEnumerator LocateButtons()
    {
        while (_buttonTemplate == null || _playButton == null)
        {
            if (_mainMenuManager != null)
            {
                var buttons = _mainMenuManager.MainMenu.GetComponentsInChildren<Button>();
                if (buttons != null)
                {
                    foreach (var button in buttons)
                    {
                        switch (button.name)
                        {
                            case "Play":
                                _playButton = button;
                                _playButton.gameObject.GetOrAddComponent<UiManager.PlayButtonVisibilityListener>();
                                break;
                            case "Join Discord":
                                _buttonTemplate = button;
                                break;
                        }
                    }
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
        
        CreateModButton(UiBase.RootObject);
    }
}