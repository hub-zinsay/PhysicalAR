using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// Scene Storage part of StylyServiceForPlayMaker (partial class)
    /// </summary>
    public partial class StylyServiceForPlayMaker
    {
        private IStylyServiceSceneStorageImpl sceneStorageImpl;

        /// <summary>
        /// シーンIDのフォーマット検証
        /// </summary>
        /// <returns>正常であればtrue</returns>
        public static bool ValidateSceneId(string sceneId)
        {
            var guidRegEx = new Regex(@"^([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}$");
            return guidRegEx.IsMatch(sceneId);
        }
        
        /// <summary>
        /// インターフェースをセット
        /// </summary>
        /// <param name="impl">実際の機能の中身</param>
        public void SetSceneStorageImpl(IStylyServiceSceneStorageImpl impl)
        {
            this.sceneStorageImpl = impl;
        }
        
        /// <summary>
        /// シーン単位の領域から文字列を読み込む。
        /// 明示的にシーンIDを指定することで再生中以外のシーンの値を読み込めるが、シーンクリエイターIDをまたいで参照はできない。
        /// </summary>
        /// <param name="key">キー文字列</param>
        /// <param name="defaultValue">キーがない場合のデフォルト値</param>
        /// <param name="sceneId">任意のシーンID指定。nullの場合は現在のシーン指定となる。</param>
        /// <returns>キーがない場合にはdefaultValueが返される</returns>
        public string SceneStorageReadString(string key, string defaultValue, string sceneId = null)
        {
            return GetSceneStorageImpl("SceneStorageReadString")?.SceneStorageReadString(key, defaultValue, sceneId);
        }

        /// <summary>
        /// シーン単位の領域へ文字列を書き込む。
        /// 明示的にシーンIDを指定することで再生中以外のシーンの値を書き込めるが、シーンクリエイターIDをまたぐことはできない。
        /// </summary>
        /// <param name="key">キー文字列</param>
        /// <param name="value">書き込む値</param>
        /// <param name="sceneId">任意のシーンID指定。nullの場合は現在のシーン指定となる。</param>
        public void SceneStorageWriteString(string key, string value, string sceneId = null)
        {
            GetSceneStorageImpl("SceneStorageWriteString")?.SceneStorageWriteString(key, value, sceneId);
        }

        /// <summary>
        /// シーン単位の領域のキー情報を削除する。
        /// </summary>
        /// <param name="key">キー文字列</param>
        /// <param name="sceneId">任意のシーンID指定。nullの場合は現在のシーン指定となる。</param>
        public void SceneStorageDeleteKey(string key, string sceneId = null)
        {
            GetSceneStorageImpl("SceneStorageDeleteKey").SceneStorageDeleteKey(key, sceneId);
        }
        
        /// <summary>
        /// SceneStorageのimplがあればそれを返却し、ない場合はエラーログを出力してnullを返す便利メソッド
        /// </summary>
        /// <param name="actionName">ログ表示用アクション名</param>
        /// <returns>IStylyServiceSceneStorageImplの実装、またはなければnull</returns>
        private IStylyServiceSceneStorageImpl GetSceneStorageImpl(string actionName)
        {
            // アップロード環境向けに、Unity Editor環境でimplが入っていなければダミーimplをセットする。
            if (sceneStorageImpl == null && Application.isEditor)
            {
                SetSceneStorageImpl(new DummySceneStorageImpl());
            }
            if (sceneStorageImpl == null)
            {
                var msg = $"<{actionName}> called, but the IStylyServiceSceneStorageImpl implementation is not available.";
                Debug.LogError(msg);
            }
            return sceneStorageImpl;
        }
    }
}
