using System;
using System.Collections;
using UnityEngine;

public static class Utils {

    public static Quaternion GetRotationBetweenVectors(Vector3 fromObject, Vector3 toObject)
    {
        Vector3 relativePos = toObject - fromObject;

        // the second argument, upwards, defaults to Vector3.up
        return Quaternion.LookRotation(relativePos, Vector3.up);
    }    

    public static void Invoke(this MonoBehaviour mb, Action f, float delay)
    {
        mb.StartCoroutine(InvokeRoutine(f, delay));
    }
 
    private static IEnumerator InvokeRoutine(System.Action f, float delay)
    {
        yield return new WaitForSeconds(delay);
        f();
    }
}