#nullable enable
using BepInEx.Unity.IL2CPP;
using UnityEngine;

namespace ModManager;

public abstract class RogueGenesiaMod : BasePlugin
{
    public abstract string ModDescription();
    public virtual bool HasDialog() => false;
    public virtual GameObject? GetModDialog() => null;
}