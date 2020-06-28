using System.Reflection;
using Modding;
using UnityEngine;
using ModCommon.Util;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace CustomSaveArt
{
    public class CustomSaveArt : Mod
    {
        internal static CustomSaveArt Instance;

        static int CUSTOM_MAPZONE = (int)GlobalEnums.MapZone.WHITE_PALACE;

        private Sprite cabgSprite;

        // Thx to 56
        public override string GetVersion()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            string ver = asm.GetName().Version.ToString();

            SHA1 sha1 = SHA1.Create();
            FileStream stream = File.OpenRead(asm.Location);

            byte[] hashBytes = sha1.ComputeHash(stream);

            string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            stream.Close();
            sha1.Clear();

            string ret = $"{ver}-{hash.Substring(0, 6)}";

            return ret;
        }

        public override void Initialize()
        {
            Log("Initializing");
            Instance = this;

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

            On.UnityEngine.UI.SaveSlotButton.PresentSaveSlot += OnSaveSlotButtonPresentSaveSlot;

            //GameManager.instance.StartCoroutine(AddCustomAreaBackground());
            Log("Initialized");
        }

        private void OnSaveSlotButtonPresentSaveSlot(On.UnityEngine.UI.SaveSlotButton.orig_PresentSaveSlot orig, UnityEngine.UI.SaveSlotButton self, SaveStats saveStats)
        {

            SaveSlotBackgrounds ssbg = self.saveSlots;

            if (cabgSprite != null)
            {
                bool present = false;
                foreach (var ab in ssbg.areaBackgrounds)
                {
                    present = present || ((ab.areaName != (GlobalEnums.MapZone)CUSTOM_MAPZONE) || !(ab.backgroundImage.Equals(cabgSprite)));
                }
                if (present)
                {
                    AreaBackground[] customAreaBackgrounds = new AreaBackground[ssbg.areaBackgrounds.Length + 1];
                    ssbg.areaBackgrounds.CopyTo(customAreaBackgrounds, 1);
                    customAreaBackgrounds[0] = new AreaBackground
                    {
                        areaName = (GlobalEnums.MapZone)CUSTOM_MAPZONE,
                        backgroundImage = cabgSprite
                    };
                    ssbg.areaBackgrounds = customAreaBackgrounds;
                }
            }

            orig(self, saveStats);
        }
    }
}
