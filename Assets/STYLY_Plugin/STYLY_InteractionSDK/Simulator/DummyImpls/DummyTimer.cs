using System.Collections;
using System.Collections.Generic;
using STYLY.Interaction.SDK.V1;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.Dummy
{
    /// <summary>
    /// Timerダミー実装のImplSetupper。
    /// </summary>
    public class DummyTimerSetupper : IImplSetupper
    {
        public void Setup(Component srcComponent)
        {
            var ifComp = srcComponent as ITimer;
            var impl = srcComponent.gameObject.AddComponent<DummyTimer>();
            impl.maxTime = ifComp.MaxTime;
            impl.minTime = ifComp.MinTime;
            impl.time = ifComp.Time;
            impl.startOnAwake = ifComp.StartOnAwake;
            impl.timerType = ifComp.TimerType;
            impl.OnExpireEvent = ifComp.OnExpireEvent;
            ifComp.StartTimerListener += impl.StartTimer;
            ifComp.StopTimerListener += impl.StopTimer;
        }
    }
    /// <summary>
    /// Timerダミー実装。
    /// </summary>
    [AddComponentMenu("Scripts/DummyScript")]
    public class DummyTimer : MonoBehaviour
    {
        [Tooltip("Start timer automatically on Awake.")]
        public bool startOnAwake = false;

        public TimerTypeEnum timerType = TimerTypeEnum.Constant;

        [Tooltip("Sec")]
        public float time; // タイマー時間

        [Tooltip("Sec")]
        public float minTime; // 最小タイマー時間

        [Tooltip("Sec")]
        public float maxTime; // 最大タイマー時間

        float remainingTime; // タイマー残り時間

        bool isRunning = false;

        public UnityEvent OnExpireEvent;

        private void Start()
        {
            if (startOnAwake)
            {
                StartTimer();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if( isRunning )
            {
                remainingTime -= UnityEngine.Time.deltaTime;
                if (remainingTime <= 0)
                {
                    isRunning = false;
                    remainingTime = 0.0f;
                    OnExpireEvent.Invoke();
                }
            }
        }
        
        public void StartTimer()
        {
            switch (timerType)
            {
                case TimerTypeEnum.RandomInRange:
                    remainingTime = Random.Range(minTime, maxTime);
                    break;
                case TimerTypeEnum.Constant:
                    remainingTime = time;
                    break;
            }
            
            if(remainingTime < 0 )
            {
                return;
            }

            isRunning = true;
        }

        public void StopTimer()
        {
            isRunning = false;
        }
    }
    
}
