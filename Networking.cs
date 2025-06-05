using System;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;

namespace GorillaStats
{
    public class Networking : MonoBehaviour
    {
        private static List<GameObject> watches = new List<GameObject>();
        private void Awake()
        {
            NetworkSystem.Instance.OnPlayerJoined += OnJoin;
            NetworkSystem.Instance.OnPlayerLeft += OnLeft;
        }

        static void OnJoin(NetPlayer player)
        {
            if (player.GetPlayerRef().CustomProperties.ContainsKey("GorillaStats"))
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig.OwningNetPlayer == player)
                    {
                        if (vrrig == null)
                        {
                            continue;
                        }
                        if (vrrig.isLocal) continue;
                        watches.Add(OtherWatch(vrrig.rightHandTransform, player.UserId));
                    }
                }
            }
        }

        static void OnLeft(NetPlayer player)
        {
            foreach (GameObject watch in watches)
            {
                if (player.UserId == watch.name)
                {
                    Destroy(watch);
                    watches.Remove(watch);
                    break;
                }
            }
        }

        static GameObject OtherWatch(Transform handTransform, string name)
        {
            GameObject theWatch = Instantiate(Main.bundle.LoadAsset<GameObject>("Watch"));
            theWatch.transform.SetParent(handTransform, false);
            theWatch.transform.localPosition = new Vector3(0.06f, 0.02f, 0f);
            theWatch.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
            theWatch.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            theWatch.name = name;

            TextMeshPro textMesh = theWatch.GetComponentInChildren<TextMeshPro>();
            textMesh.text = "Stop Snooping";
            textMesh.font = GorillaTagger.Instance.offlineVRRig.playerText1.font;
            return theWatch;
        }
    }
}