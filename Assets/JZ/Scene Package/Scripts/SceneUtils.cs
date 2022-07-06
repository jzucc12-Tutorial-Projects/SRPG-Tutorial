using System.Collections;
using UnityEngine.SceneManagement;

namespace JZ.SCENE
{
    /// <summary>
    /// <para>Additional Scene functionality</para>
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// <para>Determines scene name based off of the scene index</para>
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <returns></returns>
        public static string GetSceneNameFromIndex(int sceneIndex)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
            string withExtension = path.Substring(path.LastIndexOf('/') + 1);
            string withoutExtension = withExtension.Substring(0, withExtension.LastIndexOf('.'));
            return withoutExtension;
        }

        /// <summary>
        /// <para>Determines scene index based off of the scene name</para>
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static int GetSceneIndexFromName(string sceneName)
        {
            for(int ii = 0; ii < SceneManager.sceneCountInBuildSettings; ii++)
            {
                string currentName = GetSceneNameFromIndex(ii);
                if(currentName != sceneName) continue;
                return ii;
            }

            return -1;
        }
        
        /// <summary>
        /// <para>Does an additive transition with scene transition data</para>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerator AdditiveTransition(SceneTransitionData data)
        {
            foreach(var scene in data.scenesToUnload)
            {
                if(data.unloadMyScene && scene == data.mySceneName) continue;
                yield return SceneManager.UnloadSceneAsync(scene);
            }

            if(data.unloadMyScene)
                yield return SceneManager.UnloadSceneAsync(data.mySceneName);

            yield return SceneManager.LoadSceneAsync(data.targetSceneIndex, LoadSceneMode.Additive);
        }
    }
}