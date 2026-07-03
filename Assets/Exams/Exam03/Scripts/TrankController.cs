using UnityEngine;
namespace Exam.Exam03
{
    public class TrankController : MonoBehaviour
    {
        private float moveSpeed = 6f;
        private float turnRotation = 90f;

        void Update()
        {
            Mvoe();
        }

        private void Mvoe()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            transform.Translate(new Vector3(0, 0, v) * moveSpeed * Time.deltaTime, Space.Self);
            transform.Rotate(new Vector3(0, h, 0) * turnRotation * Time.deltaTime);
        }
    }
}

