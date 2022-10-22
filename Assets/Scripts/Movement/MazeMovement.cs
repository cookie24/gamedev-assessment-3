using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeMovement : MonoBehaviour
{
    public Dir lastInput = Dir.None;
    public Dir currentInput = Dir.None;
    private PointMovement pointMover;

    // Start is called before the first frame update
    void Start()
    {
        pointMover = GetComponent<PointMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get player input
        // This is the job of the controller scripts

        // Check if we are NOT moving
        if (!pointMover.IsMoving())
        {
            // Pick a new direction to move
            if (lastInput != Dir.None && !CollisionInDirection(lastInput, "Wall"))
            {
                // Move in the direction of lastInput
                pointMover.AddPoint(GetPosInDirection(lastInput));

                // Update current input, since we are now moving in that direction
                currentInput = lastInput;
            }
            else
            if (currentInput != Dir.None && !CollisionInDirection(currentInput, "Wall"))
            {
                // Move in the direction we're going right now
                pointMover.AddPoint(GetPosInDirection(currentInput));
            }
            else
            {
                // Hit a wall LOL (not moving)
            }
        }
    }

    public bool CollisionInDirection(Dir dir, string tag)
    {
        Vector2 checkPos = GetPosInDirection(dir);
        Collider2D collision = Physics2D.OverlapCircle(checkPos, 0.45f);
        if (collision != null && collision.tag == tag)
        {
            return true;
        }

        return false;
    }

    private Vector3 GetPosInDirection(Dir dir)
    {
        return transform.position + Direction.GetDirectionVector3(dir);
    }
}
