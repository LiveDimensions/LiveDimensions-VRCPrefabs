
using Cinemachine;
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class CameraManager : UdonSharpBehaviour
{
    [Header("Virtual Cameras")]
    [SerializeField]
    private CinemachineVirtualCamera[] virtualCameras;

    [SerializeField]
    private Transform cameraMover;

    [SerializeField]
    private VRCStation cameraStation;
    bool inCameraStation;

    private VRCPlayerApi localPlayer;

    [UdonSynced, FieldChangeCallback(nameof(ActiveCamera))]
    private int _activeCamera;
    public int ActiveCamera
    {
        set
        {
            _activeCamera = value;
            Debug.Log(_activeCamera);
            _SwitchToCamera(_activeCamera);
        }
        get => _activeCamera;
    }

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
        cameraMover.transform.SetPositionAndRotation(virtualCameras[ActiveCamera].transform.position, virtualCameras[ActiveCamera].transform.rotation);
    }

    Vector3 targetPosition;
    float horizontalRotTarget;
    float verticalRotTarget;
    Quaternion targetRotation;
    float droneSpeed = 0.1f;
    float rotateSpeed = 0.75f;
    float smoothness = 5f;

    private void Update()
    {
        if (inCameraStation && localPlayer.IsOwner(cameraMover.gameObject))
        {
            if (!localPlayer.IsUserInVR())
            {
                pitch = Convert.ToInt32(Input.GetKey("i")) - Convert.ToInt32(Input.GetKey("k"));
                yaw = Convert.ToInt32(Input.GetKey("l")) - Convert.ToInt32(Input.GetKey("j"));
            }

            targetPosition += cameraMover.right * droneSpeed * sideways;
            targetPosition += cameraMover.forward * droneSpeed * forward;
            cameraMover.position = Vector3.Lerp(cameraMover.position, targetPosition, Time.deltaTime * smoothness);

            verticalRotTarget += pitch * rotateSpeed;
            horizontalRotTarget += yaw * rotateSpeed;
            //targetRotation = Quaternion.Euler(targetRotation.x + (pitch * rotateSpeed), targetRotation.y + (yaw * rotateSpeed), 0);
            targetRotation = Quaternion.Euler(verticalRotTarget, horizontalRotTarget, 0);
            cameraMover.rotation = Quaternion.Lerp(cameraMover.rotation, targetRotation, Time.deltaTime * smoothness);
        }

        virtualCameras[ActiveCamera].transform.SetPositionAndRotation(cameraMover.position, cameraMover.rotation);
    }

    public void _MoveCameras()
    {
        inCameraStation = !inCameraStation;
        if (inCameraStation)
        {
            cameraStation.UseStation(localPlayer);
            cameraMover.SetPositionAndRotation(virtualCameras[ActiveCamera].transform.position, virtualCameras[ActiveCamera].transform.rotation);
            verticalRotTarget = virtualCameras[ActiveCamera].transform.rotation.x;
            horizontalRotTarget = virtualCameras[ActiveCamera].transform.rotation.y;
            targetRotation = virtualCameras[ActiveCamera].transform.rotation;
            targetPosition = virtualCameras[ActiveCamera].transform.position;
        } else
        {
            cameraStation.ExitStation(localPlayer);
        }
    }


    public void _SwitchToCamera(int num)
    {
        if(!(num >= 0 && num < virtualCameras.Length))
        {
            Debug.LogWarning($"[CS] {gameObject.name} Cannot switch to camera {num} as it is out of index!");
        }
        for(int i = 0; i < virtualCameras.Length; i++)
        {
            virtualCameras[i].Priority = num == i ? 1 : 0;
            if (localPlayer.IsOwner(gameObject))
            {
                cameraMover.SetPositionAndRotation(virtualCameras[i].transform.position, virtualCameras[i].transform.rotation);
                verticalRotTarget = virtualCameras[i].transform.rotation.x;
                horizontalRotTarget = virtualCameras[i].transform.rotation.y;
                targetRotation = virtualCameras[i].transform.rotation;
                targetPosition = virtualCameras[i].transform.position;
            }
        }
    }

    public void _NextCamera()
    {
        if (!localPlayer.IsOwner(gameObject))
        {
            Debug.LogWarning($"[CS] {gameObject.name} Cannot change camera as {localPlayer.playerId} is not owner");
            return;
        }
        Debug.Log($"Cameras: {virtualCameras.Length}, active camera: {ActiveCamera}");
        ActiveCamera = (_activeCamera < virtualCameras.Length-1) ? _activeCamera + 1 : 0;
        RequestSerialization();
    }

    public void _PreviousCamera()
    {
        if (!localPlayer.IsOwner(gameObject))
        {
            Debug.LogWarning($"[CS] {gameObject.name} Cannot change camera as {localPlayer.playerId} is not owner");
            return;
        }
        ActiveCamera = (ActiveCamera > 0) ? ActiveCamera-- : virtualCameras.Length - 1;
        RequestSerialization();
    }

    #region STICK_CONTROLS
    //Left Stick
    private float forward;
    private float sideways;

    //Right Stick
    private float pitch;
    private float yaw;

    public override void InputMoveVertical(float value, UdonInputEventArgs args)
    {
        forward = value;
    }

    public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
    {
        sideways = value;
    }

    public override void InputLookVertical(float value, UdonInputEventArgs args)
    {
        if (localPlayer.IsUserInVR()) return;
        pitch = value;
    }

    public override void InputLookHorizontal(float value, UdonInputEventArgs args)
    {
        if (!localPlayer.IsUserInVR()) return;
        yaw = value;
    }
    #endregion


}
