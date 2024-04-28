using ProjectTools;
using UnityEngine;

[CreateAssetMenu(menuName = "Ragdoll settings")]
public class RagdollSettings : ScriptableObject
{
    public SerializableDictionary<BodyPart, PunishOrRewardContainer> rewardsOnGroundHit = new SerializableDictionary<BodyPart, PunishOrRewardContainer>();

    public SerializableDictionary<BodyPart, float> forceMultiplierOnLimbs = new SerializableDictionary<BodyPart, float>();

    [field: SerializeField] private PunishOrRewardContainer GroundHittingConsequence = new PunishOrRewardContainer();
}
