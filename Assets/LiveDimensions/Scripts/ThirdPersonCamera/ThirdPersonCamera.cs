
using Cinemachine;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace LiveDimensions.ThirdPersonCamera
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class ThirdPersonCamera : UdonSharpBehaviour
    {
        //Hide in default inspector to show in custom inspector.
        [HideInInspector] public bool thirdPersonEnabled = false;
        [HideInInspector] public KeyCode enableThirdPersonKey = KeyCode.H;
        [HideInInspector] public KeyCode leftShoulderKey = KeyCode.Q;
        [HideInInspector] public KeyCode rightShoulderKey = KeyCode.E;
        [HideInInspector] public KeyCode backViewKey = KeyCode.J;
        [HideInInspector] public KeyCode frontViewKey = KeyCode.U;



        //Show these in default inspector
        [SerializeField] private Camera thirdPersonCamera;
        [SerializeField] private Camera uiCamera;

        [SerializeField] private Transform playerHead;

        [SerializeField] private Transform cmTarget;
        [SerializeField] private Transform cm180Target;

        [SerializeField] private CinemachineVirtualCamera backCamera;

        [SerializeField] private CinemachineVirtualCamera frontCamera;

        [SerializeField] private CinemachineVirtualCamera leftShoulder;

        [SerializeField] private CinemachineVirtualCamera rightShoulder;
        

        VRCPlayerApi localPlayer;
        VRCPlayerApi.TrackingData headTrackingData;

        private void Start()
        {
            localPlayer = Networking.LocalPlayer;

            thirdPersonCamera.enabled = thirdPersonEnabled;
            uiCamera.enabled = thirdPersonEnabled;

            CalibrateThirdPersonCamera();
        }

        public override void PostLateUpdate()
        {
            if (!thirdPersonEnabled) return;

            headTrackingData = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            playerHead.SetPositionAndRotation(headTrackingData.position, headTrackingData.rotation);
        }

        private void Update()
        {

            if (Input.GetKeyDown(enableThirdPersonKey))
            {
                thirdPersonEnabled = !thirdPersonEnabled;

                thirdPersonCamera.enabled = thirdPersonEnabled;
                uiCamera.enabled = thirdPersonEnabled;

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
            headTrackingData = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

            cmTarget.localPosition = new Vector3(0, -(headTrackingData.position.y - localPlayer.GetBonePosition(HumanBodyBones.Neck).y), -(headTrackingData.position.y - localPlayer.GetPosition().y) * 0.5f);
            cm180Target.localPosition = new Vector3(0, 0, (headTrackingData.position.y - localPlayer.GetPosition().y));
        }
    }

}

