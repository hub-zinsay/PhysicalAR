#if PLAYMAKER

using HutongGames.PlayMakerEditor;
using UnityEditor;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    /// <summary>
    /// VideoInit.csのEditor拡張
    /// </summary>
    [CustomActionEditor(typeof(VideoInit))]
    public class VideoInitCustomActionEditor : CustomActionEditor
    {
        //オプションを開いているかどうかのフラグ
        private bool isOpen = false;
        
        public override bool OnGUI()
        {
            var action = target as VideoInit;
            
            //カスタムアクションの拡張前の画面を表示する
            //編集したらTrueが返ってくる
            var isDirty = DrawDefaultInspector();

            //折りたたみUIを表示、現在開いているかを取得
            isOpen = EditorGUILayout.Foldout(isOpen, "Additional Option");

            //開いている時はGUI追加
            if(isOpen)
            {
                action.useProxyOnWebGL.Value = EditorGUILayout.Toggle("Use proxy on WebGL",action.useProxyOnWebGL.Value);
                if (!action.useProxyOnWebGL.Value)
                {
                    //HelpBox表示
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        EditorGUILayout.HelpBox(
                            "This option is only valid when playing on web browsers." +
                            "If this option is enabled, the player will connect to servers through proxy." +
                            "So it may avoid blocking by CORS restriction.",
                            MessageType.Warning);
                    }
                    EditorGUILayout.EndVertical();
                }
            } 
            
            //編集したかどうか
            return isDirty || GUI.changed;
        }
    }
}
#endif
