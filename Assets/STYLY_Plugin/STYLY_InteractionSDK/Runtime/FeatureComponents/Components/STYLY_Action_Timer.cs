using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// 指定時間のタイマーを実行するAction。
    /// タイマー時間は定数値、または一定の範囲内のランダムな値を指定可能。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("STYLY/STYLY_Action_Timer")]
    public class STYLY_Action_Timer : FeatureComponentBase, ITimer
    {
        public bool StartOnAwake { get => startOnAwake; set => startOnAwake = value; }
        public TimerTypeEnum TimerType { get => timerType; set => timerType = value; }
        public float Time { get => time; set => time = value; }
        public float MinTime { get => minTime; set => minTime = value; }
        public float MaxTime { get => maxTime; set => maxTime = value; }
        public UnityEvent OnExpireEvent { get => onExpireEvent; set => onExpireEvent = value; }
        public UnityAction StartTimerListener { get; set; }
        public UnityAction StopTimerListener { get; set; }

        [SerializeField,Tooltip("Start timer automatically on Awake.")]
        bool startOnAwake = false;

        [SerializeField] 
        TimerTypeEnum timerType;

        [SerializeField,Tooltip("Sec")]
        float time = 0f; // タイマー時間

        [SerializeField, FormerlySerializedAs("min_time"),Tooltip("Sec")]
        float minTime = 0f; // 最小タイマー時間

        [SerializeField, FormerlySerializedAs("max_time"),Tooltip("Sec")]
        float maxTime = 1f; // 最大タイマー時間

        float remainingTime; // タイマー残り時間

        [SerializeField, FormerlySerializedAs("OnExpireEvent")]
        UnityEvent onExpireEvent;
        
        public void StartTimer()
        {
            if(StartTimerListener!=null){ StartTimerListener.Invoke();}
        }

        public void StopTimer()
        {
            if(StopTimerListener!=null){StopTimerListener.Invoke();}
        }
    }
}