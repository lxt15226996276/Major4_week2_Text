using UnityEngine;

namespace Exam.Exam03
{
    /// <summary>
    /// 第三人称相机跟随：相机始终保持在坦克（trank）周围的固定相对位置与朝向，
    /// 并通过插值平滑移动，避免画面生硬抖动。
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        // [SerializeField]：私有字段仍可在 Inspector 面板拖拽赋值，避免 public 暴露过多接口
        // Transform：Unity 中所有物体位置/旋转/缩放的载体；这里指向被跟随的坦克
        [SerializeField] private Transform trank;

        // 相机相对坦克的「局部偏移」——在坦克本地坐标系下，相机位于何处（右/上/前各偏多少）
        // 开局记录一次后不再改变，坦克移动时用这个偏移反算世界坐标，关系始终不变
        private Vector3 localOffest;

        // 相机相对坦克的「局部旋转差」——开局时相机朝向与坦克朝向的差值
        // 坦克转身时，相机朝向 = 坦克当前朝向 × 这个差值，从而一起转、观察角度不变
        private Quaternion localRotation;

        // 平滑系数：越大跟得越快，越小越有「滞后感」；与 Time.deltaTime 相乘得到每帧插值比例
        private float smoothSpeed = 9.5f;

        /// <summary>
        /// Awake：脚本实例加载时、第一帧 Update 之前调用，适合做一次性的初始化计算。
        /// </summary>
        void Awake()
        {
            // InverseTransformPoint：把「世界坐标点」转换到 trank 的本地空间
            // 参数 transform.position = 相机当前世界位置
            // 返回值 localOffest = 该点在 trank 本地系下的坐标，即相对偏移
            localOffest = trank.InverseTransformPoint(transform.position);

            // Quaternion.Inverse(q)：求旋转 q 的逆（反向旋转）
            // trank.rotation * localRotation = 相机世界旋转  =>  localRotation = Inverse(trank.rotation) * 相机旋转
            // 乘法顺序：先应用 trank 的旋转，再叠加 localRotation，得到相机最终朝向
            localRotation = Quaternion.Inverse(trank.rotation) * transform.rotation;
        }

        /// <summary>
        /// LateUpdate：每帧所有 Update 执行完毕后再调用。
        /// 坦克在 Update 里移动/旋转，相机在此跟随，可读到本帧最终状态，减少抖动。
        /// </summary>
        void LateUpdate()
        {
            // 若 Inspector 未绑定 trank，避免空引用异常
            if (trank == null) return;

            // TransformPoint：把 trank 本地坐标 localOffest 变换到世界坐标
            // 坦克移到新位置后，相机目标世界位置 = 坦克位置 + 旋转后的相对偏移
            Vector3 targetPos = trank.TransformPoint(localOffest);

            // trank.rotation：坦克当前世界旋转
            // * localRotation：叠加上开局记录的相对朝向差，得到相机本帧应有的世界旋转
            Quaternion targetRot = trank.rotation * localRotation;

            // Vector3.Lerp(a, b, t)：在 a 与 b 之间线性插值，t∈[0,1] 时结果在两点之间
            // a = 相机当前位置，b = targetPos，t = smoothSpeed * Time.deltaTime（帧率无关的步进）
            // 每帧只靠近目标一部分，形成平滑跟随而非瞬移
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);

            // Quaternion.Slerp：四元数球面插值，旋转过渡比 Lerp 更自然、路径更短
            // 参数含义同 Lerp：从当前 rotation 朝 targetRot 平滑旋转
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, smoothSpeed * Time.deltaTime);
        }
    }
}
