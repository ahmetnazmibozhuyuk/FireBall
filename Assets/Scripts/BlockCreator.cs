using System.Collections.Generic;
using UnityEngine;


//In this class, the map has been created.
//You have to edit GetRelativeBlock section to calculate current relative block to cast player rope to hold on
//Update Block Position section to make infinite map.
public class BlockCreator : MonoBehaviour
{

    public int BlockCount { get; private set; }

    private static BlockCreator singleton = null;

    private GameObject[] blockPrefabs;
    private GameObject pointPrefab;
    private GameObject pointObject;

    private List<GameObject> blockPool = new List<GameObject>();
    private List<GameObject> lowerBlockPool = new List<GameObject>();
    private float lastHeightUpperBlock = 10; //readonly ise öyle yap
    private int difficulty = 1;

    private int _blockPoolAmount;
    private int _blockPoolCounter;
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
        BlockCount = bCount;
        blockPrefabs = bPrefabs;
        pointPrefab = pPrefab;
        InstantiateBlocks();
    }
    public void InstantiateBlocks()
    {
        for (int i = 0; i < BlockCount; i++)
        {
            int randomNumber = Random.Range(-2, 3);
            blockPool.Add(Instantiate(blockPrefabs[i % 3], new Vector3(0, 15 + randomNumber, i + 1), transform.rotation));
            lowerBlockPool.Add(Instantiate(blockPrefabs[i % 3], new Vector3(0, -10 + randomNumber, i + 1), transform.rotation));

            _blockPoolAmount++;
        }
        pointObject = Instantiate(pointPrefab);
    }
    public Vector3 GetRelativeBlock(float playerPosZ) // This was a Transform
    {
        int i = (int)playerPosZ + 5;
        i %= blockPool.Count;
        return new Vector3(
            blockPool[i].transform.position.x,
            blockPool[i].transform.position.y - lastHeightUpperBlock*0.5f,
            blockPool[i].transform.position.z);
    }

    public void UpdateBlockPosition(int blockIndex)
    {
        if (_blockPoolCounter >= blockPool.Count)
        {
            _blockPoolCounter = 0;
        }   
        int randomNumber = Random.Range(-2, 3);

        blockPool[_blockPoolCounter].transform.position = new Vector3(0, 15 + randomNumber, blockIndex + _blockPoolAmount);
        lowerBlockPool[_blockPoolCounter].transform.position = new Vector3(0, -10 + randomNumber+difficulty, blockIndex+_blockPoolAmount);
        _blockPoolCounter++;


        AddNextPoint(blockIndex);
        IncreaseDifficulty(blockIndex);


    }
    private void AddNextPoint(int blockIndex)
    {
        if (blockIndex % 100 == 0)
        {
            pointObject.transform.position = (blockPool[_blockPoolCounter].transform.position + lowerBlockPool[_blockPoolCounter].transform.position) * 0.5f;
            pointObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    private void IncreaseDifficulty(int blockIndex)
    {
        if (blockIndex % 300 == 0)
        {
            difficulty++;
        }
    }
}
