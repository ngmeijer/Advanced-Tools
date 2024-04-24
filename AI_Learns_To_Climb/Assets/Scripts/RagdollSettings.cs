using ProjectTools;
using UnityEngine;

[CreateAssetMenu(menuName = "Ragdoll settings")]
public class RagdollSettings : ScriptableObject
{
    public SerializableDictionary<BodyPart, CustomKeyValuePair> rewardsOnGroundHit = new SerializableDictionary<BodyPart, CustomKeyValuePair>();

    public SerializableDictionary<BodyPart, float> forceMultiplierOnLimbs = new SerializableDictionary<BodyPart, float>();

    [field: SerializeField] private CustomKeyValuePair GroundHittingConsequence = new CustomKeyValuePair();
}
