using BepInEx.Configuration;
using LookingGlass.Base;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace LookingGlass.ResizeCommandWindow
{
    internal class ModifyCommandWindow : BaseThing
    {
        public static ConfigEntry<bool> resize;
        public static ConfigEntry<float> opacity;


        public ModifyCommandWindow()
        {
            Setup();
        }
        public void Setup()
        {
            resize = BasePlugin.instance.Config.Bind<bool>("Settings", "Resize Command Window", true, "Resizes the command window to fit modded items");
            opacity = BasePlugin.instance.Config.Bind<float>("Settings", "Command Window Opacity", .5f, "Changes the Opacity of the regular command window");
        }

        internal void ResizeWindow(PickupPickerController panel)
        {

            Transform t = panel.panelInstance.transform.Find("MainPanel");
            if (t is not null)
            {
                Transform background = t.Find("Juice/BG");
                if (background is not null)
                {

                    background.GetComponent<Image>().color = new Color(1, 1, 1, opacity.Value);
                }
            }
            if (!resize.Value)
                return;

            GridLayoutGroup gridLayoutGroup = panel.panelInstance.transform.GetComponentInChildren<GridLayoutGroup>();
            int itemCount = gridLayoutGroup.transform.childCount - 1;
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            if (t is not null)
            {
                RectTransform r = t.GetComponent<RectTransform>();
                int maxHeight = 12;
                int value = Mathf.CeilToInt((Mathf.Sqrt(itemCount) + 2));
                gridLayoutGroup.constraintCount = Mathf.Min(maxHeight, value) - 2;
                float height = Mathf.Min(value, maxHeight) * (r.sizeDelta.x / 8f);
                value = value <= maxHeight ? value : value + 1 + value - maxHeight;
                float width = (value) * (r.sizeDelta.x / 8f);
                r.sizeDelta = new Vector2(width, height);
            }
        }
    }
}
