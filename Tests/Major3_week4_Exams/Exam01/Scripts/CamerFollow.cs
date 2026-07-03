using UnityEngine;

namespace Exam.Exam01
{
    /// <summary>
    /// 摄像机跟随主角。
    /// 核心思路：在 Awake 中记录「相机相对主角的本地位置与旋转差」，
    /// 在 LateUpdate 中根据主角当前位姿还原相机应在的世界位置与朝向，并用插值平滑移动。
    /// 这样主角用 A/D 旋转时，相机会绕主角保持相对角度，而不是固定在世界坐标后方。
    /// </summary>
    public class CamerFollow : MonoBehaviour
    {
        // Inspector 分组标题，方便在编辑器里区分字段用途
        [Header("跟随目标")]

        // 要跟随的主角 Transform；在 Main Camera 的 Inspector 里拖入 Hierarchy 中的 Player
        // 使用 Transform 而非 GameObject，因为后续只关心位置与旋转，不需要访问其它组件
        [SerializeField] private Transform target;

        // 相机在「主角本地坐标系」下的位置偏移（Awake 中自动计算，Play 后 Inspector 里会看到真实数值）
        // 为何用本地坐标：主角转向时，世界坐标下的固定偏移不会跟着转；本地偏移会随主角一起旋转
        [SerializeField] private Vector3 localOffset;

        // 相机相对主角的旋转差（四元数）；Awake 中自动计算，用于还原「始终看向主角」的相对朝向
        // 公式：target.rotation * localRotation == 相机应有的世界旋转
        [SerializeField] private Quaternion localRotation;

        // 平滑跟随速度：数值越大，每帧越接近目标位姿；8 是第三人称相机常用的起步值，可在 Inspector 微调
        // 乘以 Time.deltaTime 是为了帧率无关——60fps 与 30fps 下跟随手感一致
        [SerializeField] private float smoothSpeed = 8f;

        void Awake()
        {
            // 若忘记在 Inspector 指定 target，无法计算偏移，直接返回，避免下面访问 target 时空引用报错
            if (target == null) return;

            // InverseTransformPoint：把「世界坐标点」转换到「以 target 为原点、朝向为轴」的本地坐标
            // 入参 transform.position 是本相机（挂脚本物体）当前世界位置
            // 结果 localOffset 表示：相对主角，相机在右/上/前各偏多少——主角怎么动，这个相对关系不变
            localOffset = target.InverseTransformPoint(transform.position);

            // 四元数乘法顺序：Inverse(主角旋转) * 相机旋转 = 相机相对主角的「额外旋转」
            // 存下来后，每帧用 target.rotation * localRotation 即可还原相机应有的世界朝向
            // 这样主角 A/D 转身时，相机不仅位置绕圈，朝向也会跟着转，始终维持开局时的观察角度
            localRotation = Quaternion.Inverse(target.rotation) * transform.rotation;
        }

        /// <summary>
        /// 在每一帧所有 Update 执行完毕后再调用。
        /// 主角在 Update 里移动/旋转，相机若也在 Update 里跟随，可能读到「半帧」状态导致轻微抖动；
        /// LateUpdate 保证先处理完主角，再移动相机，是跟随类脚本的标准写法。
        /// </summary>
        void LateUpdate()
        {
            // TransformPoint：InverseTransformPoint 的逆运算，把本地偏移 localOffset 变回世界坐标
            // 主角移动 → target.position 变；主角旋转 → 同一 localOffset 对应的世界点也会绕主角转
            // 因此 targetPos 就是「本帧相机应该在的世界位置」
            Vector3 targetPos = target.TransformPoint(localOffset);

            // 主角当前世界旋转 × 开局记录的相对旋转 = 本帧相机应有的世界旋转
            Quaternion targetRotation = target.rotation * localRotation;

            // Slerp（球面线性插值）：在四元数之间平滑过渡，旋转路径更自然，避免欧拉角万向节问题
            // 第三个参数是 0~1 的插值因子；smoothSpeed * Time.deltaTime 越大，本帧越接近目标朝向
            // 先改 rotation 再改 position：两者独立，顺序对视觉效果影响很小，习惯上先朝向再位移亦可
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);

            // Lerp：直线插值位置，从当前相机位置向 targetPos 靠近，产生「平滑跟随」而非瞬移
            // 若 smoothSpeed 很大或设为直接赋值 transform.position = targetPos，则变成硬跟随、无缓冲
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
        }

    }
}
