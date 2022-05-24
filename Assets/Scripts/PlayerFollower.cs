using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    private Transform _player;

    private float _zDifference;
    private float _yDifference;
    private float _elapsedPosition = -2; // the counter doesn't start from 0 to prevent blocks from disappearing too quickly
    private float _previousFramePosition;

    private int _lastPassageIndex = -1;
    private void Start()
    {
        //Distance traveled is calculated using the difference between _previousFramePosition and current position.
        _previousFramePosition = transform.position.z;
    }
    public void SetPosition(Transform p)
    {
        _player = p;
        _zDifference = _player.position.z - transform.position.z;
        _yDifference = _player.position.y - transform.position.y;
        //Since the player variable is a transform, using player.transform.position is unnecessary
        //hence it is replaced with player.position. This method is called from the PlayerController on the start.
    }
    private void Update()
    {
        // Rather than relying on nested ifs, guard clause is used to improve readability of code.
        if (_player == null) return;

        UpdatePosition();
        BlockUpdate();
    }

    //UpdateBlockPosition is called only when the player moves forward a block.(A block's Z length is 1 unit.)
    private void BlockUpdate()
    {
        _elapsedPosition += transform.position.z - _previousFramePosition;
        _previousFramePosition = transform.position.z;
        if (_elapsedPosition >= 1)
        {
            _lastPassageIndex++;
            _elapsedPosition -= 1;
            BlockCreator.GetSingleton().UpdateBlockPosition(_lastPassageIndex);
        }
    }
    //Position update moved to a separate method so that it is more readable and easier to expand.
    private void UpdatePosition()
    {
        transform.position = new Vector3(transform.position.x, _player.position.y - _yDifference, _player.position.z - _zDifference);
    }
}
