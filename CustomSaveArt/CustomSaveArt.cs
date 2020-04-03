using System.Reflection;
using Modding;
using UnityEngine;
using ModCommon.Util;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;

namespace CustomSaveArt
{
    public class CustomSaveArt : Mod
    {
        internal static CustomSaveArt Instance;

        static int CUSTOM_MAPZONE = (int)GlobalEnums.MapZone.WHITE_PALACE;

        public override string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public override void Initialize()
        {
            Log("Initializing");
            Instance = this;

            On.UnityEngine.UI.SaveSlotButton.PresentSaveSlot += OnSaveSlotButtonPresentSaveSlot;

            //GameManager.instance.StartCoroutine(AddCustomAreaBackground());
            Log("Initialized");
        }

        private void OnSaveSlotButtonPresentSaveSlot(On.UnityEngine.UI.SaveSlotButton.orig_PresentSaveSlot orig, UnityEngine.UI.SaveSlotButton self, SaveStats saveStats)
        {
            Log("!OnSaveSlotButtonPresentSaveSlot");

            SaveSlotBackgrounds ssbg = self.saveSlots;
            Sprite cabgSprite = null;

            #region Load the Custom Area Background Sprite
            Assembly _asm = Assembly.GetExecutingAssembly();
            using (Stream s = _asm.GetManifestResourceStream("CustomSaveArt.Resources.CustomAreaArt.png"))
            {
                if (s != null)
                {
                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);
                    s.Dispose();

                    //Create texture from bytes
                    var tex = new Texture2D(2, 2);

                    tex.LoadImage(buffer, true);

                    // Create sprite from texture
                    cabgSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
            }
            #endregion
            if (cabgSprite != null)
            {
                if ((ssbg.areaBackgrounds[0].areaName != (GlobalEnums.MapZone)CUSTOM_MAPZONE) || !(ssbg.areaBackgrounds[0].backgroundImage.Equals(cabgSprite)))
                {
                    AreaBackground[] customAreaBackgrounds = new AreaBackground[ssbg.areaBackgrounds.Length + 1];
                    ssbg.areaBackgrounds.CopyTo(customAreaBackgrounds, 1);
                    customAreaBackgrounds[0] = new AreaBackground();
                    customAreaBackgrounds[0].areaName = (GlobalEnums.MapZone)CUSTOM_MAPZONE;
                    customAreaBackgrounds[0].backgroundImage = cabgSprite;
                    ssbg.areaBackgrounds = customAreaBackgrounds;
                }
            }
            Log("~OnSaveSlotButtonPresentSaveSlot");

            orig(self, saveStats);

            On.UnityEngine.UI.SaveSlotButton.PresentSaveSlot -= OnSaveSlotButtonPresentSaveSlot;
        }
    }
}
