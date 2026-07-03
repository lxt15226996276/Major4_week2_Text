using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Exam.Exam02
{
    public class BattleController : MonoBehaviour
    {
        [Header("血条")]
        [SerializeField] private Slider hpSlider;
        [SerializeField] private Text hpLabel;
        [SerializeField] private int maxHp = 100;
        [Header("攻击按钮")]
        [SerializeField] private Button btnAttack20;
        [SerializeField] private Button btnAttack30;
        [SerializeField] private Button btnAttack50;
        [Header("游戏结束")]
        [SerializeField] private GameObject gameOverPanel;
        private int currentHp;

        void Start()
        {
            currentHp = maxHp;
            hpSlider.maxValue = maxHp;
            hpSlider.value = currentHp;

            btnAttack20.onClick.AddListener(() => TakeDamage(20));
            btnAttack30.onClick.AddListener(() => TakeDamage(30));
            btnAttack50.onClick.AddListener(() => TakeDamage(50));

            gameOverPanel.SetActive(false);
            hpLabel.text = "生命值：" + currentHp;
        }

        private void TakeDamage(int damage)
        {
            if (currentHp <= 0) return;
            currentHp -= damage;
            if (currentHp < 0) currentHp = 0;
            hpSlider.value = currentHp;
            if (currentHp <= 0)
            {
                gameOverPanel.SetActive(true);
                Debug.Log("游戏结束");
            }
            hpLabel.text = "生命值：" + currentHp;
        }

    }
}

