using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam02
{
    public class Game3Controller : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Renderer renderer = GetComponent<Renderer>();
            renderer.material.color = Random.ColorHSV();
        }


    }
}

