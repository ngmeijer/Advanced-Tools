using UnityEngine;

public class FeetController : BodyPartController
{
    protected override void OnCollisionStay(Collision collision)
    {
        if (TouchingGround)
        {
            OnGiveReward(part, 0.1f);
        }
    }
}
