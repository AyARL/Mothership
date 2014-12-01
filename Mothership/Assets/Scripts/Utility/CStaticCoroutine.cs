using UnityEngine;
using System.Collections;
using System;

public class CStaticCoroutine : MonoBehaviour
{ 
    static public CStaticCoroutine instance;

    void Awake()
    {
        instance = this;
    }
     
    IEnumerator Perform(IEnumerator coroutine, Action onComplete = null)
    {
        onComplete = onComplete ?? delegate {};
        yield return StartCoroutine(coroutine);
        onComplete();
    }
     
    static public void DoCoroutine(IEnumerator coroutine, Action onComplete = null)
    {
        instance.StartCoroutine(instance.Perform(coroutine, onComplete));
    }
     
}