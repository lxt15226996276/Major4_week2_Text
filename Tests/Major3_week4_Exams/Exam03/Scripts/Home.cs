using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace Exam.Exam03
{
    public class Home : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Enemy")) return;
            if (GameManager.Instance == null) return;
            GameManager.Instance.HomeGameOver();
        }
    }

}
