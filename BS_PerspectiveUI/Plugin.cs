using HMUI;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace BS_PerspectiveUI
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger)
        {
            Instance = this;
            Log = logger;
            Log.Info("BS_PerspectiveUI initialized.");

            BS_Utils.Utilities.BSEvents.gameSceneLoaded += onGameSceneLoaded;
        }

        private async void onGameSceneLoaded()
        {
            Plugin.Log.Info("get shader");
            var s = Shader.Find("GUI/Text Shader");
            Plugin.Log.Info(s.ToString());

            Transform UIParent = GameObject.Find("EnergyPanel").transform.parent;
            Plugin.Log.Info(UIParent.ToString());

            await Task.Delay(1000);
            updateAllUI(UIParent, s);

        }
        private Material textMaterial = null;
        private void updateAllUI(Transform root,Shader s)
        {
            // 子オブジェクトの数を取得
            int childCount = root.childCount;

            // 子オブジェクトをループで処理
            for (int i = 0; i < childCount; i++)
            {
                Transform childTransform = root.GetChild(i);

                //update text
                CurvedTextMeshPro text = childTransform.GetComponent<CurvedTextMeshPro>();
                if (text != null)
                {
                    if (textMaterial == null)
                    {
                        textMaterial = new Material(s);
                        textMaterial.mainTexture = text.fontMaterial.mainTexture;
                        textMaterial.renderQueue = 5000;
                    }
                    text.fontMaterial = textMaterial;
                }

                //update imageview
                ImageView img = childTransform.GetComponent<ImageView>();
                if (img != null)
                {
                    Material m = new Material(s);
                    m.mainTexture = img.material.mainTexture;
                    m.renderQueue = 5000;
                    img.material = m;
                }
                // 子オブジェクトの中にさらに子オブジェクトがある場合、再帰的に処理を続行
                updateAllUI(childTransform, s);
            }
        }
        #region BSIPA Config
        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");
            new GameObject("BS_PerspectiveUIController").AddComponent<BS_PerspectiveUIController>();

        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit");

        }
    }
}
