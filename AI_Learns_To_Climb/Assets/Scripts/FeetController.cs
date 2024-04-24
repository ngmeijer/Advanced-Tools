using UnityEngine;

public class FeetController : BodyPartController
{
    protected void OnCollisionStay(Collision collision)
    {
        if (TouchingGround)
        {
            Debug.Log("Rewarding being grounded.");
            onGiveReward?.Invoke(0.1f);
        }
    }
}
