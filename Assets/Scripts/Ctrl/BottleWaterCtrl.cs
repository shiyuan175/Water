using DG.Tweening;
using QFramework;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Reflection;
using GameAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using GameDefine;
using static UnityEditor.Progress;

public class BottleWaterCtrl : MonoBehaviour
{
    public SkeletonGraphic spine, broomSpine, createSpine, changeSpine, magnetSpine, changeShineSpine, thunderSpine, broomAfterSpine, fireRuneSpine,starBlackWater;
    public GameObject spineGo, HideGo, broomItemGo, createItemGo, changeItemGo, magnetItemGo, thunderGo, broomAfterGo, wenhaoFxGo, iceGo, RainBowWater,BombBlackWaterItemGo;
    public Animator anim;
    public Image waterImg;
    public int waterColor;
    public bool isPlayItemAnim;
    public TextMeshProUGUI textItem;
    public GameObject fireRuneGo;
    public BottleCtrl bottle;

    #region NewMechineCtrl
    public BombCtrl bombCtrl;
    public BubbleCtrl bubbleCtrl;
    #endregion

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

    #region 魔法阵动画

    private Coroutine playMagnetCoroutine;
    private GameObject magnetGo;
    private GameObject magnetGo1;

   /* public void PlayUseMagnet(BottleWaterCtrl hide)
    {
        playMagnetCoroutine = StartCoroutine(CoroutinePlayUseMagnet(hide));
    }*/

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

   /* IEnumerator CoroutinePlayUseMagnet(BottleWaterCtrl hide)
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
    }*/
    #endregion

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

