//This class is on the main camera to follow player.
//You may optimize it on SetPosition section and
//Write a proper way to update blocks positions on the map to make it an infite gameplay.

using UnityEngine;

public class PlayerFollower : MonoBehaviour
{

    private Transform player;

    private float zDifference;
    private float yDifference;


    private float elapsedPosition = -2; // the counter doesn't start from 0 to prevent blocks from disappearing too quickly
    private float _previousFramePosition;

    private void Start()
    {
        _previousFramePosition = transform.position.z;
    }
    public void SetPosition(Transform p)
    {
        //Optimize this portion
        player = p;
        zDifference = player.position.z - transform.position.z;
        yDifference = player.position.y - transform.position.y;

        //Since the player variable is a transform, using player.transform.position is unnecessary
        //Hence it is replaced with player.position. Caching transform.position 
        //makes little to no difference.
    }

    int lastPassageIndex = -1;
    private void Update()
    {
        if (player == null) return;

        UpdatePosition();
        BlockUpdate();


        //BlockCreator.GetSingleton().UpdateBlockPosition(passageIndex); //You may call update block position here to make it an infinite map.
        //Hint:
        //It must be called when it is really needed in a very optimized way.

    }
    private void BlockUpdate()
    {
        elapsedPosition += transform.position.z - _previousFramePosition;
        _previousFramePosition = transform.position.z;
        if (elapsedPosition >= 1)
        {
            lastPassageIndex++;
            elapsedPosition -= 1;
            BlockCreator.GetSingleton().UpdateBlockPosition(lastPassageIndex);
        }
    }
    private void UpdatePosition()
    {
        transform.position = new Vector3(transform.position.x, player.position.y - yDifference, player.position.z - zDifference);
    }
}
