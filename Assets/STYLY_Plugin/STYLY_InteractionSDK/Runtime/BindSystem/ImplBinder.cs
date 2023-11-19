using System;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine.Assertions;
#endif
namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// FeatureComponentに実装部をバインドするシングルトンクラス。
    /// setupperGetterを渡して初期化し、利用する。
    /// </summary>
    public class ImplBinder
    {
        private Func<Type, IImplSetupper> setupperGetter;

        private bool initialized = false;
        private static ImplBinder instance;

        public static ImplBinder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ImplBinder();
                }

                return instance;
            }
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="setupperGetter">Typeに応じたImpleSetupperを返すDelegate</param>
        public void Init(Func<Type, IImplSetupper> setupperGetter)
        {
            this.setupperGetter = setupperGetter;
            initialized = true;
        }

        /// <summary>
        /// 実装部のバインド処理
        /// </summary>
        /// <param name="component">対象コンポーネント</param>
        public void Bind(Component component)
        {
            if (!initialized)
            {
#if UNITY_EDITOR
                LoadSimulator();
#else
                Debug.Log("Never initialized.");
                return;
#endif
            }

            IImplSetupper implSetupper = setupperGetter(component.GetType());

            if (implSetupper != null)
            {
                implSetupper.Setup(component);
            }
            else
            {
                Debug.Log("Setupper is not found!:" + component);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Simulatorプレファブをロードする。
        /// </summary>
        void LoadSimulator()
        {
            /// STYLY開発プロジェクトの場合は読み込まない
            if ((PlayerSettings.companyName.Equals("Psychic VR Lab") || PlayerSettings.companyName.Equals("PsychicVRLab"))&&
                (PlayerSettings.productName.IndexOf("STYLY") >= 0 ))
            {
                Debug.Log("Cannot use simulator in STYLY project");
                return;
            }
            
            var simulatorPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(SimulatorPrefabPath, typeof(GameObject));
            var simulatorGameObject = PrefabUtility.InstantiatePrefab(simulatorPrefab) as GameObject;
            Assert.IsNotNull(simulatorGameObject, "Error cannot instantiate simulator prefab");
        }
        
        const string simulatorPathInAssets = "Assets/STYLY_Plugin/STYLY_InteractionSDK/Simulator/Prefabs/STYLY_Simulator.prefab";
        const string simulatorPathInPackages = "Packages/com.psychicvrlab.styly-interaction-sdk/Simulator/Prefabs/STYLY_Simulator.prefab";

        string SimulatorPrefabPath
        {
            get
            {
                if (File.Exists(simulatorPathInAssets))
                {
                    return simulatorPathInAssets;
                }
                else
                {
                    return simulatorPathInPackages;
                }
            }
        }
#endif
    }
}