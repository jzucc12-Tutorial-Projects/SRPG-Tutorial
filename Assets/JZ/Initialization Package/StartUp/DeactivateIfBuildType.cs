using UnityEngine;

namespace JZ.STARTUP
{
    /// <summary>
    /// <para>Changes object activation depending upon game build type</para>
    /// </summary>
    public class DeactivateIfBuildType : DeactivateIf
    {
        [SerializeField] private bool standaloneOnly = true;

        protected override bool DeactivateCheck()
        {
            #if UNITY_WEBGL
            return !standaloneOnly;
            #else
            return standaloneOnly;
            #endif
        }
    }
}
