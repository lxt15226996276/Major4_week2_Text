using UnityEngine;
using UnityEngine.UI;
namespace Exam.Exam07
{
    public class UserNameDisplay : MonoBehaviour
    {
        [SerializeField] private Text userNameText;
        void Start()
        {
            if (AccountData.Instacne != null)
            {
                userNameText.text = AccountData.Instacne.CurrentUserName;
            }
        }
    }
}

