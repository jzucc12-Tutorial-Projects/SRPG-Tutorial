using UnityEngine;

namespace JZ.MENU.BUTTON.SCENE
{
    /// <summary>
    /// <para>Changes to player specified scene on press</para>
    /// </summary>
    public class SceneChangeButton : SceneButtonFunction
    {
        [SerializeField] string targetScene;

        protected override void Awake()
        {
            base.Awake();
            transitionData.targetSceneIndex = JZ.SCENE.Utils.GetSceneIndexFromName(targetScene);
        }
    }
}