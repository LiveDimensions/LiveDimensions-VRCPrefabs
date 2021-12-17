
using Cinemachine;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace LiveDimensions.ThirdPersonCamera
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ThirdPersonCamera : UdonSharpBehaviour
    {
        //Start in third person
        [SerializeField] public bool thirdPersonEnabled = false;
        
        //KeyCodes
        [SerializeField] public KeyCode enableThirdPersonKey = KeyCode.H;
        [SerializeField] public KeyCode leftShoulderKey = KeyCode.Q;
        [SerializeField] public KeyCode rightShoulderKey = KeyCode.E;
        [SerializeField] public KeyCode backViewKey = KeyCode.J;
        [SerializeField] public KeyCode frontViewKey = KeyCode.U;


        [SerializeField] private Camera thirdPersonCamera;

        [SerializeField] private Transform playerHead;

        [SerializeField] private Transform cmTarget;
        [SerializeField] private Transform cm180Target;

        //Cameras
        [SerializeField] private CinemachineVirtualCamera backCamera;
        [SerializeField] private CinemachineVirtualCamera frontCamera;
        [SerializeField] private CinemachineVirtualCamera leftShoulder;
        [SerializeField] private CinemachineVirtualCamera rightShoulder;
        

        VRCPlayerApi localPlayer;

        private void Start()
        {
            localPlayer = Networking.LocalPlayer;

            if (localPlayer.IsUserInVR())
            {
                Destroy(gameObject);
            }

            thirdPersonCamera.enabled = thirdPersonEnabled;

            CalibrateThirdPersonCamera();

            _Update();
        }

        public override void PostLateUpdate()
        {
            if (!thirdPersonEnabled || !Utilities.IsValid(localPlayer)) return;

            VRCPlayerApi.TrackingData headData = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            playerHead.SetPositionAndRotation(headData.position, headData.rotation);
        }

        public void _Update()
        {
            SendCustomEventDelayedFrames(nameof(_Update), 1);

            if (Input.GetKeyDown(enableThirdPersonKey))
            {
                thirdPersonEnabled = !thirdPersonEnabled;

                thirdPersonCamera.enabled = thirdPersonEnabled;

                if (thirdPersonEnabled)
                {
                    CalibrateThirdPersonCamera();
                }

            }

            if (!thirdPersonEnabled) return;

            if (frontCamera.enabled)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll < 0 && cm180Target.localPosition.z < 50f)
                {
                    //Zoom out
                    cm180Target.localPosition += new Vector3(0, 0, -scroll * 80f * Time.deltaTime);

                } else if(scroll > 0 && cm180Target.localPosition.z > 0.1f)
                {
                    //Zoom in
                    cm180Target.localPosition += new Vector3(0, 0, -scroll * 80f * Time.deltaTime);
                }
            }

            //if(frontCamera.enabled) cm180Target.localPosition += (cm180Target.localPosition.z > -1.75f && cm180Target.localPosition.z < 50f)?new Vector3(0, 0, 80f * -Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime): Vector3.zero;

            //Front
            if (Input.GetKeyDown(frontViewKey))
            {
                backCamera.enabled = frontCamera.enabled;

                rightShoulder.enabled = false;
                leftShoulder.enabled = false;

                frontCamera.enabled = !frontCamera.enabled;
            }

            //Back
            if (Input.GetKeyDown(backViewKey))
            {
                backCamera.enabled = true;
                frontCamera.enabled = false;
                rightShoulder.enabled = false;
                leftShoulder.enabled = false;
            }

            //Left Shoulder
            if (Input.GetKeyDown(leftShoulderKey))
            {
                backCamera.enabled = leftShoulder.enabled;

                rightShoulder.enabled = false;
                frontCamera.enabled = false;

                leftShoulder.enabled = !leftShoulder.enabled;
            }

            //Right Shoulder
            if (Input.GetKeyDown(rightShoulderKey))
            {
                backCamera.enabled = rightShoulder.enabled;

                leftShoulder.enabled = false;
                frontCamera.enabled = false;

                rightShoulder.enabled = !rightShoulder.enabled;
            }

        }

        private void CalibrateThirdPersonCamera()
        {
            VRCPlayerApi.TrackingData headData = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

            cmTarget.localPosition = new Vector3(0, -(headData.position.y - localPlayer.GetBonePosition(HumanBodyBones.Neck).y), -(headData.position.y - localPlayer.GetPosition().y) * 0.5f);
            cm180Target.localPosition = new Vector3(0, 0, (headData.position.y - localPlayer.GetPosition().y));
        }
    }

}

