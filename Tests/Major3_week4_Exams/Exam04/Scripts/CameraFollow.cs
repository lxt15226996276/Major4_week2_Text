using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam04
{
    public class CameraFollow : MonoBehaviour
    {
        // Inspector 分组标题，方便在编辑器里区分字段用途
        [Header("跟随目标")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 localOffset;
        [SerializeField] private Quaternion localRotation;
        [SerializeField] private float smoothSpeed = 8f;

        void Awake()
        {
            // 若忘记在 Inspector 指定 target，无法计算偏移，直接返回，避免下面访问 target 时空引用报错
            if (target == null) return;

            // 结果 localOffset 表示：相对主角，相机在右/上/前各偏多少——主角怎么动，这个相对关系不变
            localOffset = target.InverseTransformPoint(transform.position);

            // 这样主角 A/D 转身时，相机不仅位置绕圈，朝向也会跟着转，始终维持开局时的观察角度
            localRotation = Quaternion.Inverse(target.rotation) * transform.rotation;
        }

        /// <summary>
        /// 主角在 Update 里移动/旋转，相机若也在 Update 里跟随，可能读到「半帧」状态导致轻微抖动；
        /// </summary>
        void LateUpdate()
        {
            if (target == null) return;
            Vector3 targetPos = target.TransformPoint(localOffset);
            Quaternion targetRotation = target.rotation * localRotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
        }
    }

}
