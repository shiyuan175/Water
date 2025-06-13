using System.Collections;
using System.Collections.Generic;
using QFramework;
using QFramework.Example;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JumpLevel : MonoBehaviour ,ICanSendEvent, ICanGetUtility
{
    public TMP_InputField inputField;
    public Button button;
    public Button Btnfinish;
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
        
    }
}
