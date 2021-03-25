using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
public class OreGrid : MonoBehaviour
{
    private Vector3[,] grid;
    private GameObject[,] tiles;

    public float cellWidth;
    public int gridSize;

    public Tile swap1;
    public Tile swap2;

    private Reticle ret;

    private int numVacancies;
    private int numToBeam;

    public bool locked;

    private bool falling;

    public AudioSource sound;

    public Text ore;
    public Text time;
    public Canvas gameCanvas;
    public Canvas menuCanvas;

    public GameObject scorePanel;
    public Text scoreText;

    Vector3 startPosition;

    int oreScore;
    int timeLeft;

    int minimumContiguous;

    bool inGame;



    // Start is called before the first frame update
    void Start()
    {
        gameCanvas.enabled = false;
        scorePanel.SetActive(false);

        inGame = false;

        ret = FindObjectOfType<Reticle>();
        //ret.gameObject.SetActive(false);

    }

    public void Exit()
    {
        if(inGame)
        {
            EndGame();
        }
        else
        {
            Application.Quit();
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    public void leveOne()
    {
        minimumContiguous = 3;
        Initialize();
    }
    public void leveTwo()
    {
        minimumContiguous = 4;
        Initialize();
    }
    public void leveThree()
    {
        minimumContiguous = 5;
        Initialize();
    }


    public void EndGame()
    {
        StopAllCoroutines();

        foreach(GameObject t in tiles)
        {
            Destroy(t);
        }

        tiles = null;

        gameCanvas.enabled = false;

        menuCanvas.enabled = true;
        /////////////////////////////////////////
        scorePanel.SetActive(true);
        scoreText.text = oreScore.ToString();

        transform.position = startPosition;
        oreScore = 0;

        //ret.gameObject.SetActive(false);
        inGame = false;
    }

    public void Initialize()
    {
        menuCanvas.enabled = false;
        gameCanvas.enabled = true;
        scorePanel.SetActive(false);

        locked = false;

        startPosition = transform.position;

        grid = new Vector3[gridSize, gridSize];
        tiles = new GameObject[gridSize, gridSize];

        Debug.Log(grid.Length);


        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                grid[i, j] = new Vector3(i * cellWidth, j * cellWidth, 0.0f);


                tiles[i, j] = new GameObject();
                tiles[i, j].AddComponent<Tile>();
                Tile t = tiles[i, j].GetComponent<Tile>();
                t.grid = this;
                t.x = i;
                t.y = j;
                tiles[i, j].AddComponent<BoxCollider>();
                tiles[i, j].GetComponent<BoxCollider>().size = new Vector3(2, 2, 2);
                tiles[i, j].transform.parent = transform;
                tiles[i, j].transform.localPosition = grid[i, j];


            }
        }

        transform.Translate(new Vector3(cellWidth / 2.0f + (-0.5f * (float)gridSize * cellWidth), cellWidth / 2.0f + (-0.5f * (float)gridSize * cellWidth), 0.0f));

        oreScore = 0;
        timeLeft = 60;

        inGame = true;
        //ret.gameObject.SetActive(true);
        ore.text = oreScore.ToString();


        StartCoroutine("Countdown");
    }

    IEnumerator Countdown()
    {
        while(timeLeft > 0)
        {
            timeLeft--;
            time.text = timeLeft.ToString();
            yield return new WaitForSeconds(1);
        }

        EndGame();
    }

    public void TileSelect(int x, int y)
    {
        TileDeselect();

        if(x < gridSize - 1) tiles[x + 1, y].GetComponent<Tile>().swappable = true;
        if (x > 0) tiles[x - 1, y].GetComponent<Tile>().swappable = true;
        if (y > 0) tiles[x, y - 1].GetComponent<Tile>().swappable = true;
        if (y < gridSize - 1) tiles[x, y + 1].GetComponent<Tile>().swappable = true;
    }

