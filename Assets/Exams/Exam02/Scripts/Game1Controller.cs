using UnityEngine;
namespace Exam.Exam02
{
    public class Game1Controller : MonoBehaviour
    {
        private float moveSpeed = 5f;
        void Update()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            transform.Translate(new Vector3(h, 0, v) * moveSpeed * Time.deltaTime);
        }

    
    }
}

