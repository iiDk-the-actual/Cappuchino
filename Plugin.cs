using BepInEx;
using BepInEx.Unity.IL2CPP;
using Cappuchino.Menu;
using HarmonyLib;
using Locomotion;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cappuchino
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Init : BasePlugin
    {
        public static Init instance;

        public override void Load()
        {
            HarmonyPatches.ApplyHarmonyPatches();
            instance = this;

            AddComponent<Main>();
        }

        public override bool Unload()
        {
            HarmonyPatches.RemoveHarmonyPatches();
            return true;
        }
    }
}
