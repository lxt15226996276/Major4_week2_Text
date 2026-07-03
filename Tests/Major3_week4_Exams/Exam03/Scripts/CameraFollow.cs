using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam03
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
            // 因此 targetPos 就是「本帧相机应该在的世界位置」
            Vector3 targetPos = target.TransformPoint(localOffset);

            // 主角当前世界旋转 × 开局记录的相对旋转 = 本帧相机应有的世界旋转
            Quaternion targetRotation = target.rotation * localRotation;
            // 先改 rotation 再改 position：两者独立，顺序对视觉效果影响很小，习惯上先朝向再位移亦可
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);

            // Lerp：直线插值位置，从当前相机位置向 targetPos 靠近，产生「平滑跟随」而非瞬移
            // 若 smoothSpeed 很大或设为直接赋值 transform.position = targetPos，则变成硬跟随、无缓冲
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
        }
    }
}

