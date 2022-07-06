using UnityEngine;
using JZ.SCENE;
using UnityEngine.SceneManagement;

namespace JZ.MENU.BUTTON.SCENE
{
    /// <summary>
    /// <para>Parent class for buttons that are to cause scene transitions</para>
    /// </summary>
    public abstract class SceneButtonFunction : ButtonFunction
    {
        private SceneTransitioner sceneTransitioner => FindObjectOfType<SceneTransitioner>();
        [SerializeField] protected SceneTransitionData transitionData;
        private bool animateTransition => transitionData.animationType != AnimType.none;


        #region //Monobehvaiour
        protected override void Awake()
        {
            base.Awake();
            transitionData.mySceneIndex = gameObject.scene.buildIndex;
            transitionData.mySceneName = gameObject.scene.name;
        }
        #endregion

        #region //Loading
        public override void OnClick()
        {
            if(animateTransition)
            {
                sceneTransitioner.TransitionToScene(transitionData);
            }
            else if(transitionData.additiveLoad)
            {
                StartCoroutine(Utils.AdditiveTransition(transitionData));
            }
            else
            {
                SceneManager.LoadScene(transitionData.targetSceneIndex);
            }
        }
        #endregion
    }
}