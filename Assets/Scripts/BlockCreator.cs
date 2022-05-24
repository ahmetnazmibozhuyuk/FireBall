using System.Collections.Generic;
using UnityEngine;


//In this class, the map has been created.
//You have to edit GetRelativeBlock section to calculate current relative block to cast player rope to hold on
//Update Block Position section to make infinite map.
public class BlockCreator : MonoBehaviour
{
    private static BlockCreator singleton = null;

    private GameObject[] _blockPrefabs;
    private GameObject _pointPrefab;
    private GameObject _pointObject;

    private List<GameObject> _blockPool = new List<GameObject>();
    private List<GameObject> _lowerBlockPool = new List<GameObject>();

    private float _lastHeightUpperBlock = 10;

    private int _selectedBoxDistance = 4;

    private int _difficulty = 0;
    private int _difficultyIncreaseFrequency = 300;

    private int _blockCount;

    private int _blockPoolAmount;
    private int _blockPoolCounter;

    private int _pointFrequency = 120;


    private bool _increaseElevation;
    private float _elevation;
    private float _maxElevation = 10;
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
    public void InstantiateBlocks()
    {
        for (int i = 0; i < _blockCount; i++)
        {
            int randomNumber = Random.Range(-1, 2);
            _blockPool.Add(Instantiate(_blockPrefabs[i % 3], new Vector3(0, 10 + randomNumber+_elevation, i ), transform.rotation));
            _lowerBlockPool.Add(Instantiate(_blockPrefabs[i % 3], new Vector3(0, -10 + randomNumber+_elevation, i ), transform.rotation));
            ChangeElevation();
            _blockPoolAmount++;
        }
        _pointObject = Instantiate(_pointPrefab);
    }
    public Vector3 GetRelativeBlock(float playerPosZ) // This was a Transform
    {
        int i = (int)playerPosZ + _selectedBoxDistance;
        i %= _blockPool.Count;
        return new Vector3(
            _blockPool[i].transform.position.x,
            _blockPool[i].transform.position.y - _lastHeightUpperBlock * 0.5f, // Get the bottom point of the block
            _blockPool[i].transform.position.z);
    }

    public void UpdateBlockPosition(int blockIndex)
    {
        if (_blockPoolCounter >= _blockPool.Count)
        {
            _blockPoolCounter = 0;
        }
        float randomNumber = Random.Range(-1f, 2f); // top and bot blocks get the same random number

        _blockPool[_blockPoolCounter].transform.position = new Vector3(0, 10 + randomNumber + _elevation, blockIndex + _blockPoolAmount);
        _lowerBlockPool[_blockPoolCounter].transform.position = new Vector3(0, -10 + randomNumber + _elevation, blockIndex + _blockPoolAmount);


        AddNextPoint(blockIndex);
        ChangeDifficulty(blockIndex);

        ChangeElevation();

        _blockPoolCounter++;
    }
    private void AddNextPoint(int blockIndex)
    {
        if (blockIndex % _pointFrequency == 0)
        {
            _pointObject.transform.position = (_blockPool[_blockPoolCounter].transform.position + _lowerBlockPool[_blockPoolCounter].transform.position) * 0.5f;
            _pointObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    private void ChangeElevation()
    {

        if (_elevation > _maxElevation)
        {
            _increaseElevation = false;
        }
        if(_elevation < 0)
        {
            _increaseElevation = true;
        }

        if (_increaseElevation)
        {
            _elevation += 0.5f+_difficulty*0.1f;
        }
        else
        {
            _elevation-= 0.5f+_difficulty * 0.1f;
        }
    }
    private void ChangeDifficulty(int blockIndex)
    {
        if (blockIndex % _difficultyIncreaseFrequency == 0)
        {
            _difficulty++;
            Debug.Log("difficulty increased to " + _difficulty);
        }
    }
}
