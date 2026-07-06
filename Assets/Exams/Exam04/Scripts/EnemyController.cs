using UnityEngine;
namespace Exam.Exam04
{
    public class EnemyController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Bullet")) return;
            Destroy(other.gameObject);

            GameManager.Instance?.OnEnemyDied();
            Destroy(gameObject);
        }
    }
}

