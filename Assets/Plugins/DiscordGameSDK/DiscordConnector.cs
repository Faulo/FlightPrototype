using UnityEngine;

namespace DiscordGameSDK {
    public class DiscordConnector : MonoBehaviour {
        [SerializeField]
        long clientId = 0;
        [SerializeField]
        Discord.CreateFlags createFlags = Discord.CreateFlags.Default;

        Discord.Discord discord;
        void Start() {
            discord = new Discord.Discord(clientId, (ulong)createFlags);
            discord.GetActivityManager()?.UpdateActivity(
                new Discord.Activity {
                    Type = Discord.ActivityType.Playing,
                },
                _ => { }
            );
        }
        void OnDisable() {
            discord?.GetActivityManager()?.ClearActivity(_ => { });
        }
        void Update() {
            discord?.RunCallbacks();
        }
    }
}