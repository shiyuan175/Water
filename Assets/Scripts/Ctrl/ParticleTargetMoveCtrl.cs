using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleTargetMoveCtrl : MonoBehaviour
{
    private ParticleSystem par;
    private ParticleSystem.Particle[] arrPar;
    private int arrCount;
    private Vector3 wPos;

    [Header("粒子移动目标")]
    public Transform target;
    [Header("粒子移动基础速度")]
    public float speed = 0.1f;
    [Header("延迟激活时间（秒）")]
    public float delayTime = 1f;
    [Header("粒子系统原始发射速率")]
    public float originEmissionRate;
    [Header("粒子是否激活移动状态")]
    public bool isActive;

    [Header("速度叠加值")]
    public float speedAdd;
    [Header("速度叠加增长速率")]
    public float speedAddDelta = 2f;
    [Header("是否一次性爆发粒子模式（Burst）")]
    public bool oncePar = false;

    private void Awake()
    {
        par = GetComponent<ParticleSystem>();
        arrPar = new ParticleSystem.Particle[par.main.maxParticles];
        speedAdd = 0f;
        if (oncePar)
            originEmissionRate = par.emission.GetBurst(0).count.curveMultiplier;
        else
            originEmissionRate = par.emission.rateOverTimeMultiplier;
    }

    private void setActive()
    {
        isActive = true;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Play(10);
        }

        if (!isActive || !par)
        {
            return;
        }
        // 获取当前粒子数组和数量
        arrCount = par.GetParticles(arrPar);
        // 如果没有粒子，停止粒子系统并重置状态
        if (arrCount < 1)
        {
            isActive = false;
            par.Stop();
            speedAdd = 0f;
        }
        else
        {
            speedAdd += Time.unscaledDeltaTime * speedAddDelta;
        }

        // 遍历粒子，飞向wPos
        for (int i = 0; i < arrCount; i++)
        {
            // 当前粒子到目标的向量
            Vector3 vector = wPos - arrPar[i].position;
            // 如果向量长度小于等于速度，则将粒子位置设置为目标位置并结束其生命周期
            if (vector.magnitude <= (speed + speedAdd) * Time.unscaledDeltaTime)
            {
                arrPar[i].position = wPos;
                arrPar[i].remainingLifetime = 0f;
                //par.emissionRate = 0f;
                var emis = par.emission;
                emis.rateOverTimeMultiplier = 0;
            }
            else
            {
                //arrPar[i].position += vector.normalized * (speed + speedAdd) * Time.unscaledDeltaTime;
                arrPar[i].position += vector.normalized * (speed + speedAdd);
                //arrPar[i].position = Vector3.Lerp(arrPar[i].position, Vector3.zero, this.speed * Time.unscaledDeltaTime);
            }
        }
        par.SetParticles(arrPar, arrCount);
    }

    public void Play(Vector3 _pos, int emit_count)
    {
        //if (par.isStopped)
        //{
        wPos = _pos;
        Invoke("setActive", delayTime + 0.01f);
        if (par == null)
        {
            par = this.GetComponent<ParticleSystem>();
        }
        if (oncePar)
        {
            var b = par.emission.GetBurst(0);
            b.count = new ParticleSystem.MinMaxCurve(originEmissionRate);
            //b.minCount = (short)originEmissionRate;
            //b.maxCount = (short)originEmissionRate;
            par.emission.SetBurst(0, b);
        }
        else
        {
            //par.emissionRate = originEmissionRate;
            var emis = par.emission;
            emis.rateOverTimeMultiplier = originEmissionRate;
        }
        speedAdd = 0;
        isActive = false;
        par.Stop();
        //par.Play(withChildren: true);
        //par.Play();
        par.Emit(emit_count);
        //pointDic.Clear();
        //}
    }

    public void Play(int emit_count)
    {
        wPos = target.transform.position;
        // 延迟激活
        Invoke("setActive", delayTime + 0.01f);

        if (par == null)
        {
            par = this.GetComponent<ParticleSystem>();
        }

        if (oncePar)
        {
            var b = par.emission.GetBurst(0);
            b.count = new ParticleSystem.MinMaxCurve(originEmissionRate);
            par.emission.SetBurst(0, b);
        }
        else
        {
            var emis = par.emission;
            emis.rateOverTimeMultiplier = originEmissionRate;
        }

        speedAdd = 0;
        isActive = false;
        par.Stop();
        par.Emit(emit_count);
    }
}