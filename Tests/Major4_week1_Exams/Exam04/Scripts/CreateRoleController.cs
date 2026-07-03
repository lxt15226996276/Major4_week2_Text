using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Exam.Exam04
{
    /// <summary>
    /// 创角场景：选角色 切图 随机昵称 开始游戏
    /// </summary>
    public class CreateRoleController : MonoBehaviour
    {
        /// <summary>
        /// 单个角色的名称 详情 立绘
        /// </summary>
        public class RoleInfo
        {
            public string roleName;  //职业名，如：战士
            public string roleDetail;//详情，如【近战物理输入】
            public Sprite portrait; //人物立绘
        }

        [Header("角色数据")]
        [SerializeField] private RoleInfo[] roles = new RoleInfo[4];
        [SerializeField] private Button[] roleButtons;

        [Header("显示控件")]
        [SerializeField] private Image rolePortrait;
        [SerializeField] private Text roleNameText;
        [SerializeField] private Text roleDetailText;

        [Header("底部操作")]
        [SerializeField] private InputField nicknameInput;
        [SerializeField] private Button btnRandomName;
        [SerializeField] private Button btnStartGame;

        [Header("开始游戏")]
        [SerializeField] private float quitDelay = 7f;

        //当前选中的角色索引
        private int courrentIndex = 0;

    }
}

