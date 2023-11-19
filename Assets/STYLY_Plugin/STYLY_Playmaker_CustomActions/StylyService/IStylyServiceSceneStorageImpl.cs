using System;

namespace STYLY
{
    /// <summary>
    /// interface to handle Scene Storage requests
    /// </summary>
    public interface IStylyServiceSceneStorageImpl
    {
        /// <summary>
        /// シーン単位の領域から文字列を読み込む。
        /// 明示的にシーンIDを指定することで再生中以外のシーンの値を読み込めるが、シーンクリエイターIDをまたいで参照はできない。
        /// </summary>
        /// <param name="key">キー文字列</param>
        /// <param name="defaultValue">キーがない場合のデフォルト値</param>
        /// <param name="sceneId">任意のシーンID指定。nullの場合は現在のシーン指定となる。</param>
        /// <returns>キーがない場合にはdefaultValueが返される</returns>
        string SceneStorageReadString(string key, string defaultValue, string sceneId = null);
        
        /// <summary>
        /// シーン単位の領域へ文字列を書き込む。
        /// 明示的にシーンIDを指定することで再生中以外のシーンの値を書き込めるが、シーンクリエイターIDをまたぐことはできない。
        /// </summary>
        /// <param name="key">キー文字列</param>
        /// <param name="value">書き込む値</param>
        /// <param name="sceneId">任意のシーンID指定。nullの場合は現在のシーン指定となる。</param>
        void SceneStorageWriteString(string key, string value, string sceneId = null);
        
        /// <summary>
        /// シーン単位の領域のキー情報を削除する。
        /// </summary>
        /// <param name="key">キー文字列</param>
        /// <param name="sceneId">任意のシーンID指定。nullの場合は現在のシーン指定となる。</param>
        void SceneStorageDeleteKey(string key, string sceneId = null);
    }
}
