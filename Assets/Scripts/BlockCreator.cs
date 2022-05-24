using System.Collections.Generic;
using UnityEngine;

//_difficulty increase makes the elevations more sharp thus the game more and more difficult. 
//I added an another GameObject list to contain lower blocks so I could reuse them after
//the level generation.
public class BlockCreator : MonoBehaviour
{
    private static BlockCreator singleton = null;

    private GameObject[] _blockPrefabs;
    private GameObject _pointPrefab;
    private GameObject _pointObject;

    private List<GameObject> _blockPool = new List<GameObject>();
    private List<GameObject> _lowerBlockPool = new List<GameObject>();

    private float _lastHeightUpperBlock = 10;

    private readonly int _selectedBoxDistance = 4; // The distance between the player and the box it latches onto

    private int _difficulty = 0;
    private int _difficultyIncreaseFrequency = 300;
    private int _blockCount;
    private int _blockPoolCounter;
    private int _pointFrequency = 120;

    private bool _shouldIncreaseElevation;

    private float _elevation;

    private readonly float _elevationBaseStep = 0.5f;
    private readonly float _difficultyMultiplier = 0.1f;

    private readonly float _maxElevation = 10f;
    private readonly float _minElevation = 0f;

    public static BlockCreator GetSingleton()
    {
        if (singleton == null)
        {
            singleton = new GameObject("_BlockCreator").AddComponent<BlockCreator>();
        }
        return singleton;
    }

    public void Initialize(int bCount, GameObject[] bPrefabs, GameObject pPrefab)
    {
        _blockCount = bCount;
        _blockPrefabs = bPrefabs;
        _pointPrefab = pPrefab;
        InstantiateBlocks();
    }
    #region Block - Point Generation and Positions
    public void InstantiateBlocks()
    {
        for (int i = 0; i < _blockCount; i++)
        {
            int randomNumber = Random.Range(-1, 2);
            _blockPool.Add(Instantiate(_blockPrefabs[i % 3], new Vector3(0, 10 + randomNumber+_elevation, i ), transform.rotation));
            _lowerBlockPool.Add(Instantiate(_blockPrefabs[i % 3], new Vector3(0, -10 + randomNumber+_elevation, i ), transform.rotation));
            ChangeElevation();
        }
        _pointObject = Instantiate(_pointPrefab);
    }
    public Vector3 GetRelativeBlock(float playerPosZ) // This was a Transform that turned into a Vector3 to get the precise Vector3 point for joint.
    {
        if (playerPosZ < 0) playerPosZ = 0;
        int i = (int)playerPosZ + _selectedBoxDistance;

        i %= _blockPool.Count;
        return new Vector3(
            _blockPool[i].transform.position.x,
            _blockPool[i].transform.position.y - _lastHeightUpperBlock * 0.5f, // Get the bottom point of the block
            _blockPool[i].transform.position.z);
    }
    //This method acts as a Object Pool; rather than instantiating and destroying gameObjects over and over again
    //we use a pool to greatly reduce garbage.
    public void UpdateBlockPosition(int blockIndex)
    {
        if (_blockPoolCounter >= _blockPool.Count)
        {
            _blockPoolCounter = 0;
        }
        float randomNumber = Random.Range(-1f, 2f); // saved in a local variable so top and bot blocks get the same random number

        _blockPool[_blockPoolCounter].transform.position = new Vector3(0, 10 + randomNumber + _elevation, blockIndex + _blockCount);
        _lowerBlockPool[_blockPoolCounter].transform.position = new Vector3(0, -10 + randomNumber + _elevation, blockIndex + _blockCount);

        AddNextPoint(blockIndex);
        ChangeDifficulty(blockIndex);

        ChangeElevation();

        _blockPoolCounter++;
    }
    //initial joint location is called from this method.
    public Vector3 GetBlockPositionByIndex(int index)
    {
        if (index >= _blockPool.Count || index < 0)
        {
            Debug.LogError("The block doesn't exist in the pool.");
            return Vector3.zero;
        }

        return new Vector3(
            _blockPool[index].transform.position.x,
            _blockPool[index].transform.position.y - _lastHeightUpperBlock * 0.5f, // Get the bottom point of the block
            _blockPool[index].transform.position.z);
    }
    private void AddNextPoint(int blockIndex)
    {
        if (blockIndex % _pointFrequency == 0)
        {
            _pointObject.transform.position = (_blockPool[_blockPoolCounter].transform.position + _lowerBlockPool[_blockPoolCounter].transform.position) * 0.5f;
            _pointObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    #endregion

    #region Elevation - Difficulty
    private void ChangeElevation()
    {
        //Check if should increase or decrease elevation. _shouldIncreaseElevation doesn't change when the
        //_elevation value is between min and max value and both if's are skipped.
        if (_elevation > _maxElevation)
        {
            _shouldIncreaseElevation = false;
        }
        if(_elevation < _minElevation)
        {
            _shouldIncreaseElevation = true;
        }

        // Apply elevation
        if (_shouldIncreaseElevation)
        {
            _elevation += _elevationBaseStep + _difficulty*_difficultyMultiplier;
        }
        else
        {
            _elevation-= _elevationBaseStep + _difficulty * _difficultyMultiplier;
        }
    }

    private void ChangeDifficulty(int blockIndex)
    {
        if (blockIndex % _difficultyIncreaseFrequency == 0)
        {
            _difficulty++;
        }
    }
    #endregion
}
