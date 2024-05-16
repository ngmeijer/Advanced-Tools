using UnityEngine;

public class AgentCollisionManager : MonoBehaviour
{
    [HideInInspector] public CombatMLAgent CombatAgent;

    public void CheckColliderTagOnEnter(GameObject pCollisionObject)
    {
        switch (pCollisionObject.tag)
        {
            case "Obstacle":
                handleObstacleCollision();
                break;
            case "Wall":
                handleWallCollision();
                break;
            case "Collectible":
                handleCollectibleCollision();
                break;
            case "HealthPotion":
                handleHealthPotionCollision();
                break;
            case "Weapon":
                handleWeaponCollision(pCollisionObject);
                break;
            case "Agent":
                if (CombatAgent.AgentWeaponState == WeaponState.CARRY_WEAPON)
                    handleDamageDealing(pCollisionObject);
                break;
            case "InvisibleBarrier":
                CombatAgent.EndEpisode();
                break;
        }
    }

    private void handleHealthPotionCollision()
    {
        CombatAgent.ModifyHealth(CombatAgent.TrainingSettings.HealthPotionValue);
    }

    private void handleWeaponCollision(GameObject pCollisionObject)
    {
        if (CombatAgent.AgentWeaponState == WeaponState.CARRY_WEAPON)
            return;

        CombatAgent.SetWeaponState(WeaponState.CARRY_WEAPON, pCollisionObject);
        CombatAgent.AddReward(CombatAgent.TrainingSettings.ResultOnWeaponPickup);
    }

    private void handleDamageDealing(GameObject pCollisionObject)
    {
        CombatMLAgent enemy = pCollisionObject.GetComponent<CombatMLAgent>();

        enemy.ReceiveWeaponDamage(CombatAgent.TrainingSettings.WeaponDamage);
        if (enemy.CurrentHealth > 0)
        {
            CombatAgent.AddReward(CombatAgent.TrainingSettings.ResultOnDamagedEnemy);
            Debug.Log("Damaged enemy.");
            return;
        }

        CombatAgent.UpdateKD(1, 0);
        CombatAgent.OnAgentKill.Invoke(enemy, CombatAgent);
        Debug.Log("Killed enemy");
    }

    private void handleObstacleCollision()
    {
        CombatAgent.ModifyHealth(CombatAgent.TrainingSettings.ObstacleDamage);
        CombatAgent.AddReward(CombatAgent.TrainingSettings.ResultOnObstacleHit);
        //_collidedWithDamageDealer = true;
    }

    private void handleWallCollision()
    {
        CombatAgent.ModifyHealth(CombatAgent.TrainingSettings.WallDamage);
        CombatAgent.AddReward(CombatAgent.TrainingSettings.ResultOnWallHit);
        //_collidedWithDamageDealer = true;
    }

    private void handleCollectibleCollision()
    {
        CombatAgent.ModifyCollectibleCount(1);
        CombatAgent.AddReward(CombatAgent.TrainingSettings.ResultOnCollectibleHit);
        CombatAgent.OnFoundCollectible?.Invoke();
    }
}