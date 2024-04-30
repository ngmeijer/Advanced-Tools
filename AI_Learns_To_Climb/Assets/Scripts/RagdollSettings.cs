using ProjectTools;
using UnityEngine;

[CreateAssetMenu(menuName = "Ragdoll settings")]
public class RagdollSettings : ScriptableObject
{
    public SerializableDictionary<BodyPart, PunishOrRewardContainer> rewardsOnGroundHit = new SerializableDictionary<BodyPart, PunishOrRewardContainer>();
}
