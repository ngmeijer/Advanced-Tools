using UnityEngine;


[CreateAssetMenu(menuName = "Reinforcement Learning consequences")]
public class AgentReinforcementLearningData : ScriptableObject
{
    [Header("Agent properties")]
    [Range(1, 100)][SerializeField] private int _maxHealth;
    public int MaxHealth => _maxHealth;

    [Range(1, 50)][SerializeField] private float _moveSpeed = 10;
    public float MoveSpeed => _moveSpeed;

    [Range(-100, 0)][SerializeField] private int _weaponDamage = 60;
    public int WeaponDamage => _weaponDamage;

    [Range(1, 100)][SerializeField] private int _healthPotionValue;
    public int HealthPotionValue => _healthPotionValue;

    [Range(-100, 0)][SerializeField] private int _wallDamage;
    public int WallDamage => _wallDamage;

    [Range(-100, 0)][SerializeField] private int _obstacleDamage;
    public int ObstacleDamage => _obstacleDamage;

    [Header("Positive reinforcement")]
    [Range(0, 1)][SerializeField] private float _resultOnSurviveFrame;
    public float ResultOnSurviveFrame => _resultOnSurviveFrame;

    [Range(0, 1)][SerializeField] private float _resultOnCollectibleHit;
    public float ResultOnCollectibleHit => _resultOnCollectibleHit;

    [Range(0, 1)][SerializeField] private float _resultOnDamagedEnemy;
    public float ResultOnDamagedEnemy => _resultOnDamagedEnemy;

    [Range(0, 1)][SerializeField] private float _resultOnWeaponPickup;
    public float ResultOnWeaponPickup => _resultOnWeaponPickup;

    [Space(20)]
    [Header("Negative reinforcement")]
    [Range(-1, 0)][SerializeField] private float _resultOnWallHit;
    public float ResultOnWallHit => _resultOnWallHit;

    [Range(-1, 0)][SerializeField] private float _resultOnDamageReceive;
    public float ResultOnDamageReceive => _resultOnDamageReceive;

    [Range(-1, 0)][SerializeField] private float _resultOnObstacleHit;
    public float ResultOnObstacleHit => _resultOnObstacleHit;
}
