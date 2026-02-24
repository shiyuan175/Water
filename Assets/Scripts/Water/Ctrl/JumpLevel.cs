using Game.Water;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Water
{
    public class JumpLevel : MonoBehaviour, ICanSendEvent, ICanGetUtility
    {
        public TMP_InputField inputField;
        public Button SkipBtn;
        public Button Btnfinish;
        public GameObject debugPanel;

        private const string COMMAND = "IOQ1@#123";
        private System.Text.StringBuilder buffer = new();

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        private void Start()
        {
            SkipBtn.onClick.AddListener(() =>
            {
                LevelManager.Instance.StartGame(int.Parse(inputField.text));
                this.GetUtility<SaveDataUtility>().SaveLevel(int.Parse(inputField.text));
                if (!UIKit.GetPanel<UIGameNode>())
                    UIKit.OpenPanel<UIGameNode>();
                UIKit.GetPanel<UIGameNode>().Show();
            });

            Btnfinish.onClick.AddListener(() =>
            {
                StartCoroutine(LevelManager.Instance.TestFinish());
            });
        }

        private void Update()
        {
            if (!debugPanel.activeSelf)
            {
                foreach (char c in Input.inputString)
                {
                    if (c == '\n' || c == '\r')
                    {
                        if (buffer.ToString() == COMMAND)
                        {
                            debugPanel.Show();
                            return;
                        }
                        buffer.Clear();
                        continue;
                    }
                    buffer.Append(c);
                }
            }
        }
    }
}
