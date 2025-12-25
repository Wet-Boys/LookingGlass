using BepInEx.Configuration;
using LookingGlass.Base;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using RoR2.UI;
using System.Collections;
using System.Reflection;
using MonoMod.RuntimeDetour;
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
            resize = BasePlugin.instance.Config.Bind<bool>("Command Settings", "Resize Command Window", true, "Resizes the command window to fit modded items");
            opacity = BasePlugin.instance.Config.Bind<float>("Command Settings", "Command Window Opacity", 80f, "Changes the Opacity of the regular command window");
            SetupRiskOfOptions();
        }
        public void SetupRiskOfOptions()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(resize, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new SliderOption(opacity, new SliderConfig() { restartRequired = false }));

            new Hook(
                typeof(PickupPickerPanel).GetMethod(nameof(PickupPickerPanel.SetPickupOptions), BindingFlags.Public | BindingFlags.Instance),
                typeof(ModifyCommandWindow).GetMethod(nameof(ResizeWindow_New), BindingFlags.NonPublic | BindingFlags.Instance), this);
        }

        internal void ResizeWindow_New(Action<PickupPickerPanel, PickupPickerController.Option[]> orig, PickupPickerPanel self, PickupPickerController.Option[] options)
        {
            if (!resize.Value || !self.name.StartsWith("Command"))
            {
                orig(self, options);
                return;
            }
            Transform t = self.transform.Find("MainPanel");
            if (t is not null)
            {
                Transform background = t.Find("Juice/BG");
                if (background is not null)
                {
                    Color originalColor = background.GetComponent<Image>().color;
                    background.GetComponent<Image>().color = new Color(originalColor.r, originalColor.g, originalColor.b, opacity.Value / 100f);
                }
            }

     

            int itemCount = options.Length;
        
            int maxHeight = 12;
            int value = Mathf.CeilToInt((Mathf.Sqrt(itemCount) + 2));
            GridLayoutGroup gridLayoutGroup = self.transform.GetComponentInChildren<GridLayoutGroup>();
            if (gridLayoutGroup)
            {
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                gridLayoutGroup.constraintCount = Mathf.Min(maxHeight, value) - 2;
                self.maxColumnCount = gridLayoutGroup.constraintCount;
                self.automaticButtonNavigation = true;
            }
            orig(self, options);

            if (t is not null)
            {
                RectTransform r = t.GetComponent<RectTransform>();//I'm not reading this section, congratulations or I'm sorry that happened...
                
                float height = Mathf.Min(value, maxHeight) * (r.sizeDelta.x / 8f);
                //int columnReduction = value <= maxHeight ? 0 : 1;
                value = value <= maxHeight ? value : value + 1 + value - maxHeight;
                float width = (value) * (r.sizeDelta.x / 8f);
                width = Mathf.Max(width, 340f);//Min size so the "What is your command Quote" fits
                height = Mathf.Max(height, 340f);//Min size so the "What is your command Quote" fits
                r.sizeDelta = new Vector2(width, height); //Ugh, this is all kinda jank but it works 95%, just come back to this at some point

            }

        }

        /*
        internal void ResizeWindow(PickupPickerController controller)
        {
            return;
            if (!resize.Value || !controller.name.StartsWith("CommandCube"))
                return;
            Transform t = controller.panelInstance.transform.Find("MainPanel");
            if (t is not null)
            {
                Transform background = t.Find("Juice/BG");
                if (background is not null)
                {
                    Color originalColor = background.GetComponent<Image>().color;
                    background.GetComponent<Image>().color = new Color(originalColor.r, originalColor.g, originalColor.b, opacity.Value / 100f);
                }
            }
          

            GridLayoutGroup gridLayoutGroup = controller.panelInstance.transform.GetComponentInChildren<GridLayoutGroup>();
            int itemCount = gridLayoutGroup.transform.childCount - 1;
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            if (t is not null)
            {
                RectTransform r = t.GetComponent<RectTransform>();//I'm not reading this section, congratulations or I'm sorry that happened...
                int maxHeight = 12;
                int value = Mathf.CeilToInt((Mathf.Sqrt(itemCount) + 2));
                gridLayoutGroup.constraintCount = Mathf.Min(maxHeight, value) - 2;
                float height = Mathf.Min(value, maxHeight) * (r.sizeDelta.x / 8f);
                int columnReduction = value <= maxHeight ? 0 : 1;
                value = value <= maxHeight ? value : value + 1 + value - maxHeight;
                float width = (value) * (r.sizeDelta.x / 8f);
                width = Mathf.Max(width, 340f);//Min size so the "What is your command Quote" fits
                height = Mathf.Max(height, 340f);//Min size so the "What is your command Quote" fits
                r.sizeDelta = new Vector2(width, height); //Ugh, this is all kinda jank but it works 95%, just come back to this at some point

                //Fucks up coloring v
                //Makes window way too large v
                //What purpose did this have ???
                Run.instance.StartCoroutine(FixColumnCountAndStuff(gridLayoutGroup, controller));
            }
        }
        public static IEnumerator FixColumnCountAndStuff(GridLayoutGroup gridLayoutGroup, PickupPickerController panel)
        {
         
            yield return new WaitForEndOfFrame();
            int columnCount = 1;
            float firstY = gridLayoutGroup.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition.y;
            for (int i = 2; i < gridLayoutGroup.transform.childCount; i++)
            {
                if (gridLayoutGroup.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition.y == firstY)
                {
                    columnCount++;
                }
            }
            panel.panelInstanceController.maxColumnCount = columnCount;

            //v Doing this fucks up the coloring of the background
            panel.panelInstanceController.SetPickupOptions(panel.options);

            //if this needs to be kept for some reason, idk maybe this reverses the coloring mistake
            PickupDef pickupDef = PickupCatalog.GetPickupDef(panel.options[0].pickup.pickupIndex);
            Color baseColor = pickupDef.baseColor;
            Color darkColor = pickupDef.darkColor;
            Image[] array = panel.panelInstanceController.coloredImages;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].color = new Color((array[i].color.r / baseColor.r), (array[i].color.g / baseColor.g), (array[i].color.b / baseColor.b));
            }
            array = panel.panelInstanceController.darkColoredImages;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].color = new Color((array[i].color.r / baseColor.r), (array[i].color.g / baseColor.g), (array[i].color.b / baseColor.b));
            }
        }
        */
    }
}