    public void TileDeselect()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                tiles[i, j].GetComponent<Tile>().isSelected = false;
                tiles[i, j].GetComponent<Tile>().swappable = false;
            }
        }
    }


    public void Swap()
    {
        swap1.stone.MoveToTile(swap2);
        swap2.stone.MoveToTile(swap1);

        TileDeselect();

        StartCoroutine("WaitThenScan");

        locked = true;
    }


    IEnumerator WaitThenScan()
    {
        yield return new WaitForSeconds(0.5f);

        numToBeam = ScanAll();

        if (numToBeam > 0)
        {

            yield return new WaitForSeconds(1.5f);

            FallIntoGaps();

            //falling = true;

            while(falling)
            {
                if (falling)
                {
                    //Debug.Log("Still falling");
                    yield return new WaitForSeconds(0.1f);
                }
            }

            yield return new WaitForSeconds(0.4f);

            StartCoroutine("WaitThenScan");
        }
        else
        {
            //Debug.Log("No matches found.");
            locked = false;
        }
    }

    public List<Stone> ScanCheck(int x, int y)
    {

        bool endCheck = false;
        bool loopCheck = false;
        bool addedStone = false;

        List<Stone> stones = new List<Stone>();

        Stone.stoneType checkType = tiles[x, y].GetComponent<Tile>().stone.type;

        int checkY = y;
        int checkX = x;

        bool startCheck = false;


        if (x < gridSize - 1)
        { 
            if(tiles[x + 1, y].GetComponent<Tile>().stone.type == checkType)
            {
                //stones.Add(tiles[x, y].GetComponent<Tile>().stone);
                if(!stones.Contains(tiles[x + 1, y].GetComponent<Tile>().stone))
                stones.Add(tiles[x + 1, y].GetComponent<Tile>().stone);

                startCheck = true;
            }
        }
        if (x > 0)
        {             
            if (tiles[x - 1, y].GetComponent<Tile>().stone.type == checkType)
            {
                if (!stones.Contains(tiles[x - 1, y].GetComponent<Tile>().stone))
                    //stones.Add(tiles[x, y].GetComponent<Tile>().stone);
                    stones.Add(tiles[x - 1, y].GetComponent<Tile>().stone);

                startCheck = true;
            }
        }
        if (y > 0)
        {             
            if (tiles[x, y - 1].GetComponent<Tile>().stone.type == checkType)
            {
                if (!stones.Contains(tiles[x, y - 1].GetComponent<Tile>().stone))
                    //stones.Add(tiles[x, y].GetComponent<Tile>().stone);
                    stones.Add(tiles[x, y - 1].GetComponent<Tile>().stone);

                startCheck = true;
            }
        }
        if (y < gridSize - 1)
        { 
            if (tiles[x, y + 1].GetComponent<Tile>().stone.type == checkType)
            {
                if (!stones.Contains(tiles[x, y + 1].GetComponent<Tile>().stone))
                    //stones.Add(tiles[x, y].GetComponent<Tile>().stone);
                    stones.Add(tiles[x, y + 1].GetComponent<Tile>().stone);

                startCheck = true;
            }
        }


        if (startCheck == false)
        { 

            return null;

        }

        if (!stones.Contains(tiles[x, y].GetComponent<Tile>().stone))
        {
            stones.Add(tiles[x, y].GetComponent<Tile>().stone);
        }

        while (endCheck == false)
        {
            //Debug.Log("Iterating...");



            addedStone = false;

            //Stone[] stonesCopy = new Stone[stones.Count];
            List<Stone> stonesCopy = new List<Stone>(stones);
            //stones.CopyTo(stonesCopy);

            foreach(Stone s in stonesCopy)
            {
                checkX = s.transform.parent.GetComponent<Tile>().x;
                checkY = s.transform.parent.GetComponent<Tile>().y;

                if (checkX < gridSize - 1)
                {
                    if (tiles[checkX + 1, checkY].GetComponent<Tile>().stone.type == checkType)
                    {
                        if (!stonesCopy.Contains(tiles[checkX + 1, checkY].GetComponent<Tile>().stone))
                        {
                            stones.Add(tiles[checkX + 1, checkY].GetComponent<Tile>().stone);
                            addedStone = true;
                        }

                    }
                }
                if (checkX > 0)
                {
                    if (tiles[checkX - 1, checkY].GetComponent<Tile>().stone.type == checkType)
                    {
                        if (!stonesCopy.Contains(tiles[checkX - 1, checkY].GetComponent<Tile>().stone))
                        {
                            stones.Add(tiles[checkX - 1, checkY].GetComponent<Tile>().stone);
                            addedStone = true;
                        }

                    }
                }
                if (checkY > 0)
                {
                    if (tiles[checkX, checkY - 1].GetComponent<Tile>().stone.type == checkType)
                    {
                        
                        if (!stonesCopy.Contains(tiles[checkX, checkY - 1].GetComponent<Tile>().stone))
                        {
                            stones.Add(tiles[checkX, checkY - 1].GetComponent<Tile>().stone);
                            addedStone = true;
                        }

                    }
                }
                if (checkY < gridSize - 1)
                {
                    if (tiles[checkX, checkY + 1].GetComponent<Tile>().stone.type == checkType)
                    {
                        if (!stonesCopy.Contains(tiles[checkX, checkY + 1].GetComponent<Tile>().stone))
                        {
                            stones.Add(tiles[checkX, checkY + 1].GetComponent<Tile>().stone);
                            addedStone = true;
                        }

                    }
                }
            }

            if(addedStone == false)
            {
                endCheck = true;
            }
        }

        if(stones.Count >= minimumContiguous)
        {
            return stones;

            //foreach(Stone s in stones)
            //{
            //    //s.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            //    s.BeamOut();
            //    numVacancies++;
            //}
        }
        else
        {
            return null;
        }

    }

    IEnumerator ApplyGravity()
    {
        //Debug.Log("Falling...");

        falling = true;

        for (int p = 0; p < 9; p++)
        {
            //falling = true;           
            bool moved = false;
            //Debug.Log(p);
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (j > 0)
                    {
                        if (!tiles[i, j].GetComponent<Tile>().empty)
                        {
                            if (tiles[i, j - 1].GetComponent<Tile>().empty)
                            {
                                tiles[i, j].GetComponent<Tile>().stone.MoveToTile(tiles[i, j - 1].GetComponent<Tile>());
                                moved = true;
                            }
                        }
                    }
                }
            }

            if(moved == false || p >= 8)
            {
                p = 15;
                falling = false;
                //Debug.Log("Done falling");
            }

            yield return new WaitForSeconds(0.2f);
        }
        RefillVacancies();
    }

    private void FallIntoGaps()
    {
        StartCoroutine("ApplyGravity");
    }

    private void RefillVacancies()
    {
        //Debug.Log("Dropping in new Stones");

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if(tiles[i,j].GetComponent<Tile>().empty)
                {
                    tiles[i, j].GetComponent<Tile>().CreateStone();
                }
            }
        }

        //StartCoroutine("ScanAll");
    }

    private int ScanAll()
    {
        //Debug.Log("Scanning...");

        List<List<Stone>> listList = new List<List<Stone>>();

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                listList.Add(ScanCheck(i, j));
                //yield return new WaitForSeconds(0.01f);
            }
        }

        List<Stone> beamTargets = new List<Stone>();

        foreach(List<Stone> l in listList)
        {
            if (l != null)
            {
                foreach (Stone s in l)
                {
                    if (!beamTargets.Contains(s))
                    {
                        beamTargets.Add(s);
                    }
                }
            }
        }

        int returnNum = beamTargets.Count;

        if(returnNum > 0)
        {
            sound.Play();
        }

        foreach (Stone s in beamTargets)
        {
            //s.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            s.BeamOut();
            numVacancies++;
            oreScore++;
            if (s.type == Stone.stoneType.DIAMOND)
            {
                timeLeft++;
            }
        }

        ore.text = oreScore.ToString();

        return returnNum;
    }

    private void AdjacencyCheck(int x, int y)
    {

    }
}
