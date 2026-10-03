using UnityEngine;

public enum EnemyType { Minus, Plus, Variable }

public class MathEnemy : MonoBehaviour
{
    [Header("Enemy Classification")]
    public EnemyType enemyType = EnemyType.Minus;

    [Header("Mass Impact Tweakables")]
    public float minusMassDamage = 25f;
    public float plusMassGain = 35f;
    public float variableMultiplier = 0.5f;

    [Header("Patrol Movement")]
    public float moveSpeed = 2f;
    public float patrolDistance = 3f;

    [Header("Cooldown Settings")]
    public float triggerCooldown = 0.5f;
    private float nextTriggerTime = 0f;

    private Vector3 startPos;
    private int direction = 1;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startPos.x) >= patrolDistance)
        {
            direction *= -1;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        HandlePlayerTouch(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        HandlePlayerTouch(collision.gameObject);
    }

    private void HandlePlayerTouch(GameObject touchedObject)
    {
        if (Time.time < nextTriggerTime) return;

        ChalkPlayer player = touchedObject.GetComponent<ChalkPlayer>();
        if (player != null)
        {
            nextTriggerTime = Time.time + triggerCooldown;

            switch (enemyType)
            {
                case EnemyType.Minus:
                    player.ConsumeMass(minusMassDamage);
                    break;

                case EnemyType.Plus:
                    player.GainMass(plusMassGain);
                    break;

                case EnemyType.Variable:
                    float previousMass = player.currentMass;
                    float newMass = player.currentMass * variableMultiplier;

                    if (newMass < previousMass)
                    {
                        player.ConsumeMass(previousMass - newMass);
                    }
                    else
                    {
                        player.GainMass(newMass - previousMass);
                    }
                    break;
            }
        }
    }
}