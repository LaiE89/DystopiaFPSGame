using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue {
    public string name;
    public bool isCenter;

    [TextArea(3, 10)]
    public string[] sentences;

    [Header("Enemy Objectives Stuff")]
    public bool isObjective;
    public bool isCancellable;

    [Header("Trigger Type")]
    public bool onTarget;
    public bool onDeath;
    public bool onAttack;
    public bool onMouse;
    public bool onHurt;
}
