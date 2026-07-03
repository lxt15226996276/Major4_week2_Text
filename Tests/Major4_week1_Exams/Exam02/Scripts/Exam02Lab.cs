using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam02
{
    public class Exam02Lab : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Lab1_DictionaryRegisterLogin();
            StartCoroutine(Lab2_FakeAsyncLoad());
            Lab3_DropdownColor();
        }
        //实验1：字典注册登录
        void Lab1_DictionaryRegisterLogin()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            //模拟注册三个账号
            dic.Add("user1", "111");
            dic.Add("user2", "222");
            dic.Add("user3", "333");

            Debug.Log("[实验1 字典数量：]" + dic.Count);

            //模拟登录成功
            string inputAcc = "user2";
            string inpuPwd = "222";
            if (dic.TryGetValue(inputAcc, out string stored) && stored == inpuPwd)
            {
                Debug.Log("[实验1] 登录成功");
            }
            else
            {
                Debug.Log("[实验1] 登录失败");
            }

            //模拟登录失败
            if (!dic.TryGetValue("wrong", out _))
            {
                Debug.Log("[实验1] 账号不存在，登录失败");
            }
        }
        //实验2：协程模拟异步加载进度
        IEnumerator Lab2_FakeAsyncLoad()
        {
            float progess = 0f;
            Debug.Log("[实验2] 开始模拟加载");

            while (progess < 1f)
            {
                progess += 0.1f;
                Debug.Log($"[实验2 ] 进度：{progess:p0}");

                yield return new WaitForSeconds(0.3f);
            }
            Debug.Log("[实验2 ] 加载完成，可切换场景");
        }
        //实验3： Dropdown 索引->颜色
        void Lab3_DropdownColor()
        {
            string[] serves = { "一区", "二区", "三区" };
            Color[] colors = { Color.red, Color.yellow, Color.green };

            for (int index = 0; index < serves.Length; index++)
            {
                Debug.Log($"[实验3] 选择{serves[index]}->颜色 {colors[index]}");
            }
        }
    }
}

