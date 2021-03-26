using UnityEngine;

public class Utils {

    public static Quaternion GetRotationBetweenVectors(Vector3 fromObject, Vector3 toObject)
    {
        Vector3 relativePos = toObject - fromObject;

        // the second argument, upwards, defaults to Vector3.up
        return Quaternion.LookRotation(relativePos, Vector3.up);
    }    
}