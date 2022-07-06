namespace JZ.MENU.BUTTON.SCENE
{
    /// <summary>
    /// <para>Reloads the current scene</para>
    /// </summary>
    public class ReloadSceneButton : SceneButtonFunction
    {
        protected override void Awake()
        {
            base.Awake();
            transitionData.targetSceneIndex = transitionData.mySceneIndex;
        }
    }
}