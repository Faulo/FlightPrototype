using System;
using UnityEngine;

namespace DiscordGameSDK {
    public class DiscordConnector : MonoBehaviour {
        [Serializable]
        struct ActivityAssets {
            [SerializeField]
            public string largeImage;
            [SerializeField]
            public string largeText;
        }
        [SerializeField]
        long clientId = 0;
        [SerializeField]
        Discord.CreateFlags createFlags = Discord.CreateFlags.Default;
        [SerializeField]
        ActivityAssets assets = default;

        Discord.Discord discord;
        void Start() {
            try {
                discord = new Discord.Discord(clientId, (ulong)createFlags);
                discord.GetActivityManager()?.UpdateActivity(
                    new Discord.Activity {
                        Type = Discord.ActivityType.Playing,
                        Assets = new Discord.ActivityAssets {
                            LargeImage = assets.largeImage,
                            LargeText = assets.largeText,
                        },
                    },
                    _ => { }
                );
            } catch (Exception) {
                discord = null;
            }
        }
        void OnDisable() {
            try {
                discord?.GetActivityManager()?.ClearActivity(_ => { });
            } catch (Exception) {
                discord = null;
            }
        }
        void Update() {
            try {
                discord?.RunCallbacks();
            } catch (Exception) {
                discord = null;
            }
        }
    }
}