using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ExamplePlugin
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class BasePlugin : BaseUnityPlugin
    {
        public const string PluginGUID = $"{PluginAuthor}.{PluginName}";
        public const string PluginAuthor = "Balls";
        public const string PluginName = "InYourMouth";
        public const string PluginVersion = "1.0.0";
        public void Awake()
        {
            Log.Init(Logger);

        }

        private void Update()
        {
        }
    }
}
