using UnityEditor;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// STYLY_Action_Timerのインスペクターエディタ拡張。
    /// </summary>
    [CustomEditor( typeof(STYLY_Action_Timer))]
    public class STYLY_Action_TimerInspector : Editor
    {
        SerializedProperty startOnAwakeProperty;
        SerializedProperty timerTypeProperty;
        SerializedProperty timeProperty;
        SerializedProperty minTimeProperty;
        SerializedProperty maxTimeProperty;
        SerializedProperty onExpireEventProperty;

        public TimerTypeEnum timerType;
        
        private void OnEnable()
        {
            startOnAwakeProperty = serializedObject.FindProperty("startOnAwake");
            timerTypeProperty = serializedObject.FindProperty("timerType");
            timeProperty = serializedObject.FindProperty("time");
            minTimeProperty = serializedObject.FindProperty("minTime");
            maxTimeProperty = serializedObject.FindProperty("maxTime");
            onExpireEventProperty = serializedObject.FindProperty("onExpireEvent");
        }

        public override void OnInspectorGUI()
        {
            STYLY_Action_Timer target = (STYLY_Action_Timer) this.target;
            EditorGUILayout.PropertyField(startOnAwakeProperty);
            EditorGUILayout.PropertyField(timerTypeProperty);

            // TimerTypeに応じて表示するプロパティを切り替える。
            if (target.TimerType == TimerTypeEnum.Constant)
            {
                // 定数時間の場合
                EditorGUILayout.PropertyField(timeProperty);
            }
            else
            {
                // 範囲内ランダム時間の場合
                EditorGUILayout.PropertyField(minTimeProperty);
                EditorGUILayout.PropertyField(maxTimeProperty);
            }
            EditorGUILayout.PropertyField(onExpireEventProperty);
            
            // Apply changes to the serializedProperty - always do this at the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }
    }
}
