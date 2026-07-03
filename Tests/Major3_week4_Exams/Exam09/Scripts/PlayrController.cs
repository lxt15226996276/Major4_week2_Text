using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Exam.Exam09
{
    public class PlayrController : MonoBehaviour
    {
        //移动速速
        private float moveSpeed = 10f;
        //旋转速度
        private float turnSpeed = 90f;
        [SerializeField] private GameObject Effect;
        private bool IsMove = true;

        void Update()
        {
            if (IsMove)
            {
                float V = Input.GetAxis("Vertical");
                float H = Input.GetAxis("Horizontal");
                transform.Translate(transform.forward * V * moveSpeed * Time.deltaTime, Space.World);
                transform.Rotate(transform.up * H * turnSpeed * Time.deltaTime);
            }

        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.CompareTag("Wall") || collision.transform.tag == "AICar")
            {
                Instantiate(Effect, transform.position, Quaternion.identity, this.transform);

                IsMove = false;
            }
            else if (collision.transform.CompareTag("End"))
            {

                SceneManager.LoadScene("GameOver");
            }
        }

    }
}


