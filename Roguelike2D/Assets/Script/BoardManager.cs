using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardManager : MonoBehaviour
{
    [System.Serializable]
    public class Count
    {
        public int min;
        public int max;

        public Count(int minimum, int maximum)
        {
            min = minimum;
            max = maximum;
        }
    }

    public int column = 8;
    public int row = 8;


    public GameObject food;
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);

    public GameObject exit;                                         //Prefab to spawn for exit.
    public GameObject[] floorTiles;                                 //Array of floor prefabs.
    public GameObject[] wallTiles;                                  //Array of wall prefabs.
    public GameObject[] foodTiles;                                  //Array of food prefabs.
    public GameObject[] enemyTiles;                                 //Array of enemy prefabs.
    public GameObject[] outerWallTiles;                             //Array of outer tile prefabs.

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3> ();

    void InitialiseList()                                   // 초기화 - 타일 놓을 영역을 gridPosition에 ADD
    {
        gridPositions.Clear();

        for (int x = 1; x < column - 1; x++)           // 플레이 공간의 타일을 List에 넣는다
        {
            for (int y = 1; y < row - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0.0f));
            }
        }

    }


    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        for (int x = -1; x < column + 1; x++)            // 외벽생성( OuterWall)
        {
            for (int y = -1; y < row + 1; y++)
            {
                GameObject toInstantiate = floorTiles[ Random.Range(0, floorTiles.Length) ];    // Floor를 만들계획


                if (x == -1 || x == column || y == -1 || y == row)                              // 외벽위치라면 외벽 계획
                {
                    toInstantiate = outerWallTiles[ Random.Range(0, outerWallTiles.Length) ]; 
                }
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject; // 계획을 생성

                instance.transform.SetParent(boardHolder);
            }
        }
    }




    void LayoutObjectAtRandom(GameObject[] makeTileArray, int min, int max)
    {

        int amount = Random.Range(min, max+1);                                                   // 몇개만들것인가 결정 
                                                                                                 // +1해준이유는 int일때 max값이 나오지않는다
        for (int i = 0; i < amount; i++)                                                         // 갯수만큼 생성
        {
            GameObject selectTile = makeTileArray[Random.Range(0, makeTileArray.Length)];        // 어떤타일을 만들것인가 결정
            Instantiate(selectTile, RandomPosition(), Quaternion.identity);
        }

    }

    Vector3 RandomPosition()
    {
        int randomResult = Random.Range(0, gridPositions.Count);

        Vector3 toReturn = gridPositions[randomResult];
        gridPositions.RemoveAt(randomResult);
        return toReturn;
    }

    public void SetupScene(int level)
    {
        BoardSetup ();
        InitialiseList ();
        
        // 벽생성
        LayoutObjectAtRandom(wallTiles, wallCount.min, wallCount.max);
        // food 생성
        LayoutObjectAtRandom(foodTiles, foodCount.min, foodCount.max);
        
        // log2 level 하여 몬스터수 지정
        int enemyAmount = (int) Mathf.Log(level, 2.0f);
        LayoutObjectAtRandom(enemyTiles , enemyAmount, enemyAmount);
        
        // exit 생성
        Instantiate(exit, new Vector3(column - 1, row - 1, 0.0f), Quaternion.identity);
    }

}


