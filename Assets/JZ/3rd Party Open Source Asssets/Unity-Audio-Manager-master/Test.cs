using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [ScriptableObjectDropdown(typeof(SfxContainer))]
    public SfxContainer sfx;
    
    void Start()
    {
        StartCoroutine("play");
    }

    IEnumerator play() {
        while (true) {
            AudioManager.Instance().PlayOneShot(sfx, Vector3.zero);
            yield return new WaitForSeconds(1);
        }
    }
}
