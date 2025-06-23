using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
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

            AddComponent<Plugin>();
        }

        public override bool Unload()
        {
            HarmonyPatches.RemoveHarmonyPatches();
            return true;
        }
    }

    public class Plugin : MonoBehaviour
    {
        void Start()
        {
            SceneManager.LoadScene("CapuchinCopy");
        }

        void OnGUI()
        {

        }

        void FixedUpdate()
        {
        }
    }
}
