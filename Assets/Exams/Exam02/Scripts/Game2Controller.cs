using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
namespace Exam.Exam02
{
    public class Game2Controller : MonoBehaviour
    {
        private float rotateSpeed = 90f;
        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    }

}

