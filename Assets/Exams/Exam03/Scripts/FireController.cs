using UnityEngine;
using UnityEngine.UI;
namespace Exam.Exam03
{
    public class FireController : MonoBehaviour
    {
        [SerializeField] private Button btnFire;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float speed;


        void OnEnable()
        {
            btnFire.onClick.AddListener(Fire);
        }

        private void Fire()
        {
            if (bulletPrefab == null || firePoint == null) return;
            var obj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            obj.GetComponent<Rigidbody>().AddForce(firePoint.forward * speed, ForceMode.Impulse);
            Destroy(obj, 3f);
        }
        void OnDestroy()
        {
            btnFire.onClick.RemoveListener(Fire);
        }

    }
}

