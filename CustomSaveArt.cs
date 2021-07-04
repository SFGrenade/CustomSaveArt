using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using GlobalEnums;
using Modding;
using On.UnityEngine.UI;
using UnityEngine;

namespace CustomSaveArt
{
    public class CustomSaveArt : Mod
    {
        internal static CustomSaveArt Instance;

        private static readonly int CUSTOM_MAPZONE = (int) MapZone.WHITE_PALACE;

        private Sprite cabgSprite;

        // Thx to 56
        public override string GetVersion()
        {
            var asm = Assembly.GetExecutingAssembly();

            var ver = asm.GetName().Version.ToString();

            var sha1 = SHA1.Create();
            var stream = File.OpenRead(asm.Location);

            var hashBytes = sha1.ComputeHash(stream);

            var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            stream.Close();
            sha1.Clear();

            var ret = $"{ver}-{hash.Substring(0, 6)}";

            return ret;
        }

        public override void Initialize()
        {
            Log("Initializing");
            Instance = this;

            #region Load the Custom Area Background Sprite

            var _asm = Assembly.GetExecutingAssembly();
            using (var s = _asm.GetManifestResourceStream("CustomSaveArt.Resources.CustomAreaArt.png"))
            {
                if (s != null)
                {
                    var buffer = new byte[s.Length];
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

            SaveSlotButton.PresentSaveSlot += OnSaveSlotButtonPresentSaveSlot;

            //GameManager.instance.StartCoroutine(AddCustomAreaBackground());
            Log("Initialized");
        }

        private void OnSaveSlotButtonPresentSaveSlot(SaveSlotButton.orig_PresentSaveSlot orig,
            UnityEngine.UI.SaveSlotButton self, SaveStats saveStats)
        {
            var ssbg = self.saveSlots;

            if (cabgSprite != null)
            {
                var present = false;
                foreach (var ab in ssbg.areaBackgrounds)
                    present = present || ab.areaName != (MapZone) CUSTOM_MAPZONE ||
                              !ab.backgroundImage.Equals(cabgSprite);
                if (present)
                {
                    var customAreaBackgrounds = new AreaBackground[ssbg.areaBackgrounds.Length + 1];
                    ssbg.areaBackgrounds.CopyTo(customAreaBackgrounds, 1);
                    customAreaBackgrounds[0] = new AreaBackground
                    {
                        areaName = (MapZone) CUSTOM_MAPZONE,
                        backgroundImage = cabgSprite
                    };
                    ssbg.areaBackgrounds = customAreaBackgrounds;
                }
            }

            orig(self, saveStats);
        }
    }
}