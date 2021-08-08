
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

        VRC_Pickup currentPickup;

        private void OnTriggerEnter(Collider other)
        {
            VRC_Pickup pickup = (VRC_Pickup)other.GetComponent(typeof(VRC_Pickup));

            if (pickup)
            {
                if (!currentPickup)
                {
                    currentPickup = pickup;
                    StartSnap();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            VRC_Pickup pickup = (VRC_Pickup)other.GetComponent(typeof(VRC_Pickup));

            if (pickup)
            {
                if (pickup.Equals(currentPickup))
                {
                    EndSnap();
                }
            }
        }

        void StartSnap()
        {
            currentPickup.Drop();

            currentPickup.transform.SetPositionAndRotation(GetSnapPosition(), GetSnapRotation());

            currentPickup.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }

        void EndSnap()
        {
            currentPickup = null;
        }

        Vector3 GetSnapPosition()
        {
            return customSnapLocation ? transform.TransformPoint(customSnapPosition) : transform.position;
        }

        Quaternion GetSnapRotation()
        {
            return customSnapLocation ? customSnapRotation : transform.rotation;
        }
    }
}
