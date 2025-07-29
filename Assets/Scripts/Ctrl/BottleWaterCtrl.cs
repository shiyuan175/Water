using DG.Tweening;
using QFramework;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using GameAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class BottleWaterCtrl : MonoBehaviour
{
    public SkeletonGraphic spine, broomSpine, createSpine, changeSpine, magnetSpine, changeShineSpine, thunderSpine, broomAfterSpine, fireRuneSpine,starBlackWater;
    public GameObject spineGo, HideGo, broomItemGo, createItemGo, changeItemGo, magnetItemGo, thunderGo, broomAfterGo, wenhaoFxGo, iceGo;
    public Animator anim;
    public Image waterImg;
    public int waterColor;
    public bool isPlayItemAnim;
    public TextMeshProUGUI textItem;
    public GameObject fireRuneGo;
    public BottleCtrl bottle; 
    public BombCtrl bombCtrl;
    
    public Color color
    {
        get
        {
            return waterImg.color;
        }

        set
        {
            waterImg.color = value;
            var waterRenderUpdater = waterImg.gameObject.GetComponent<WaterRenderUpdate>();
            if (waterRenderUpdater != null)
            {
                waterRenderUpdater.WaterColor = value;
            }
        }
    }
    // Start is called before the first frame update
    private void Start()
    {

    }

    

    public void SetSpineActive(bool active)
    {
        spineGo.SetActive(active);
    }

    public void PlayFillAnim(float time)
    {
        //StartCoroutine(CoroutinePlayFillAnim());
        waterImg.fillAmount = 0;
        waterImg.DOFillAmount(1, time).SetEase(Ease.Linear);
    }

    public void PlayOutAnim(float time)
    {
        //StartCoroutine(CoroutinePlayFillAnim());


        if (time == 0)
        {
            waterImg.fillAmount = 0;
            gameObject.SetActive(false);
            broomItemGo.SetActive(false);
            createItemGo.SetActive(false);
            changeItemGo.SetActive(false);
        }
        else
        {
            waterImg.fillAmount = 1;
            waterImg.DOFillAmount(0, time).SetEase(Ease.Linear).OnComplete(() =>
            {
                gameObject.SetActive(false);
                broomItemGo.SetActive(false);
                createItemGo.SetActive(false);
                changeItemGo.SetActive(false);
            });
        }

    }

    public void PlayUseBroom(BottleWaterCtrl hide)
    {
        StartCoroutine(CoroutinePlayUseBroom(hide));
    }



    IEnumerator CoroutinePlayUseBroom(BottleWaterCtrl hide)
    {
        isPlayItemAnim = true;
        hide.gameObject.SetActive(true);
        var go = Instantiate(broomItemGo);
        var go1 = Instantiate(hide.broomItemGo);
        go.transform.parent = transform;
        go.transform.localScale = broomItemGo.transform.localScale;
        go.transform.localPosition = broomItemGo.transform.localPosition;
        go1.transform.parent = transform;
        go1.transform.localScale = broomItemGo.transform.localScale;
        go1.transform.localPosition = broomItemGo.transform.localPosition + new Vector3(0, 83.4f, 0);


        var useSpine = go.GetComponent<SkeletonGraphic>();
        var useSpine1 = go1.GetComponent<SkeletonGraphic>();
        yield return new WaitForEndOfFrame();
        go.transform.parent = LevelManager.Instance.gameCanvas;
        go1.transform.parent = LevelManager.Instance.gameCanvas;
        useSpine1.AnimationState.SetAnimation(0, broomSpine.AnimationState.ExpandToIndex(0).Animation.name, false);

        go1.transform.DOLocalMove(go.transform.localPosition, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Destroy(go1);
        });

        broomItemGo.SetActive(false);
        go.transform.Find("Top").gameObject.SetActive(false);
        useSpine.AnimationState.SetAnimation(0, "disappear", false);
        isPlayItemAnim = false;
        hide.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        hide.bottle.SetBottleColor();

        Destroy(go);
        //gameObject.SetActive(false);

    }

    public IEnumerator ShowBroomAfter()
    {
        yield return new WaitForSeconds(1);

        gameObject.SetActive(true);
        broomAfterGo.SetActive(true);
        broomAfterSpine.AnimationState.SetEmptyAnimation(0, 0f);
        broomAfterSpine.AnimationState.SetAnimation(0, "combine", false);

        yield return new WaitForSeconds(1.2f);
        broomAfterSpine.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void PlayUseCreate(BottleCtrl bottleCtrl, BottleWaterCtrl hide)
    {
        StartCoroutine(CoroutinePlayUseCreate(bottleCtrl, hide));
    }



    IEnumerator CoroutinePlayUseCreate(BottleCtrl bottleCtrl, BottleWaterCtrl hide)
    {
        isPlayItemAnim = true;
        //broomItemGo.SetActive(true);
        var go = Instantiate(createItemGo);
        var go1 = Instantiate(hide.createItemGo);
        go.transform.parent = transform;
        go.transform.localScale = createItemGo.transform.localScale;
        go.transform.localPosition = createItemGo.transform.localPosition;
        go1.transform.parent = transform;
        go1.transform.localScale = createItemGo.transform.localScale;
        go1.transform.localPosition = createItemGo.transform.localPosition + new Vector3(0, 83.4f, 0);

        var useSpine = go.GetComponent<SkeletonGraphic>();
        var useSpine1 = go1.GetComponent<SkeletonGraphic>();
        yield return new WaitForEndOfFrame();
        go.transform.parent = LevelManager.Instance.gameCanvas;
        go1.transform.parent = LevelManager.Instance.gameCanvas;
        useSpine1.AnimationState.SetAnimation(0, createSpine.AnimationState.ExpandToIndex(0).Animation.name, false);

        go1.transform.DOLocalMove(go.transform.localPosition, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Destroy(go1);
        });
        createItemGo.SetActive(false);
        go.transform.Find("Top").gameObject.SetActive(false);
        useSpine.AnimationState.SetAnimation(0, "combine", false);
        isPlayItemAnim = false;
        hide.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);
        //gameObject.SetActive(false);
        Destroy(go);
        bottleCtrl.SetBottleColor();
        isPlayItemAnim = false;
        gameObject.SetActive(false);

    }


    public void PlayUseChange(BottleWaterCtrl hide)
    {
        StartCoroutine(CoroutinePlayUseChange(hide));
    }

    IEnumerator CoroutinePlayUseChange(BottleWaterCtrl hide)
    {
        isPlayItemAnim = true;
        //broomItemGo.SetActive(true);
        var go = Instantiate(changeItemGo);
        var go1 = Instantiate(hide.changeItemGo);
        go.transform.parent = transform;
        go.transform.localScale = changeItemGo.transform.localScale;
        go.transform.localPosition = changeItemGo.transform.localPosition;
        go1.transform.parent = transform;
        go1.transform.localScale = changeItemGo.transform.localScale;
        go1.transform.localPosition = changeItemGo.transform.localPosition + new Vector3(0, 83.4f, 0);

        var useSpine = go.GetComponent<SkeletonGraphic>();
        var useSpine1 = go1.GetComponent<SkeletonGraphic>();
        yield return new WaitForEndOfFrame();
        go.transform.parent = LevelManager.Instance.gameCanvas;
        go1.transform.parent = LevelManager.Instance.gameCanvas;
        useSpine1.AnimationState.SetAnimation(0, changeSpine.AnimationState.ExpandToIndex(0).Animation.name, false);

        go1.transform.DOLocalMove(go.transform.localPosition, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Destroy(go1);
        });
        changeItemGo.SetActive(false);
        go.transform.Find("Top").gameObject.SetActive(false);
        useSpine.AnimationState.SetAnimation(0, "combine", false);
        isPlayItemAnim = false;
        hide.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        isPlayItemAnim = false;
        Destroy(go);
        gameObject.SetActive(false);
    }


    #region 魔法阵动画

    private Coroutine playMagnetCoroutine;
    private GameObject magnetGo;
    private GameObject magnetGo1;

    public void PlayUseMagnet(BottleWaterCtrl hide)
    {
        playMagnetCoroutine = StartCoroutine(CoroutinePlayUseMagnet(hide));
    }

    /// <summary>
    /// 提供外部终止动画播放的方法
    /// </summary>
    public void StopPlayUseMagnet()
    {
        if (playMagnetCoroutine != null)
        {
            StopCoroutine(playMagnetCoroutine);
            playMagnetCoroutine = null;
        }

        if (magnetGo != null)
        {
            Destroy(magnetGo);
            magnetGo = null;
        }

        if (magnetGo1 != null)
        {
            Destroy(magnetGo1);
            magnetGo1 = null;
        }

        isPlayItemAnim = false;
    }

    IEnumerator CoroutinePlayUseMagnet(BottleWaterCtrl hide)
    {
        isPlayItemAnim = true;
        //broomItemGo.SetActive(true);
        magnetGo = Instantiate(magnetItemGo);
        magnetGo1 = Instantiate(hide.magnetItemGo);
        magnetGo.transform.parent = transform;
        magnetGo.transform.localScale = magnetItemGo.transform.localScale;
        magnetGo.transform.localPosition = magnetItemGo.transform.localPosition;
        magnetGo1.transform.parent = transform;
        magnetGo1.transform.localScale = magnetItemGo.transform.localScale;
        magnetGo1.transform.localPosition = magnetItemGo.transform.localPosition + new Vector3(0, 83.4f, 0);

        var useSpine = magnetGo.GetComponent<SkeletonGraphic>();
        var useSpine1 = magnetGo1.GetComponent<SkeletonGraphic>();
        yield return new WaitForEndOfFrame();
        magnetGo.transform.parent = LevelManager.Instance.gameCanvas;
        magnetGo1.transform.parent = LevelManager.Instance.gameCanvas;
        useSpine1.AnimationState.SetAnimation(0, magnetSpine.AnimationState.ExpandToIndex(0).Animation.name, false);

        magnetGo1.transform.DOLocalMove(magnetGo.transform.localPosition, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (magnetGo1 != null) Destroy(magnetGo1);
        });
        magnetItemGo.SetActive(false);
        magnetGo.transform.Find("Top").gameObject.SetActive(false);
        useSpine.AnimationState.SetAnimation(0, "combine", false);
        isPlayItemAnim = false;
        hide.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        isPlayItemAnim = false;
        //gameObject.SetActive(false);
        if (magnetGo != null) Destroy(magnetGo);
        gameObject.SetActive(false);
        playMagnetCoroutine = null;
    }
    #endregion

    public void PlayFillAnimConnect()
    {
        string spineAnimName = "";
        switch (waterColor)
        {
            case 0:
                spineAnimName = "daoshui_dh";
                break;
            case 1:
                spineAnimName = "daoshui_cl";
                break;
        }
        spineGo.SetActive(true);
        anim.Play("WaterFillConnect");
        spine.AnimationState.SetAnimation(0, spineAnimName, false);
        //StartCoroutine(CoroutinePlayFillAnim());
    }

    public void PlayEmptyAnim()
    {
        anim.Play("NormalEmpty");
    }

    public void SetHide(bool isHide, bool noWait)
    {
        if (isHide || (!isHide && noWait) || !gameObject.activeSelf)
        {
            if (!isHide && HideGo.activeSelf)
            {
                wenhaoFxGo.SetActive(false);
                wenhaoFxGo.SetActive(true);
            }

            HideGo.SetActive(isHide);

        }
        else
        {
            StartCoroutine(PlayHide(isHide, noWait));
        }
    }

    public void SetHide(bool isHide, bool noWait,GameDefine.ItemType itemType)
    {
        if (isHide || (!isHide && noWait) || !gameObject.activeSelf)
        {
            if (!isHide && HideGo.activeSelf)
            {
                wenhaoFxGo.SetActive(false);
                wenhaoFxGo.SetActive(true);
            }
            HideGo.SetActive(isHide);
            var type = itemType.GetType();
            var fieldName = Enum.GetName(type, itemType);

            if (fieldName == null)
                return;
            var fieldInfo = type.GetField(fieldName);
            
            if (fieldInfo.GetCustomAttribute(typeof(WaterColorState), false) is not WaterColorState attribute)
                return;

            switch (attribute.SpineType)
            {
                case EColorStateSpineType.EBroomSpine:
                    broomItemGo.SetActive(!isHide);
                    break;
                case EColorStateSpineType.EMagnetSpine:
                    magnetItemGo.SetActive(!isHide);
                    break;
                case EColorStateSpineType.ECreateSpine:
                    createItemGo.SetActive(!isHide);
                    break;
                case EColorStateSpineType.EChangeSpine:
                    changeItemGo.SetActive(!isHide);
                    break;
            }

        }
        else
        {
            StartCoroutine(PlayHide(isHide, noWait));
        }
    }

    public IEnumerator PlayHide(bool isHide, bool noWait)
    {
        yield return new WaitForSeconds(0.6f);

        if (!isHide && HideGo.activeSelf)
        {
            wenhaoFxGo.SetActive(false);
            wenhaoFxGo.SetActive(true);
        }


        HideGo.SetActive(isHide);
    }


    public IEnumerator ChangeShine()
    {
        LevelManager.Instance.isPlayFxAnim = true;
        changeShineSpine.gameObject.SetActive(true);
        changeShineSpine.AnimationState.SetEmptyAnimation(0, 0f);
        yield return new WaitForSeconds(1.4f);
        changeShineSpine.AnimationState.SetAnimation(0, "attack", false);

        yield return new WaitForSeconds(2);
        changeShineSpine.gameObject.SetActive(false);

        LevelManager.Instance.isPlayFxAnim = false;
    }
    public IEnumerator ShowThunder(Transform target)
    {
        yield return new WaitForSeconds(1f);
        var go = Instantiate(thunderGo);
        //var useSpine = go.GetComponent<SkeletonGraphic>();
        //useSpine.AnimationState.SetEmptyAnimation(0, 0f);
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.parent = LevelManager.Instance.gameCanvas;
        go.transform.localScale = new Vector3(1, 1, 1);
        ThunderCtrl thunderCtrl = go.GetComponent<ThunderCtrl>();
        thunderCtrl.target = target;
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = thunderGo.transform;
        source.weight = 1;
        thunderCtrl.positionConstraint.AddSource(source);

        go.SetActive(true);
        //var offset = fromPos - transform.position;
        //go.transform.localScale = new Vector3(thunderGo.transform.localScale.x, Vector3.Distance(fromPos, transform.position) / 5.5f, thunderGo.transform.localScale.z);
        ////thunderGo.transform.position = (fromPos + transform.position) / 2;
        //var angle = Vector3.Angle(fromPos - transform.position, Vector3.up);
        //thunderGo.transform.rotation = Quaternion.Euler(0, 0, -angle);

        yield return new WaitForEndOfFrame();
        thunderCtrl.positionConstraint.constraintActive = true;
        //thunderSpine.AnimationState.SetAnimation(0, "bullet", false);

        //yield return new WaitForSeconds(2);
        //thunderGo.SetActive(false);

    }

    public IEnumerator BreakIce(BottleWaterCtrl waterCtrl)
    {
        waterCtrl.bottle.isPlayAnim = true;

        isPlayItemAnim = true;
        fireRuneGo.SetActive(true);
        fireRuneSpine.AnimationState.SetAnimation(0, "combine", false);

        AudioKit.PlaySound("resources://Audio/FireBreakIce");
        yield return new WaitForSeconds(1.2f);

        var go = GameObject.Instantiate(fireRuneGo);
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.parent = LevelManager.Instance.gameCanvas;
        go.transform.localScale = new Vector3(1, 1, 1);
        var spine = go.transform.Find("FireRune").GetComponent<SkeletonGraphic>();
        spine.AnimationState.SetAnimation(0, "bullet", false);

        var offset = waterCtrl.transform.position - transform.position;
        if(offset.x < 0)
        {
            go.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            go.transform.localScale = new Vector3(-1, 1, 1);
        }


        go.transform.DOMove(waterCtrl.transform.position, 0.45f).SetEase(Ease.Linear).OnComplete(() =>
        {
            waterCtrl.HideIce(() => waterCtrl.bottle.isPlayAnim = false);
            isPlayItemAnim = false;
            fireRuneGo.SetActive(false);
            go.DestroySelf();
        });

    }

    public void HideIce(Action action)
    {
        fireRuneGo.SetActive(true);

        TrackEntry trackEntry =
        fireRuneSpine.AnimationState.SetAnimation(0, "attack", false);

        trackEntry.Complete += (entry) =>
        {
            UnlockIceWater();
            fireRuneGo.SetActive(false);
            action?.Invoke();
        };
    }

    public void UnlockIceWater()
    {
        bottle.UnlockIceWater();
    }

    public void SetColorState(GameDefine.ItemType itemType, Color inColor, bool isTopWater, int time = 0)
    {
        this.color = inColor;
        
        var type = itemType.GetType();
        var fieldName = Enum.GetName(type, itemType);

        if (fieldName == null) 
            return;
        var fieldInfo = type.GetField(fieldName);
        if (fieldInfo.GetCustomAttribute(typeof(WaterColorState), false) is not WaterColorState attribute) 
            return;
        broomItemGo.SetActive(attribute.BroomItemActive);
        broomItemGo.transform.Find("Top").gameObject.SetActive(isTopWater);
        createItemGo.SetActive(attribute.CreateItemActive);
        createItemGo.transform.Find("Top").gameObject.SetActive(isTopWater);
        changeItemGo.SetActive(attribute.ChangeItemActive);
        changeItemGo.transform.Find("Top").gameObject.SetActive(isTopWater);
        magnetItemGo.SetActive(attribute.MagnetItemActive);
        magnetItemGo.transform.Find("Top").gameObject.SetActive(isTopWater);
        if (attribute.SpineAnim.IsNullOrEmpty() == false && attribute.SpineType > EColorStateSpineType.None && attribute.SpineType < EColorStateSpineType.Max)
        {
            switch (attribute.SpineType)
            {
                case EColorStateSpineType.EBroomSpine:
                    broomSpine.AnimationState.SetAnimation(0, attribute.SpineAnim, false);
                    break;
                case EColorStateSpineType.EMagnetSpine:
                    magnetSpine.AnimationState.SetAnimation(0, attribute.SpineAnim, false);
                    break;
                case EColorStateSpineType.ECreateSpine:
                    createSpine.AnimationState.SetAnimation(0, attribute.SpineAnim, false);
                    break;
                case EColorStateSpineType.EChangeSpine:
                    changeSpine.AnimationState.SetAnimation(0, attribute.SpineAnim, false);
                    break;
            }
        }
    }

    public void PlayStarBlackWaterEffect()
    {
        HideGo.SetActive(false);
        starBlackWater.Show();
        TrackEntry entry = starBlackWater.AnimationState.SetAnimation(0, "attack", false);
        entry.Complete += (trackEntry) =>
        {
            starBlackWater.Hide();
        };
    }
}
