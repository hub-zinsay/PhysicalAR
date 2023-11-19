namespace STYLY.MaintenanceTool.Utility
{
    /// <summary>
    /// プラットフォームユーティリティーのインターフェース
    /// <para>派生クラス: PCVRUtility, AndroidVRUtility等</para>
    /// </summary>
    public interface IPlatformUtility
    {
        int StartSTYLY();

        int StartSTYLY(string urlScheme);

        string CreateURLScheme(string guid, string userName = null, bool testMode = false);

        void ClearSTYLYTestMode();

        void SaveSceneXmlToSTYLYTestMode(string sceneXml);

        int CopyBuildedAssetBundleToSTYLYTestMode(string guid);
    }
}
