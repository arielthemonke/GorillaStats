using BepInEx;
using System.IO;
using System.Reflection;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine.InputSystem;

namespace GorillaStats
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.ProjectName, PluginInfo.Version)]
    public class Main : BaseUnityPlugin
    {
        public static AssetBundle bundle;
        public GameObject Watch;
        public GameObject Cube;
        public GameObject Screen;
        public GameObject Cylinder;
        public TextMeshPro watchText;

        private float deltaTime;
        public string ping;
        public float fps;

        private Vector3 lastPosition;
        public float playerSpeed;

        public int playerCount;

        private ConfigFile cfg = new ConfigFile(Path.Combine(Paths.ConfigPath, "GorillaStats.cfg"), true);
        private ConfigEntry<string> watchColour;
        private ConfigEntry<string> screenColour;
        private bool wasPressed;

        private int currentPageIndex = 0;

        public event Action OnWatchSpawned;

        public static Main instance;
        
        private GameObject notifManager;
        private GameObject Networking;

        void Awake()
        {
            instance = this;
            GorillaTagger.OnPlayerSpawned(Init);
            watchColour = cfg.Bind("Colours","Cube Colour", "#FFFFFF", "Watch colour hex code");
            screenColour = cfg.Bind("Colours", "Screen Color", "#00631b", "Colour of the watch's screen in hex");
        }

        void Init()
        {
            bundle = LoadAssetBundle("GorillaStats.watch.watch"); // Why did I make this the path lol
            Watch = Instantiate(bundle.LoadAsset<GameObject>("Watch"));
            
            Cube = Watch.transform.GetChild(0).gameObject;
            Screen = Watch.transform.GetChild(1).gameObject;
            Cylinder = Watch.transform.GetChild(2).gameObject;
            Cube.GetComponent<MeshRenderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            Cube.GetComponent<MeshRenderer>().material.color = colourFromString(watchColour.Value, Color.white);
            Screen.GetComponent<MeshRenderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            Screen.GetComponent<MeshRenderer>().material.color = colourFromString(screenColour.Value, colourFromString("#00631b", Color.black));
            Cylinder.GetComponent<MeshRenderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            Cylinder.GetComponent<MeshRenderer>().material.color = colourFromString(watchColour.Value, Color.white);
            Watch.transform.SetParent(GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.transform);
            Watch.transform.localPosition = new Vector3(-0.03f, 0f, -0.07f);
            Watch.transform.localRotation = Quaternion.Euler(325f, 0f, 90f);
            Watch.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

            watchText = Watch.GetComponentInChildren<TextMeshPro>();
            watchText.font = GorillaTagger.Instance.offlineVRRig.playerText1.font; // thanks hansolo1000falcon!
            watchText.text = "Loading...";
            

            lastPosition = GorillaLocomotion.GTPlayer.Instance.bodyCollider.transform.position;
            
            
            GorillaStatsPageManager.RegisterPage(new BasePage());

            notifManager = new GameObject("GorillaStatsNotifManager");
            notifManager.AddComponent<NotifLib>();
            
            Networking = new GameObject("GorillaStatsNetworking");
            Networking.AddComponent<Networking>();
            
            OnWatchSpawned?.Invoke();
            
            AddProps();
        }

        void AddProps()
        {
            var pageNames = GorillaStatsPageManager.GorillaStatsPages.Select(p => p.PageName).ToArray();
            
            string allPPages = string.Join(", ", pageNames);
            
            
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash.Add("GorillaStats", allPPages);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        void Update()
        {
            if (NotifLib.shouldReturn) return;
            if (Watch == null || watchText == null) return;

            if (ControllerInputPoller.instance.rightControllerSecondaryButton && !wasPressed || Keyboard.current.nKey.wasPressedThisFrame)
            {
                currentPageIndex++;
                if (currentPageIndex >= GorillaStatsPageManager.GorillaStatsPages.Count)
                {
                    currentPageIndex = 0;
                }
            }
            try
            {
                if (GorillaStatsPageManager.GorillaStatsPages.Count > 0)
                {
                    var page = GorillaStatsPageManager.GorillaStatsPages[currentPageIndex];
                    watchText.text = page.GetPageText();
                }
                
                if (PhotonNetwork.InRoom)
                {
                    ping = PhotonNetwork.GetPing().ToString();
                    playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
                }
                else
                {
                    ping = "N/A";
                    playerCount = 0;
                }

                deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
                fps = 1.0f / deltaTime;

                Vector3 currentPosition = GorillaLocomotion.GTPlayer.Instance.bodyCollider.transform.position;
                playerSpeed = Vector3.Distance(currentPosition, lastPosition) / Time.deltaTime;
                lastPosition = currentPosition;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            
            wasPressed = ControllerInputPoller.instance.rightControllerSecondaryButton;
        }

        public string GetFPSColor(float fps)
        {
            if (fps >= 110f) return "green";
            if (fps >= 50f) return "yellow";
            return "red";
        }

        public string GetPingColor(string pingString)
        {
            if (float.TryParse(pingString, out float ping))
            {
                if (ping <= 50f) return "green";
                if (ping <= 100f) return "yellow";
                return "red";
            }
            else
            {
                return "white";
            }
        }

        private Rect statsWindow = new Rect(100, 100, 100, 160);

        private void OnGUI()
        {
            statsWindow = GUI.Window(100, statsWindow, DrawStatsWindow, "==GorillaStats==");
        }

        private void DrawStatsWindow(int windowID)
        {
            GUILayout.BeginVertical();

            GUILayout.Label($"PING: {ping}");
            GUILayout.Label($"PLAYERS: {playerCount}");
            GUILayout.Label($"TIME: {DateTime.Now:HH:mm}");
            GUILayout.Label($"FPS: {Mathf.RoundToInt(1f / deltaTime)}");
            GUILayout.Label($"SPEED: {playerSpeed:F2}");

            GUILayout.EndVertical();

            GUI.DragWindow();
        }


        public AssetBundle LoadAssetBundle(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }

        Color colourFromString(string colour, Color fallback)
        {
            if (ColorUtility.TryParseHtmlString(colour, out Color colourResult))
            {
                return colourResult;
            }
            return fallback;
        }
    }
}