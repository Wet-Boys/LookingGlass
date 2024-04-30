using BepInEx;
using FOSSUI.AutoSortItems;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FOSSUI
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class BasePlugin : BaseUnityPlugin
    {
        public const string PluginGUID = $"{PluginAuthor}.{PluginName}";
        public const string PluginAuthor = "Balls";
        public const string PluginName = "InYourMouth";
        public const string PluginVersion = "1.0.0";
        internal static BasePlugin instance;
        public void Awake()
        {
            Log.Init(Logger);
            instance = this;
            AutoSortItemsClass thing = new AutoSortItemsClass();
            thing.Setup();
        }

        private void Update()
        {
        }
    }
}
