using System.Collections.Generic;
using System;
using System.Linq;
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
    
    public static Vector2 SettingVector2(float xValue, float yValue) {
        Vector2 result;
        result.x = xValue;
        result.y = yValue;
        return result;
    }

    public static Vector3 SettingVector(float xValue, float yValue, float zValue) {
        Vector3 result;
        result.x = xValue;
        result.y = yValue;
        result.z = zValue;
        return result;
    }

    public static Quaternion SettingQuaternion(float xValue, float yValue, float zValue, float wValue) {
        Quaternion result;
        result.x = xValue;
        result.y = yValue;
        result.z = zValue;
        result.w = wValue;
        return result;
    }

    public static Quaternion OffsetQuaternion(Quaternion objectPos, float xAxisDiff, float yAxisDiff, float zAxisDiff, float wAxisDiff) {
        Quaternion result;
        result.x = objectPos.x + xAxisDiff;
        result.y = objectPos.y + yAxisDiff;
        result.z = objectPos.z + zAxisDiff;
        result.w = objectPos.w + wAxisDiff;
        return result;
    }

    public static void AlertRadius(float radius, Vector3 alertPosition, Vector3 targetPosition, LayerMask mask) {
        Collider[] list = Physics.OverlapSphere(alertPosition, radius, mask);
        //Debug.Log(string.Join<Collider>(", ", list));
        for (int i = 0; i < list.Length; i++) {
            Enemies.Movement enemyScript = list[i].GetComponent<Enemies.Movement>();
            if (enemyScript != null && enemyScript.agent.enabled && !enemyScript.alreadyAttacked && enemyScript.isAlertable) {
                //Debug.Log("Alerted: " + enemyScript);
                enemyScript.agent.SetDestination(targetPosition);
            }
        } 
    }

    public static int LayerMaskToLayer(LayerMask layerMask) {
        return (int) Mathf.Log(layerMask.value, 2);
    }

    public static bool checkInArray (object item, object[] list) {
        foreach(object i in list) {
            if (i == item) {
                return true;
            }
        }
        return false;
    }

    public static void ResetAllAnimatorTriggers(Animator animator) {
        foreach (var trigger in animator.parameters) {
            if (trigger.type == AnimatorControllerParameterType.Trigger) {
                animator.ResetTrigger(trigger.name);
            }
        }
    }
}