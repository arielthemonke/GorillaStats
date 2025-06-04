using System.Collections;
using UnityEngine;

namespace GorillaStats
{
    public class NotifLib : MonoBehaviour
    {
        public static bool shouldReturn;
        
        public static NotifLib instance;

        void Awake()
        {
            instance = this;
        }
        
        public void SendNotification(string msg, float duration)
        {
            StartCoroutine(SendMsg(msg, duration));
        }

        static IEnumerator SendMsg(string msg, float duration)
        {
            GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
            shouldReturn = true;
            Main.instance.watchText.text = msg;
            yield return new WaitForSeconds(duration);
            shouldReturn = false;
        }
    }
}