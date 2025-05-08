using BepInEx;
using System.IO;
using System.Reflection;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System;
using OculusSampleFramework;
using UnityEngine.UIElements;
using UnityEngine.Animations.Rigging;

namespace GorillaStats
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.ProjectName, PluginInfo.Version)]
    public class Main : BaseUnityPlugin
    {
        public static AssetBundle bundle;
        public GameObject Watch;
        public TextMeshPro watchText;

        private float deltaTime;
        public static string ping;

        private Vector3 lastPosition;
        private float playerSpeed;

        private int playerCount;

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
            watchText.text = "Loading..."; 

            lastPosition = GorillaLocomotion.GTPlayer.Instance.bodyCollider.transform.position;
        }

        void Update()
        {
            if (Watch == null || watchText == null) return;

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
            float fps = 1.0f / deltaTime;

            Vector3 currentPosition = GorillaLocomotion.GTPlayer.Instance.bodyCollider.transform.position;
            playerSpeed = Vector3.Distance(currentPosition, lastPosition) / Time.deltaTime;
            lastPosition = currentPosition;
            
            try
            {
                string fpsColor = GetFPSColor(fps);
                string pingColor = GetPingColor(ping);
                watchText.text = $"<color={fpsColor}>FPS: {Mathf.Round(fps)}</color>\n" + $"<color={pingColor}>PING: {ping}</color>\n" + $"<color=white>SPEED: {playerSpeed:F2}</color>\n" + $"<color=white>TIME: {DateTime.Now:HH:mm:ss}</color>\n" + $"<color=white>PLAYERS: {playerCount}</color>";
            }
            catch (Exception e)
            {
                Debug.LogError($"[GorillaStats]: Error updating watch text: {e}, unity I will do bad things to you");
            }

        }

        private string GetFPSColor(float fps)
        {
            if (fps >= 110f) return "green";
            if (fps >= 50f) return "yellow";
            return "red";
        }

        private string GetPingColor(string pingString)
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

        private void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 100, 200), "==GorillaStats==");
            GUI.Label(new Rect(10, 30, 100, 20), "PING: " + ping);
            GUI.Label(new Rect(10, 50, 100, 20), "PLAYERS: " + playerCount);
            GUI.Label(new Rect(10, 70, 100, 20), "TIME: " + DateTime.Now.ToString("HH:mm"));
            GUI.Label(new Rect(10, 90, 100, 20), $"FPS: {Mathf.Round(1f / deltaTime)}");
            GUI.Label(new Rect(10, 110, 100, 20), $"SPEED: {playerSpeed:F2}");
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
