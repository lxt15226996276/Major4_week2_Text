using UnityEngine;
namespace Exam.Exam03
{
    public class EnemyController : MonoBehaviour
    {
        void Start()
        {
            //随机生成颜色
            GetComponent<Renderer>().material.color = Random.ColorHSV();
        }

    }
}

