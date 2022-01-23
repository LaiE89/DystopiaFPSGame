using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ToolMethods : MonoBehaviour {

    public static Vector3 OffsetPosition(Vector3 objectPos, float xAxisDiff, float yAxisDiff, float zAxisDiff) {
        Vector3 result;
        result.x = objectPos.x + xAxisDiff;
        result.y = objectPos.y + yAxisDiff;
        result.z = objectPos.z + zAxisDiff;
        return result;
    }
    
    public static Vector3 SettingVector(float xValue, float yValue, float zValue) {
        Vector3 result;
        result.x = xValue;
        result.y = yValue;
        result.z = zValue;
        return result;
    }
}