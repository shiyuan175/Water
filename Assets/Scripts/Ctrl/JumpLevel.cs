using System.Collections;
using System.Collections.Generic;
using QFramework;
using QFramework.Example;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JumpLevel : MonoBehaviour, ICanSendEvent, ICanGetUtility
{
    public TMP_InputField inputField;
    public Button button;
    public Button Btnfinish;
    public GameObject debugPanel;
    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(() =>
        {
            LevelManager.Instance.StartGame(int.Parse(inputField.text));
            this.GetUtility<SaveDataUtility>().SaveLevel(int.Parse(inputField.text));
            UIKit.ClosePanel<UIGameNode>();
            UIKit.OpenPanel<UIGameNode>();
            //this.SendEvent<GameStartEvent>();
            //GameCtrl.Instance.InitGameCtrl();
        });

        Btnfinish.onClick.AddListener(() =>
        {
            StartCoroutine(LevelManager.Instance.TestFinish());
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            for (int i = 0; i < 2; i++)
                this.SendEvent(new ReturnToMainEvent { PassLevel = true });
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            for (int i = 0; i < 10; i++)
                this.SendEvent(new ReturnToMainEvent { PassLevel = true });
        }
        if (Input.GetKey(KeyCode.L))
        {
            LevelManager.Instance.AddMoveNum();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            debugPanel.SetActive(true);
        }
    }
}