    #region 破冰动画和回调

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
            bottle.UnlockIceWater();
            fireRuneGo.SetActive(false);
            action?.Invoke();
        };
    }

    #endregion

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

        GameObject _itemGo;
        Debug.Log(attribute.SpineType);
        switch (attribute.SpineType)
        {
            case EColorStateSpineType.EBroomSpine:
                _itemGo = broomItemGo;
                broomSpine.AnimationState.SetAnimation(0, attribute.SpineAnim, false);
                break;
            case EColorStateSpineType.EMagnetSpine:
                _itemGo = magnetItemGo;
                magnetSpine.AnimationState.SetAnimation(0, attribute.SpineAnim, false);
                break;
            case EColorStateSpineType.ECreateSpine:
                _itemGo = createItemGo;
                createSpine.AnimationState.SetAnimation(0, attribute.SpineAnim, false);
                break;
            case EColorStateSpineType.EChangeSpine:
                _itemGo = changeItemGo;
                changeSpine.AnimationState.SetAnimation(0, attribute.SpineAnim, false);
                break;
            // 特殊水块
            case EColorStateSpineType.ERainBowWater:
                _itemGo = RainBowWater;
                break;
   /*         case EColorStateSpineType.EBombBlackWater:
                break;*/
            default:
                Debug.LogWarning("_itemGo 没有匹配到特性");
                _itemGo = null;
                break;
        }
        Debug.Log(_itemGo);
     /*   Debug.Log(_itemGo.transform.Find("Top"));*/
        _itemGo?.Show();
        _itemGo?.transform.Find("Top")?.gameObject.SetActive(isTopWater);
        /*暂存
         * if (attribute.SpineAnim.IsNullOrEmpty() == false && attribute.SpineType > EColorStateSpineType.None && attribute.SpineType < EColorStateSpineType.Max)
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
        }*/
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

    /// <summary>
    /// 统一的道具使用动画方法
    /// </summary>
    /// <param name="otherWater">另一个道具水块</param>
    /// <param name="itemType">道具类型</param>
    /// <param name="onComplete">完成回调</param>
    public void PlayUseItem(BottleWaterCtrl otherWater, ItemType itemType, Action onComplete = null)
    {
        StartCoroutine(CoroutinePlayUseItem(otherWater, itemType, onComplete));
    }

    /// <summary>
    /// 统一的道具动画协程
    /// </summary>
    private IEnumerator CoroutinePlayUseItem(BottleWaterCtrl hide, ItemType itemType, Action onComplete)
    {
        isPlayItemAnim = true;
        hide.gameObject.SetActive(true);

        // 根据道具类型获取对应的资源引用
        var (itemGo, spineComponent, animName, useTopNode) = GetItemResources(itemType);
        var (hideItemGo, hideSpineComponent, _, _) = GetItemResources(itemType);

        if (itemGo == null || hideItemGo == null)
        {
            Debug.LogError($"道具资源未找到: {itemType}");
            yield break;
        }

        // 1. 创建道具实例
        var go = Instantiate(itemGo);
        var go1 = Instantiate(hideItemGo);

        // 2. 设置初始位置
        go.transform.SetParent(transform, false);
        go.transform.localScale = itemGo.transform.localScale;
        go.transform.localPosition = itemGo.transform.localPosition;

        go1.transform.SetParent(transform, false);
        go1.transform.localScale = hideItemGo.transform.localScale;
        go1.transform.localPosition = hideItemGo.transform.localPosition + new Vector3(0, 83.4f, 0);

        var useSpine = go.GetComponent<SkeletonGraphic>();
        var useSpine1 = go1.GetComponent<SkeletonGraphic>();

        yield return new WaitForEndOfFrame();

        // 3. 移动到画布层级
        go.transform.SetParent(LevelManager.Instance.gameCanvas, true);
        go1.transform.SetParent(LevelManager.Instance.gameCanvas, true);

        // 4. 播放Spine动画
        if (useSpine1 != null && hideSpineComponent != null)
        {
            useSpine1.AnimationState.SetAnimation(0, hideSpineComponent.AnimationState.ExpandToIndex(0).Animation.name, false);
        }

        // 5. 移动动画
        go1.transform.DOLocalMove(go.transform.localPosition, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (go1 != null) Destroy(go1);
        });

        // 6. 隐藏原始道具，播放消失动画
        itemGo.SetActive(false);
        if (useTopNode && go.transform.Find("Top") != null)
        {
            go.transform.Find("Top").gameObject.SetActive(false);
        }

        if (useSpine != null)
        {
            useSpine.AnimationState.SetAnimation(0, animName, false);
        }

        isPlayItemAnim = false;
        hide.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        // 7. 回调处理
        onComplete?.Invoke();

        // 8. 清理
        if (go != null) Destroy(go);
    }

    /// <summary>
    /// 根据道具类型获取对应的资源
    /// </summary>
    private (GameObject itemGo, SkeletonGraphic spine, string animName, bool useTopNode) GetItemResources(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.ClearRandomWaterItem => (broomItemGo, broomSpine, "disappear", true),
            ItemType.MakeColorItem => (createItemGo, createSpine, "combine", true),
            ItemType.MagnetItem => (magnetItemGo, magnetSpine, "combine", true),
            ItemType.ChangeGreen or ItemType.ChangeOrange or ItemType.ChangePink
            or ItemType.ChangeYellow or ItemType.ChangePurple or ItemType.ChangeDarkBlue
                => (changeItemGo, changeSpine, "combine", true),
            _ => (null, null, "", false)
        };
    }
    // 保持原有方法的兼容性
    public void PlayUseBroom(BottleWaterCtrl hide)
    {
        PlayUseItem(hide, ItemType.ClearRandomWaterItem, () =>
        {
            hide.bottle.SetBottleColor();
        });
    }

    public void PlayUseCreate(BottleCtrl bottleCtrl, BottleWaterCtrl hide)
    {
        PlayUseItem(hide, ItemType.MakeColorItem, () =>
        {
            bottleCtrl.SetBottleColor();
        });
    }

    public void PlayUseChange(BottleWaterCtrl hide)
    {
        PlayUseItem(hide, ItemType.ChangeGreen, () =>
        {
            // 变色道具的特定逻辑
        });
    }

    public void PlayUseMagnet(BottleWaterCtrl hide)
    {
        PlayUseItem(hide, ItemType.MagnetItem, () =>
        {
            // 磁铁道具的特定逻辑
        });
    }

}
