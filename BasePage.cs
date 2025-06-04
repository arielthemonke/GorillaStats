using System;
using UnityEngine;

namespace GorillaStats
{
    public class BasePage : IGorillaStatsPage
    {
        string IGorillaStatsPage.PageName => "BasePage";

        string IGorillaStatsPage.GetPageText()
        {
            string fpsColor = Main.instance.GetFPSColor(Main.instance.fps);
            string pingColor = Main.instance.GetPingColor(Main.instance.ping);
            string text = $"<color={fpsColor}>FPS: {Mathf.Round(Main.instance.fps)}</color>\n" +
                             $"<color={pingColor}>PING: {Main.instance.ping}</color>\n" +
                             $"<color=white>SPEED: {Main.instance.playerSpeed:F2}</color>\n" +
                             $"<color=white>TIME: {DateTime.Now:HH:mm:ss}</color>\n" +
                             $"<color=white>PLAYERS: {Main.instance.playerCount}</color>";
            return text;
        }
    }
}