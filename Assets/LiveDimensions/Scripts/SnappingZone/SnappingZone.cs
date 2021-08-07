
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace LiveDimensions.SnappingZone
{
    [RequireComponent(typeof(Collider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class SnappingZone : UdonSharpBehaviour
    {
        [HideInInspector]
        public Vector3 customSnapPosition = Vector3.zero; //Local position
        [HideInInspector]
        public Quaternion customSnapRotation = Quaternion.identity;

        [HideInInspector]
        public bool customSnapLocation;
        [HideInInspector]
        public bool lastCustomSnapLocation;

        public void OnTriggerEnter(Collider other)
        {
            if (other == null) return;
            if (!Utilities.IsValid(other)) return;

            VRC_Pickup pickup = (VRC_Pickup)other.GetComponent(typeof(VRC_Pickup));
            if (pickup && Networking.LocalPlayer.IsOwner(pickup.gameObject))
            {
                pickup.Drop();
                if (customSnapLocation)
                    pickup.transform.SetPositionAndRotation(transform.TransformPoint(customSnapPosition), customSnapRotation);
                else
                    pickup.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }
        }
    }
}
