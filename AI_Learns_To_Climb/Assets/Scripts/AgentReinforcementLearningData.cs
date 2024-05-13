using UnityEngine;


[CreateAssetMenu(menuName = "Reinforcement Learning consequences")]
public class AgentReinforcementLearningData : ScriptableObject
{
    [Header("Agent properties")]
    [Range(1, 100)][SerializeField] private int _maxHealth;
    public int MaxHealth => _maxHealth;

    [Range(1, 20)][SerializeField] private float _moveSpeed = 10;
    public float MoveSpeed => _moveSpeed;

    [Range(1, 100)][SerializeField] private int _weaponDamage = 60;
    public int WeaponDamage => _weaponDamage;


    [Header("Positive reinforcement")]
    [Range(0, 1)][SerializeField] private float _resultOnSurviveFrame;
    public float ResultOnSurviveFrame => _resultOnSurviveFrame;

    [Range(0, 1)][SerializeField] private float _resultOnCollectibleHit;
    public float ResultOnCollectibleHit => _resultOnCollectibleHit;

    [Range(1, 100)][SerializeField] private int _healthPotionValue;
    public int HealthPotionValue => _healthPotionValue;

    [Range(0, 1)][SerializeField] private float _resultOnDamagedEnemy;
    public float ResultOnDamagedEnemy => _resultOnDamagedEnemy;

    [Range(0, 1)][SerializeField] private float _resultOnWeaponPickup;
    public float ResultOnWeaponPickup => _resultOnWeaponPickup;

    [Space(20)]
    [Header("Negative reinforcement")]
    [Header("Walls")]
    [Range(1, 100)][SerializeField] private int _wallDamage;
    public int WallDamage => _wallDamage;
    [Range(-1, 0)][SerializeField] private float _resultOnWallHit;
    public float ResultOnWallHit => _resultOnWallHit;

    [Range(-1, 0)][SerializeField] private float _resultOnWeaponHit;
    public float ResultOnWeaponHit => _resultOnWeaponHit;

    [Header("Obstacles")]
    [Range(1, 100)][SerializeField] private int _obstacleDamage;
    public int ObstacleDamage => _obstacleDamage;
    [Range(-1, 0)][SerializeField] private float _resultOnObstacleHit;
    public float ResultOnObstacleHit => _resultOnObstacleHit;

    [Space(10)]
    [Range(-1, 0)][SerializeField] private float _resultOnGroundblockStay;
    public float ResultOnGroundblockStay => _resultOnGroundblockStay;

    [Range(-0.1f, -0.0001f)][SerializeField] private float _resultOnNotFindingCollectibles;
    public float ResultOnNotFindingCollectibles => _resultOnNotFindingCollectibles;
}
