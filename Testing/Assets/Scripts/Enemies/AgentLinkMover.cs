using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public enum OffMeshLinkMoveMethod
{
    Teleport,
    NormalSpeed,
    Parabola,
    Curve
}

[RequireComponent(typeof(NavMeshAgent))]
public class AgentLinkMover : MonoBehaviour
{
    public Animator animator;
    // public OffMeshLinkMoveMethod m_Method = OffMeshLinkMoveMethod.Parabola;
    public AnimationCurve m_Curve = new AnimationCurve();

    IEnumerator Start()
    {
        // Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), true);
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false;
        while (true) {
            if (agent.isOnOffMeshLink && agent.isActiveAndEnabled) {
                NavMeshLink link = (NavMeshLink)agent.navMeshOwner;
                int areaType = link.area;
                /*
                if (m_Method == OffMeshLinkMoveMethod.NormalSpeed)
                    yield return StartCoroutine(NormalSpeed(agent));
                else if (m_Method == OffMeshLinkMoveMethod.Parabola)
                    yield return StartCoroutine(Parabola(agent)); // height = 2.0f, duration = 0.5f
                else if (m_Method == OffMeshLinkMoveMethod.Curve)
                    yield return StartCoroutine(Curve(agent)); // duration = 0.5f
                */
                if (areaType == 2) {
                    yield return StartCoroutine(Parabola(agent));
                }else {
                    yield return StartCoroutine(Curve(agent)); 
                }
                if (agent.isActiveAndEnabled) {
                    agent.CompleteOffMeshLink();
                }
            }
            yield return null;
        }
    }

    IEnumerator NormalSpeed(NavMeshAgent agent)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        while (agent.transform.position != endPos)
        {
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator Parabola(NavMeshAgent agent)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float duration;
        if (agent.speed < 6) {
            duration = Vector3.Distance(startPos, endPos) / 6;
        }else {
            duration = Vector3.Distance(startPos, endPos) / agent.speed;
        }
        if (duration < 0.7f) {
            duration = 0.7f;
        }
        float height = duration;
        float normalizedTime = 0.0f;
        animator.SetTrigger("isJumping");
        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            if (float.IsNaN(yOffset)) {
                yOffset = 0.1f;
            }
            Vector3 direction = (endPos - startPos).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(ToolMethods.SettingVector(direction.x, 0, direction.z));
            agent.transform.rotation = lookRotation;
            // Debug.Log("Transform Assignment: " + Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up + ", Offset: " + yOffset + ", Vector3 sum: " + yOffset * Vector3.up);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        animator.ResetTrigger("isJumping");
    }

    IEnumerator Curve(NavMeshAgent agent)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        // float duration = Vector3.Distance(startPos, endPos) / (agent.speed / 1.5f);
        float duration;
        if (agent.speed < 6) {
            duration = Vector3.Distance(startPos, endPos) / (6 / 1.5f);
        }else {
            duration = Vector3.Distance(startPos, endPos) / (agent.speed / 1.5f);
        }
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = m_Curve.Evaluate(normalizedTime);
            if (float.IsNaN(yOffset)) {
                yOffset = 0.1f;
            }
            Vector3 direction = (endPos - startPos).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(ToolMethods.SettingVector(direction.x, 0, direction.z));
            agent.transform.rotation = lookRotation;
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }
}