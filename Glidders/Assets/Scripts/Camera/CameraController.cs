using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Glidders
{
    public class CameraController : MonoBehaviour
    {

        [SerializeField] private CinemachineTargetGroup cinemachineTargetGroup;
        [SerializeField] private Transform cursorTransform;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AddTarget(Transform targetTransform)
        {
            cinemachineTargetGroup.AddMember(targetTransform, 1, 1);
        }

        public void RemoveTarget(Transform targetTransform)
        {
            cinemachineTargetGroup.RemoveMember(targetTransform);
        }

        public void AddCarsor()
        {
            cinemachineTargetGroup.AddMember(cursorTransform, 1, 1);
        }

        public void RemoveCarsor()
        {
            cinemachineTargetGroup.RemoveMember(cursorTransform);
        }
    }
}

