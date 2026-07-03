using UnityEngine;

namespace Exam.Exam07
{
    /// <summary>Exam07：WASD/方向键移动 PlayerCube（XZ 平面）。</summary>
    public class CubeController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        private void Update()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 dir = new Vector3(h, 0f, v);
            if (dir.sqrMagnitude < 0.01f) return;

            transform.Translate(dir.normalized * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}