using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam01
{
    public class FakeButton
    {
        //模拟unity 的 unityEvent,内部就是委托列表
        public event Action OnClick;

        public void Click()
        {
            Debug.Log(" [FakeButon]被点击了");
            OnClick?.Invoke();
        }
    }
    public class DelegatePratice : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Exercise1_ActionBasic();
            Exercise2_MethodGroupVSLambda();
            Exercise3_NullSafeInvoke();
        }

        void Exercise1_ActionBasic()
        {
            //声明：一个接收string 无返回值的委托变量
            Action<string> onMessage;

            //赋值：把sayhello 方法[存进去](注意：没有括号)
            onMessage = SayHello;

            //调用委托
            onMessage.Invoke("Unity 委托");

            Debug.Log("实验1完成");
        }

        void Exercise2_MethodGroupVSLambda()
        {
            FakeButton btn = new FakeButton();
            btn.OnClick += OnButtonClickA;
            btn.OnClick += () => Debug.Log("[Lambda]回车触发的匿名逻辑");
            btn.OnClick += () => OnButtonClickB();
            btn.Click();
            Debug.Log("====实验2 完成====");
        }

        void Exercise3_NullSafeInvoke()
        {
            Action<string> callback = null;
            callback?.Invoke("这行不会执行");

            callback = msg => Debug.Log("[实验3] 回调：" + msg);
            callback?.Invoke("第二次有委托了");
            Debug.Log("=====实验3 完成===");
        }
        void SayHello(string msg)
        {
            Debug.Log("[实验1] 收到消息" + msg);
        }
        void OnButtonClickA() { Debug.Log("[方法组] OnButtonClickA"); }
        void OnButtonClickB() { Debug.Log("[方法组] OnButtonClickB"); }
    }
}

