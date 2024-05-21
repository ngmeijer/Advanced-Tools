using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CSV Data")]
public class CSVData : ScriptableObject
{
    public List<int> EpisodeIDs = new List<int>();
    public List<float> CumulativeRewards = new List<float>();
    public List<float> EpisodeDurations = new List<float>();
    public List<int> CollectiblesCount = new List<int>();
}
