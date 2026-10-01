using UnityEngine;

public enum EnemyType { Minus, Plus, Variable }

public class MathEnemy : MonoBehaviour
{
    [Header("Enemy Classification")]
    public EnemyType enemyType = EnemyType.Minus;

    [Header("Mass Impact Tweakables")]
    public float minusMassDamage = 25f; // Eats player mass (makes player small/fast)
    public float plusMassGain = 35f;    // Overloads player with mass (makes player huge/heavy)

    [Header("Patrol Movement")]
    public float moveSpeed = 2f;
    public float patrolDistance = 3f;

    private Vector3 startPos;
    private int direction = 1;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Patrol back and forth along the floor
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startPos.x) >= patrolDistance)
        {
            direction *= -1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ChalkPlayer player = collision.GetComponent<ChalkPlayer>();
        if (player != null)
        {
            switch (enemyType)
            {
                case EnemyType.Minus:
                    // Shrinks player mass
                    player.ConsumeMass(minusMassDamage);
                    break;

                case EnemyType.Plus:
                    // Expands player mass (makes player huge and heavy)
                    player.GainMass(plusMassGain);
                    break;

                case EnemyType.Variable:
                    // Variable hazard randomly inflates or deflates player mass on touch
                    float randomMassChange = Random.Range(-30f, 30f);
                    if (randomMassChange < 0)
                        player.ConsumeMass(Mathf.Abs(randomMassChange));
                    else
                        player.GainMass(randomMassChange);
                    break;
            }
        }
    }
}