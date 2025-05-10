using BepInEx;
using System.IO;
using System.Reflection;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace GorillaStats
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.ProjectName, PluginInfo.Version)]
    public class Main : BaseUnityPlugin
    {
        public static AssetBundle bundle;
        public GameObject Watch;
        public TextMeshPro watchText;

        
        
        private List<GorillaStatsPlugin> loadedMods = new List<GorillaStatsPlugin>();
        private bool wasPressed;

        private int CurrentModNumba = -1;

        void Start()
        {
            GorillaTagger.OnPlayerSpawned(Init);
        }

        void Init()
        {
            bundle = LoadAssetBundle("GorillaStats.watch.watch"); // Why did I make this the path lol
            Watch = Instantiate(bundle.LoadAsset<GameObject>("Watch"));

            Watch.transform.SetParent(GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.transform);
            Watch.transform.localPosition = new Vector3(-0.03f, 0f, -0.07f);
            Watch.transform.localRotation = Quaternion.Euler(325f, 0f, 90f);
            Watch.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

            watchText = Watch.GetComponentInChildren<TextMeshPro>();
            watchText.font = GorillaTagger.Instance.offlineVRRig.playerText1.font; // thanks hansolo1000falcon!
            watchText.text = "Loading...\nPress right primary to continue..."; 

            LoadDaMods();
        }

        void Update()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton && !wasPressed || Keyboard.current.lKey.wasPressedThisFrame)
            {
                CurrentModNumba++;
                if (CurrentModNumba > loadedMods.Count - 1)
                {
                    CurrentModNumba = -1;
                }

                foreach (var mod in loadedMods) mod.OnDisable();
                if (CurrentModNumba >= 0)
                {
                    loadedMods[CurrentModNumba].OnEnable();
                }
            }
            wasPressed = ControllerInputPoller.instance.rightControllerSecondaryButton;
            if (CurrentModNumba >= 0 && CurrentModNumba < loadedMods.Count)
            {
                loadedMods[CurrentModNumba].Forever();
                watchText.text = loadedMods[CurrentModNumba].TextToDisplay;
            }
        }
        
        private void LoadDaMods()
        {
            var baseType = typeof(GorillaStatsPlugin);
            var modTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t != baseType && baseType.IsAssignableFrom(t));

            foreach (var type in modTypes)
            {
                if (Activator.CreateInstance(type) is GorillaStatsPlugin mod)
                {
                    loadedMods.Add(mod);
                }
            }
        }


        public AssetBundle LoadAssetBundle(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }
    }
}
