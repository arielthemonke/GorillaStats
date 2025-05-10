using System;
using UnityEngine;
using Photon.Pun;

namespace GorillaStats
{
    public class MainPage : GorillaStatsPlugin
    {
        private float deltaTime;
        public static string ping;

        private Vector3 lastPosition;
        private float playerSpeed;

        private float fps;

        private int playerCount;
        
        private string fpsColor;
        private string pingColor;
        public override void Forever()
        {
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
            
            fpsColor = GetFPSColor(fps);
            pingColor = GetPingColor(ping);
            base.Forever();
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

        public override void OnEnable()
        {
            lastPosition = GorillaLocomotion.GTPlayer.Instance.bodyCollider.transform.position;
            base.OnEnable();
        }

        public override string TextToDisplay => 
            $"<color={fpsColor}>FPS: {Mathf.Round(fps)}</color>\n" +
            $"<color={pingColor}>PING: {ping}</color>\n" +
            $"<color=white>SPEED: {playerSpeed:F2}</color>\n" +
            $"<color=white>TIME: {DateTime.Now:HH:mm:ss}</color>\n" +
            $"<color=white>PLAYERS: {playerCount}</color>";
        
        
        private void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 100, 200), "==GorillaStats==");
            GUI.Label(new Rect(10, 30, 100, 20), "PING: " + ping);
            GUI.Label(new Rect(10, 50, 100, 20), "PLAYERS: " + playerCount);
            GUI.Label(new Rect(10, 70, 100, 20), "TIME: " + DateTime.Now.ToString("HH:mm"));
            GUI.Label(new Rect(10, 90, 100, 20), $"FPS: {Mathf.Round(1f / deltaTime)}");
            GUI.Label(new Rect(10, 110, 100, 20), $"SPEED: {playerSpeed:F2}");
        }
    }
}