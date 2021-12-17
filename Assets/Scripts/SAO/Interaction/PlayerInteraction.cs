using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace SAO.Interaction
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PlayerInteraction : UdonSharpBehaviour
    {
        //public PlayerTracker playerTracker;
        public BoxCollider boxCollider;
        
        // [Header("Main References")]
        // public GameObject desktopHud;
        // public GameObject vrHud;
        // public Transform vrHeadTracker;

        [Header("HUD References")] public TextMeshProUGUI nameText;
        public TextMeshProUGUI timeText;
        public TextMeshProUGUI dateText;

        [Header("Health References")] public float maxHealth = 250f;
        float health;
        public Image mask;
        public Image healthBar;

        public Gradient healthGradient;
        public TextMeshProUGUI currentHealthText;

        [Header("HP AudioSource")] public AudioSource audioSource;
        public AudioClip normalDamageSound;
        public AudioClip criticalDamageSound;
        public AudioClip healSound;

        public void _SetupInteractionUI()
        {
            startHealth = maxHealth;

            nameText.text = Networking.LocalPlayer.displayName;
            dateText.text = DateTime.Now.ToString("yyyy/MM/dd");
            Destroy(boxCollider);
        }

        private float timer;
        private float targetTime = 1f;
        private float startHealth;

        private void Update()
        {
            timeText.text = DateTime.Now.ToString("HH : mm");

            timer += Time.deltaTime / targetTime;
            mask.fillAmount = Mathf.Lerp(startHealth / maxHealth, health / maxHealth, timer);

            healthBar.color = healthGradient.Evaluate(mask.fillAmount);
            currentHealthText.text = (mask.fillAmount * maxHealth).ToString("F0") + " /";
        }

        public void _HealTest()
        {
            Heal(50f);
        }

        public void _DamageTest()
        {
            TakeDamage(50f);
        }

        private void TakeDamage(float damage)
        {
            _SetHealth(health - damage, true);
        }

        private void Heal(float amount)
        {
            _SetHealth(health + amount, true);
        }

        public void _SetHealth(float value, bool notify)
        {
            startHealth = health;
            timer = 0f;
            if (notify)
            {
                if (value <= health)
                {
                    audioSource.PlayOneShot((value < maxHealth * 0.2f) ? criticalDamageSound : normalDamageSound);
                    //audioSource.Play();
                }
                else
                {
                    audioSource.PlayOneShot(healSound);
                }
            }

            health = Mathf.Clamp(value, 0, maxHealth);
        }

        public override void PostLateUpdate()
        {
            // if (localPlayer.IsUserInVR())
            // {
            //     VRCPlayerApi.TrackingData trackingData = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            //
            //     vrHeadTracker.position = trackingData.position;
            //     vrHeadTracker.rotation = Quaternion.Lerp(vrHeadTracker.rotation, trackingData.rotation, Time.deltaTime*2);
            // }
        }
    }
}
