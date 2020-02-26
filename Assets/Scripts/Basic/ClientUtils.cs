﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ClientUtils
{
    public static Color HTMLColorToColor(string htmlColor)
    {
        Color cl = new Color();
        ColorUtility.TryParseHtmlString(htmlColor, out cl);
        return cl;
    }

    public static Vector3 GenerateRandomPosInsideCollider(BoxCollider bc)
    {
        float x = Random.Range(bc.center.x - bc.size.x * 0.5f, bc.center.x + bc.size.x * 0.5f);
        float y = Random.Range(bc.center.y - bc.size.y * 0.5f, bc.center.y + bc.size.y * 0.5f);
        float z = Random.Range(bc.center.z - bc.size.z * 0.5f, bc.center.z + bc.size.z * 0.5f);
        Vector3 localPos = new Vector3(x, y, z);
        Vector3 worldPos = bc.transform.TransformPoint(localPos);
        return worldPos;
    }

    public static SortedDictionary<int, List<T>> GetRank<T>(this IEnumerable<T> value, IComparer<T> comparer)
    {
        List<T> list = value.ToList<T>();
        list.Sort(comparer); // 排序  Sort方法排序后的结果是升序
        list.Reverse(); //反转List中的数据  就变成降序了

        SortedDictionary<int, List<T>> rank = new SortedDictionary<int, List<T>>();

        int order = 0;
        int pcount = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (!rank.ContainsKey(order))
            {
                rank.Add(order, new List<T> { });
            }

            if (i < list.Count && i > 0)
            {
                int result = comparer.Compare(list[i], list[i - 1]);
                if (result != 0)
                {
                    if (pcount == 0)
                    {
                        order++;
                    }
                    else
                    {
                        order = order + 1 + pcount;
                        pcount = 0;
                    }
                }
                else
                {
                    pcount++;
                }
            }

            if (!rank.ContainsKey(order))
            {
                rank.Add(order, new List<T> { });
            }

            rank[order].Add(list[i]);
        }

        for (int i = 0; i < 4; i++)
        {
            if (!rank.ContainsKey(i))
            {
                rank.Add(i, new List<T> { });
            }
        }

        return rank;
    }

    public static float DistanceBetweenPointToSegment(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
    {
        Vector3 S_P = point - segmentStart;
        Vector3 S_E = segmentEnd - segmentStart;
        Vector3 E_P = point - segmentEnd;
        Vector3 E_S = -S_E;

        if (Vector3.Dot(S_P, S_E) <= 0)
        {
            return S_P.magnitude;
        }

        if (Vector3.Dot(E_P, E_S) <= 0)
        {
            return E_P.magnitude;
        }

        return (Vector3.Dot(S_P, S_E) / S_E.magnitude * S_E.normalized - S_P).magnitude;
    }

    public static List<T> GetRandomFromList<T>(List<T> OriList, int number, List<T> exceptList = null)
    {
        if (OriList == null || OriList.Count == 0) return new List<T>();

        List<T> ori = OriList.ToArray().ToList();
        if (exceptList != null)
        {
            List<T> remove = new List<T>();
            foreach (T t in ori)
            {
                if (exceptList.Contains(t))
                {
                    remove.Add(t);
                }
            }

            foreach (T removeT in remove)
            {
                ori.Remove(removeT);
            }
        }

        if (number > ori.Count) number = ori.Count;

        HashSet<int> indices = new HashSet<int>();
        while (indices.Count < number)
        {
            int index = Random.Range(0, ori.Count);
            if (!indices.Contains(index))
            {
                indices.Add(index);
            }
        }

        List<T> res = new List<T>();
        foreach (int i in indices)
        {
            res.Add(ori[i]);
        }

        return res;
    }

    public static string TimeToString(float timeTick)
    {
        return Mathf.FloorToInt(timeTick / 60f) + ":" + Mathf.FloorToInt(timeTick % 60).ToString().PadLeft(2, '0');
    }

    public static string TimeToString_Milisecond(float timeTick)
    {
        return "." + Mathf.CeilToInt(timeTick % 1 * 10f);
    }

    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static Vector3 GetIntersectWithLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
    {
        float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
        return d * direct.normalized + point;
    }

    public static float GetClipLength(Animator animator, string clip)
    {
        if (null == animator || string.IsNullOrEmpty(clip) || null == animator.runtimeAnimatorController)
            return 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        AnimationClip[] tAnimationClips = ac.animationClips;
        if (null == tAnimationClips || tAnimationClips.Length <= 0) return 0;
        AnimationClip tAnimationClip;
        for (int tCounter = 0, tLen = tAnimationClips.Length; tCounter < tLen; tCounter++)
        {
            tAnimationClip = ac.animationClips[tCounter];
            if (null != tAnimationClip && tAnimationClip.name == clip)
                return tAnimationClip.length;
        }

        return 0F;
    }

    public static Vector3 TryToMove(this Player player, Vector3 destPos, float colliderRadius, int invokeCount = 0)
    {
        if (invokeCount >= 100)
        {
            Debug.Log("GiveUp");
            return player.transform.position;
        }

        Vector3 dest = destPos.PlanerizeVector3(player.transform.position.y);
        Vector3 dir = destPos - player.transform.position;

        RaycastHit[] hits = player.PlayerControl.PlayerCollider.SweepTestAll(dir, dir.magnitude);
        //Debug.Log(hits.Length);

        List<RaycastHit> validHits = new List<RaycastHit>();
        List<Vector3> normalDirs = new List<Vector3>();

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            int layer = hit.collider.gameObject.layer;
            if (layer == GameManager.Instance.Layer_RangeOfActivity)
            {
                validHits.Add(hit);
                normalDirs.Add(hit.normal);
            }
        }

        // Cross异号且Dot全部小于0 则无法移动

        if (validHits.Count > 0)
        {
            if (validHits.Count == 1)
            {
                RaycastHit hit = validHits[0];
                Vector3 parallel = Quaternion.Euler(0, 90, 0) * hit.normal.PlanerizeVector3(0);
                Vector3 tangentMovement = Vector3.Dot(parallel, dir) / parallel.magnitude * parallel.normalized * 0.8f;
                dest = hit.point + hit.normal * colliderRadius + tangentMovement;
                return player.TryToMove(dest, colliderRadius, invokeCount + 1);
            }
            else
            {
                return player.transform.position;
                for (int i = 0; i < validHits.Count; i++)
                {
                    for (int j = i + 1; j < validHits.Count; j++)
                    {
                        if ((-dir).InBetweenTwoVectors(normalDirs[i], normalDirs[j]))
                        {
                            return player.transform.position;
                        }
                    }
                }
            }

            return player.transform.position;
        }

        return dest.PlanerizeVector3(player.transform.position.y);
    }

    private static bool InBetweenTwoVectors(this Vector3 me, Vector3 v1, Vector3 v2)
    {
        float signAngle1 = Vector3.SignedAngle(me, v1, Vector3.up);
        float signAngle2 = Vector3.SignedAngle(me, v2, Vector3.up);
        if (signAngle1 * signAngle2 < 0 && Mathf.Abs(signAngle1) + Mathf.Abs(signAngle2) < 180f)
        {
            return true;
        }

        return false;
    }

    public static Vector3 PlanerizeVector3(this Vector3 vector3, float y)
    {
        Vector3 res = Vector3.Scale(new Vector3(1, 0, 1), vector3);
        res.y = y;
        return res;
    }
}