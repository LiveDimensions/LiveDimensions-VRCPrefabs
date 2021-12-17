using UdonSharp;
using UdonSharp.Examples.Utilities;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace LiveDimensions.Audio.PlayerSettings
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class PlayerAudioOverride : UdonSharpBehaviour
    {
        [Header("Player Audio Settings")]
        [Range(0,24)]
        public float voiceGain = 15; //0 - 24
        public float voiceFar = 25;
        public float voiceNear = 10;
        public float voiceVolumetricRadius = 0;
        public bool voiceDisableLowpass = false;

        [Header("Avatar Audio Settings")]
        [Range(0,10)]
        public float avatarMaxAudioGain = 10;   //0 - 10
        public float avatarMaxFarRadius = 40;
        public float avatarMaxNearRadius = 40;
        public float avatarMaxVolumetricRadius = 40;
        public bool avatarForceSpacialization = false;
        public bool avatarDisableCustomCurve = false;

        public WorldAudioSettings worldAudioSettings;

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (player.isLocal) return;

            //Player voice
            player.SetVoiceGain(voiceGain);
            player.SetVoiceDistanceFar(voiceFar);
            player.SetVoiceDistanceNear(voiceNear);
            player.SetVoiceVolumetricRadius(voiceVolumetricRadius);
            player.SetVoiceLowpass(!voiceDisableLowpass);

            //Avatar audio
            player.SetAvatarAudioGain(avatarMaxAudioGain);
            player.SetAvatarAudioFarRadius(avatarMaxFarRadius);
            player.SetAvatarAudioNearRadius(avatarMaxNearRadius);
            player.SetAvatarAudioVolumetricRadius(avatarMaxVolumetricRadius);
            player.SetAvatarAudioForceSpatial(avatarForceSpacialization);
            player.SetAvatarAudioCustomCurve(!avatarDisableCustomCurve);
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (player.isLocal) return;

            if (worldAudioSettings == null)
            {
                SetDefaultAudioSettings(player);
                return;
            }

            //Player Voice
            player.SetVoiceGain(worldAudioSettings.voiceGain);
            player.SetVoiceDistanceFar(worldAudioSettings.voiceFar);
            player.SetVoiceDistanceNear(worldAudioSettings.voiceNear);
            player.SetVoiceVolumetricRadius(worldAudioSettings.voiceVolumetricRadius);
            player.SetVoiceLowpass(!worldAudioSettings.voiceDisableLowpass);

            //Avatar audio
            player.SetAvatarAudioGain(worldAudioSettings.avatarMaxAudioGain);
            player.SetAvatarAudioFarRadius(worldAudioSettings.avatarMaxFarRadius);
            player.SetAvatarAudioNearRadius(worldAudioSettings.avatarMaxNearRadius);
            player.SetAvatarAudioVolumetricRadius(worldAudioSettings.avatarMaxVolumetricRadius);
            player.SetAvatarAudioForceSpatial(worldAudioSettings.avatarForceSpacialization);
            player.SetAvatarAudioCustomCurve(!worldAudioSettings.avatarDisableCustomCurve);
        }

        private void SetDefaultAudioSettings(VRCPlayerApi player)
        {
            //Player Voice
            player.SetVoiceGain(15);
            player.SetVoiceDistanceFar(25);
            player.SetVoiceDistanceNear(0);
            player.SetVoiceVolumetricRadius(0);
            player.SetVoiceLowpass(true);

            //Avatar audio
            player.SetAvatarAudioGain(10);
            player.SetAvatarAudioFarRadius(40);
            player.SetAvatarAudioNearRadius(40);
            player.SetAvatarAudioVolumetricRadius(40);
            player.SetAvatarAudioForceSpatial(false);
            player.SetAvatarAudioCustomCurve(true);
        }
    }
}

