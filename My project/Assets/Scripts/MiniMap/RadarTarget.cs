using System.Collections.Generic;
using UnityEngine;

public class RadarTarget : MonoBehaviour
{
    public enum Team { Enemy, Ally }
    public Team team = Team.Enemy;

    public static readonly List<RadarTarget> All = new List<RadarTarget>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetList() { All.Clear(); }

    void OnEnable() { All.Add(this); }
    void OnDisable() { All.Remove(this); }
}