using UnityEngine;

public class FeetController : BodyPartController
{
    protected void OnCollisionStay(Collision collision)
    {
        if (TouchingGround)
        {
            Debug.Log("Rewarding being grounded.");
            onGiveRewardOrPunishment?.Invoke(0.1f);
        }
    }
}
