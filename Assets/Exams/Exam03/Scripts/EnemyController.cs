using UnityEngine;
namespace Exam.Exam03
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private int healthHp;
        void Start()
        {
            //随机生成颜色
            GetComponent<Renderer>().material.color = Random.ColorHSV();
        }
        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Bullet")) return;

            var health = FindObjectOfType<PlayerHealth>();
            if (health != null)
                health.AddHealth(healthHp);

            Destroy(other.gameObject);
            //Destroy(gameObject);
        }
    }
}

