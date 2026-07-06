using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam04
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smooth = 12f;
        private Vector3 localOffset;
        private Quaternion loaclRotation;

        void Awake()
        {
            localOffset = target.InverseTransformPoint(transform.position);
            loaclRotation = Quaternion.Inverse(target.rotation) * transform.rotation;
        }

        void LateUpdate()
        {
            var targetPos = target.TransformPoint(localOffset);
            var targetRot = target.rotation * loaclRotation;

            transform.position = Vector3.Lerp(transform.position, targetPos, smooth * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, smooth * Time.deltaTime);
        }
    }
}

