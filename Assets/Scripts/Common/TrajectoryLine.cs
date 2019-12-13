using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TrajectoryLine : MonoBehaviour
{
    public Color enemyColor = Color.red;
    public Color playerColor = Color.green;

    public LineRenderer line;

    public float lineWidth = 0.5f;
    public float lineAngle = 30.0f;

    public IEnumerator DrawTrajectoryWhileTime(GameObject source, GameObject target, float displayTime)
    {
        SetColor(source.tag);

        float timer = 0;
        while(timer < displayTime)
        {
            DrawLine(source.transform.position, target.transform.position);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        RemoveLine();
    }

    public IEnumerator DrawTrajectoryWhileTime(GameObject source, Vector3 targetPos, float displayTime)
    {
        SetColor(source.tag);

        float timer = 0;
        while (timer < displayTime)
        {
            DrawLine(source.transform.position, targetPos);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        RemoveLine();
    }

    public IEnumerator DrawTrajectoryWhileInterrupt(GameObject source, GameObject target)
    {
        SetColor(source.tag);

        while (FightSceneController.Instance.GetCurrentFightState() != FightState.Dead
            || FightSceneController.Instance.GetCurrentFightState() != FightState.End)
        {
            DrawLine(source.transform.position, target.transform.position);
            yield return new WaitForEndOfFrame();
        }

        RemoveLine();
    }

    private void DrawLine(Vector3 sourcePos, Vector3 targetPos)
    {
        line.positionCount = 2;

        sourcePos.y = 0.05f;
        targetPos.y = 0.05f;
        
        line.SetPosition(0, sourcePos);
        line.SetPosition(1, targetPos);

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
    }

    public void DrawLineUntilRange(Vector3 sourcePos, Vector3 direction, float attackRange)
    {
        line.positionCount = 2;

        sourcePos.y = 0.05f;

        direction *= attackRange;
        direction.y = 0.05f;
        
        line.SetPosition(0, sourcePos);
        line.SetPosition(1, sourcePos + direction);

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
    }
    public void DrawLineWithAngle(Vector3 sourcePos, Vector3 direction, float attackRange, float angle)
    {
        int pointCount = (int)(angle * 0.5f);
        line.positionCount = pointCount + 2;

        sourcePos.y = 0.05f;

        Vector3 rangeVector = direction * attackRange;
        rangeVector.y = 0.05f;
        Vector3 attackRangePoint = sourcePos + rangeVector;
        
        Vector3 secterVector = direction * (Mathf.Cos(Mathf.PI * ((angle * 0.5f) / 180)) * attackRange);
        secterVector.y = 0.05f;
        Vector3 sectorStartPoint = sourcePos + secterVector;

        line.SetPosition(0, sourcePos);
        line.SetPosition(1, sectorStartPoint);

        int idx = 2;
        float interval = 0.0f;
        while (interval < 1.0f)
        {
            interval += 1.0f / (float)pointCount;
            Vector3 lerpPoint = Vector3.Lerp(sectorStartPoint, attackRangePoint, interval);
            
            line.SetPosition(idx, lerpPoint);
            idx += 1;
        }
        line.SetPosition(line.positionCount - 1, attackRangePoint);

        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0.0f, lineWidth));

        //float sectorWidth = attackRange * Mathf.Sin(Mathf.PI * ((angle * 0.5f) / 180)) * 2;
        float rightside = (line.GetPosition(line.positionCount - 1) - line.GetPosition(1)).magnitude;
        float leftside = attackRange - rightside;
        float keyLerpFactor = rightside / attackRange;
        float lerpAngle = angle * 0.5f;
        keyLerpFactor = keyLerpFactor / lerpAngle;

        float startkey = leftside / attackRange + keyLerpFactor;
        for (float key = startkey; key < 1.0f; key += keyLerpFactor)
        {
            lerpAngle -= 1.0f;
            // 아니 왜 직선인데~~~~
            //float keySectorWidth = attackRange * key *(Mathf.Tan(Mathf.PI * ((lerpAngle) / 180))) * 2.0f;
            float keySectorWidth = 2.0f * attackRange * Mathf.Sin(Mathf.Deg2Rad * lerpAngle);
            Keyframe keyframe = new Keyframe(key, keySectorWidth);
            curve.AddKey(keyframe);

        }
        //Debug.Log(lerpAngle);
        curve.AddKey(new Keyframe(1.0f, 0f));

        line.widthCurve = curve;
    }

    public void RemoveLine()
    {
        StopAllCoroutines();
        ObjectPoolManager.Instance.Free(gameObject);
    }

    public void SetColor(string sourceTag)
    {
        if(sourceTag == "Player")
        {
            line.startColor = playerColor;
            line.endColor = playerColor;
            //line.material.color = playerColor;
        }
        else if(sourceTag == "Enemy")
        {
            line.startColor = enemyColor;
            line.endColor = enemyColor;
            //line.material.color = enemyColor;
        }
    }

    public void SetWidth(float value)
    {
        lineWidth = value;
    }
}
