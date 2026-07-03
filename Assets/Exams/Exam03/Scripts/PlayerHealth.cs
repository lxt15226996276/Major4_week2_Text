using UnityEngine;
using UnityEngine.UI;
namespace Exam.Exam03
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private Slider hpSlider;
        [SerializeField] private int maxHp = 100;
        private int currentHp;

        void Awake()
        {
            hpSlider.maxValue = maxHp;
            currentHp = 0;
            hpSlider.value = 0;
            victoryPanel.SetActive(false);
        }

        /// <summary>
        /// 加血逻辑
        /// </summary>
        /// <param name="hp"></param>
        private void AddHealth(int hp)
        {
            currentHp += hp;
            if (currentHp > maxHp)
            {
                currentHp = maxHp;
            }

            hpSlider.value = currentHp;
            if (currentHp >= maxHp)
            {
                victoryPanel.SetActive(true);
            }
        }

    }
}

