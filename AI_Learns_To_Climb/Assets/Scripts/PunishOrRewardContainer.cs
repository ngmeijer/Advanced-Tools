using UnityEngine;

[System.Serializable]
public struct PunishOrRewardContainer
{
    public PUNISH_OR_REWARD Type;
    [Range(-1, 1)] public float Amount;
}
