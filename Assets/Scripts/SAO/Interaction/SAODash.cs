
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class SAODash : UdonSharpBehaviour
{
    [Header("Dash Settings")]
    public float dashDistance = 1f;
    public float dashDuration = 0.25f;
    public float dashExitTime = 0.25f;
    public AnimationCurve speedCurve;
    public float softCooldown = 0.5f;
    public float cooldown = 1.5f;
    public int maxConsecutiveDashes = 2;
    //float dashVeloctiy; //Calculated, v = d/t 

    [Header("Dash Activation")]
    [Tooltip("Double tap, right joystick or button for dashing?")]
    public bool doubleTapDash;
    public bool buttonDash;
    public bool rightAxisDash;

    [Header("Dash Button")]
    public KeyCode desktopKey = KeyCode.LeftShift;
    public string vrButton = "Oculus_CrossPlatform_SecondaryThumbstick"; //https://docs.google.com/spreadsheets/d/1_iF0NjJniTnQn-knCjb5nLh6rlLfW_QKM19wtSW_S9w/edit#gid=1150012376

    [Header("Dash Orientation")]
    [Tooltip("Use joystick, head or hand orientation?")]
    public bool joystickPoint = true;
    public VRCPlayerApi.TrackingDataType trackingType = VRCPlayerApi.TrackingDataType.Head;


    [Header("References")]
    public AudioSource audioSource;
    public AudioClip dashSound;

    VRCPlayerApi localPlayer;

    //States
    bool dashing;               //While dashing
    bool exitingDash;           //While exiting dash (can trigger a new dash here)
    bool onSoftCooldown;        //Time period where you can dash consecutively if haven't reached max consec dashes.
    bool onCooldown;            //Can't dash
    int consecutiveDashes = 0;  //Counter of consecutive dashes

    //Timers
    float dashExitTimer = 0f;

    //Vector3 lastVelocity;

    //Input stuff
    Vector2 input = new Vector2(0, 0);
    float rightVerticalAxis = 0f;
    float joystickAngle = 0f;
    float lastJoystickAngle = 0f;

    bool rightAxisDown;

    //Double tap things
    bool lastInput;
    int tapCount;
    float lastTap;
    float maxTapFrequency = 0.5f;
    int maxTaps = 2;

    void Start()
    {
        localPlayer = Networking.LocalPlayer;
        audioSource.clip = dashSound;

        //dashVeloctiy = dashLength / dashDuration;

        SendCustomEventDelayedSeconds(nameof(_LateStart), 1f);
    }

    public void _LateStart()
    {
        if (localPlayer.IsUserInVR())
        {
            rightAxisDash = false;
        }
    }

    public void Update()
    {
        DashCheck();

        if (dashing || exitingDash)
        {
            //Use joystick, hand or head orientation.
            //TODO figure out the tracking data offset for hands
            float angle = joystickPoint ? GetJoystickAngle() : localPlayer.GetTrackingData(trackingType).rotation.eulerAngles.y;

            //If you are dashing and you release Joystick/WASD, keep going to your original angle, not forward.
            if (joystickPoint && input.sqrMagnitude < 0.125f)
                angle = lastJoystickAngle;

            //Dash direction.
            Vector3 normal = (Quaternion.Euler(0, angle, 0) * localPlayer.GetRotation() * Vector3.forward).normalized;

            if (!exitingDash)
            {
                //Main dash
                //TODO: dashDistance/dashDuration can be calculated in Start, but for runtime dash values testing it is like this rn.
                localPlayer.TeleportTo(localPlayer.GetPosition() + (normal * (dashDistance / dashDuration) * Time.deltaTime), localPlayer.GetRotation(), VRC_SceneDescriptor.SpawnOrientation.Default, true);
            }
            else
            {
                //Slow down to exit dash
                dashExitTimer += Time.deltaTime;
                localPlayer.TeleportTo(localPlayer.GetPosition() + (normal * (dashDistance / dashDuration) * Time.deltaTime * speedCurve.Evaluate(dashExitTimer / dashExitTime)), localPlayer.GetRotation(), VRC_SceneDescriptor.SpawnOrientation.Default, true);
            }

            lastJoystickAngle = angle;
        }
    }

    void DashCheck()
    {
        if (onCooldown) return;

        //Not checking for !dashing here, because you can buffer a double tap.
        if (doubleTapDash)
        {
            DoubleTapDashCheck();
            return;
        }
        else if (buttonDash)
        {
            ButtonDashCheck();
            return;
        }
        else if (rightAxisDash)
        {
            RightAxisDashCheck();
            return;
        }
    }

    public void _StartDash()
    {
        consecutiveDashes = onSoftCooldown ? consecutiveDashes++ : 0;

        dashing = true;
        audioSource.Play();

        //End dash after duration
        SendCustomEventDelayedSeconds(nameof(_EndDash), dashDuration);
    }

    public void _EndDash()
    {
        dashing = false;

        exitingDash = true;
        SendCustomEventDelayedSeconds(nameof(_ExitDash), dashExitTime);

        //TODO: If spammed do cooldown, else soft cooldown.
        if (consecutiveDashes < maxConsecutiveDashes - 1)
        {
            onSoftCooldown = true;
            SendCustomEventDelayedSeconds(nameof(_EndSoftCooldown), softCooldown);
        }
        else
        {
            onCooldown = true;
            SendCustomEventDelayedSeconds(nameof(_EndCooldown), cooldown);
        }
    }

    public void _EndSoftCooldown()
    {
        onSoftCooldown = false;
    }

    public void _EndCooldown()
    {
        consecutiveDashes = 0;
        onCooldown = false;
    }

    public void _ExitDash()
    {
        dashExitTimer = 0f;
        lastJoystickAngle = 0f;
        exitingDash = false;
    }

    void DoubleTapDashCheck()
    {
        if (input.sqrMagnitude >= 0.7225f)
        {
            if (!lastInput)
            {
                lastTap = Time.time;
                lastInput = true;
                tapCount++;
                if (!dashing && tapCount == 2)
                {
                    _StartDash();
                }
                if (tapCount == maxTaps) tapCount = 0;
            }
        }
        else
        {
            lastInput = false;
            if (Time.time - lastTap > maxTapFrequency) tapCount = 0;
        }
    }

    void RightAxisDashCheck()
    {
        //Flick up or down.
        if (!dashing)
        {
            if (rightVerticalAxis >= 0.85f || rightVerticalAxis <= -0.85f)
            {
                if (!rightAxisDown) _StartDash();
                rightAxisDown = true;
            } else
            {
                rightAxisDown = false;
            }
                
        }
    }

    void ButtonDashCheck()
    {
        if (!dashing && (Input.GetButtonDown(vrButton) || Input.GetKeyDown(desktopKey)))
        {
            _StartDash();
        }
    }

    float GetJoystickAngle()
    {
        return Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
    }

    public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
    {
        input.x = value;
    }

    public override void InputMoveVertical(float value, UdonInputEventArgs args)
    {
        input.y = value;
    }

    public override void InputLookVertical(float value, UdonInputEventArgs args)
    {
        rightVerticalAxis = value;
    }
}
