using System;
using System.Collections;
using System.Linq;
using Microsoft.VisualBasic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public static class Utils
{

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

    public static string[] GetSliderValues(RangeSlider slider)
    {
        return new string[2] { slider.LowValue.ToString(), slider.HighValue.ToString() };
    }

    public static string GenerateRandomId(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        System.Random random = new System.Random();
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}