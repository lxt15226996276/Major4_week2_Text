using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam07
{
    public class CameraFollow : MonoBehaviour
    {
        private Transform player;
        private Vector3 localOffset;
        private Quaternion localRotate;
        private float smoothSpeed = 9.5f;

        void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            localOffset = player.InverseTransformPoint(transform.position);
            localRotate = Quaternion.Inverse(player.rotation) * transform.rotation;
        }
        void LateUpdate()
        {
            if (player == null) return;
            Vector3 targetPos = player.TransformPoint(localOffset);
            Quaternion targetRoation = player.rotation * localRotate;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRoation, smoothSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
        }

    }
}

