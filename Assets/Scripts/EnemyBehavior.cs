using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public float speed = 1f;
    private List<string> directions;
    private Vector2 startPosition;
    private int currentDirectionIndex;
    private float moveProgress;
    public float health = 10.0f;
    private float changeDirectionInterval = 1f;

    void Start()
    {
        currentDirectionIndex = 0;
        moveProgress = 0f;
    }

    void Update()
    {
        if (directions != null && currentDirectionIndex < directions.Count)
        {
            moveProgress += Time.deltaTime;

            if (moveProgress >= changeDirectionInterval)
            {
                currentDirectionIndex++;
                moveProgress = 0f;
            }

            if (currentDirectionIndex < directions.Count)
            {
                string direction = directions[currentDirectionIndex];
                Vector2 movementVector = direction switch
                {
                    "L" => Vector2.left,
                    "R" => Vector2.right,
                    "U" => Vector2.up,
                    "D" => Vector2.down,
                    _ => Vector2.zero,
                };
                transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + movementVector, speed * Time.deltaTime);
            }
        }
    }

    public void SetPath(Vector2 newStartPosition, List<string> newDirections)
    {
        startPosition = newStartPosition;
        directions = newDirections;
        transform.position = startPosition;
    }

    public void SetPathFromString(string pathData)
    {
        string[] parts = pathData.Split(' ');
        startPosition = ParseVector2(parts[0]);
        directions = new List<string>(parts[1..]);
        transform.position = startPosition;
    }

    private Vector2 ParseVector2(string vecString)
    {
        string[] parts = vecString.Trim('(', ')').Split(',');
        if (parts.Length != 2)
        {
            Debug.LogError("Invalid Vector2 string format");
            return Vector2.zero;
        }

        if (!float.TryParse(parts[0], out float x) || !float.TryParse(parts[1], out float y))
        {
            Debug.LogError("Failed to parse Vector2 components");
            return Vector2.zero;
        }

        return new Vector2(x, y);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}