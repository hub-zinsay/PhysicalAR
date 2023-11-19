using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.V1
{    
    /// <summary>
    /// Timer時間指定のタイプ
    /// </summary>
    public enum TimerTypeEnum
    {
        Constant,        // 定数で指定
        RandomInRange    // Min-Max範囲のランダムな範囲で指定
    }
    
    /// <summary>
    /// FeatureComponent TimerのInterface。
    /// </summary>
    public interface ITimer
    {
        bool StartOnAwake { get; set; }
        TimerTypeEnum TimerType { get; set; }
        float Time { get; set; }
        float MinTime { get; set; }
        float MaxTime { get; set; }
        UnityEvent OnExpireEvent { get; set; }
        
        UnityAction StartTimerListener { get; set; }
        
        UnityAction StopTimerListener { get; set; }
    }
}