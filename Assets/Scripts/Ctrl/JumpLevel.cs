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
    int i = 0;
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
            UIKit.GetPanel<UIGameNode>().Show();
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
#if UNITY_EDITOR
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

        if (Input.GetKeyDown(KeyCode.O))
        {
            LevelManager.Instance.CurtainUpdate();
        }
#endif
       
    }
}
