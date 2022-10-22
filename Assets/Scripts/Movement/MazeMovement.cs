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
            if (lastInput != Dir.None && !CollisionInDirection(lastInput, "Wall", 0.45f))
            {
                // Move in the direction of lastInput
                pointMover.AddPoint(GetPosInDirection(lastInput));

                // Update current input, since we are now moving in that direction
                currentInput = lastInput;
            }
            else
            if (currentInput != Dir.None && !CollisionInDirection(currentInput, "Wall", 0.45f))
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

    public bool CollisionInDirection(Dir dir, string tag, float radius)
    {
        Vector2 checkPos = GetPosInDirection(dir);
        Collider2D[] collArray = Physics2D.OverlapCircleAll(checkPos, radius);
        foreach (Collider2D coll in collArray)
        {
            if (coll != null && coll.tag == tag)
            {
                return coll;
            }
        }
        return false;
    }

    private Vector3 GetPosInDirection(Dir dir)
    {
        return transform.position + (Direction.GetDirectionVector3(dir));
    }
}
