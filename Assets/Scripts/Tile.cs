using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject stoneObj;
    public Stone stone;

    private bool HasStone;

    private bool isHovered;
    public bool isSelected;
    public bool swappable;

    private Reticle ret;
    public OreGrid grid;

    public int x;
    public int y;

    public bool empty;

    // Start is called before the first frame update
    void Start()
    {
        stoneObj = new GameObject();
        stoneObj.AddComponent<Stone>();

        stoneObj.transform.parent = transform;

        stone = stoneObj.GetComponent<Stone>();

        stone.MoveToTile(this);

        ret = FindObjectOfType<Reticle>();

        isSelected = false;
        swappable = false;
        empty = false;
    }


    IEnumerator NewStone()
    {
        stoneObj = new GameObject();
        stoneObj.AddComponent<Stone>();

        stoneObj.transform.parent = transform;

        stone = stoneObj.GetComponent<Stone>();

        stone.transform.position = transform.position + new Vector3(0, 10, 0);

        yield return new WaitForSeconds(0.1f);

        stone.LongMoveToTile(this);

        empty = false;
    }

    public void CreateStone()
    {
        StartCoroutine("NewStone");
    }

    public void SetStone(GameObject s)
    {
        stoneObj = s;
        stone = stoneObj.GetComponent<Stone>();
        empty = false;
    }

    public void ClearStone()
    {
        Destroy(stone.gameObject);
        stone = null;
    }

    // Update is called once per frame


    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("CLICKED");
        if (!grid.locked && stone.type != Stone.stoneType.DIAMOND)
        {
            if (!isSelected)
            {
                if (swappable)
                {
                    grid.swap2 = this;
                    grid.Swap();

                    ret.DropTarget();

                    isHovered = false;
                    stone.StopSpinning();
                }
                else if ((ret.hasSelection == false))
                {
                    ret.AcquireTarget(transform.position);
                    isSelected = true;

                    grid.TileSelect(x, y);
                    grid.swap1 = this;
                }
            }
            else
            {
                ret.DropTarget();
                isSelected = false;

                grid.TileDeselect();
            }
        }
        else
        {
            Debug.Log("LOCKED");
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        stone.StartSpinning();       
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        stone.StopSpinning();        
    }

    void Update()
    {
        if(swappable)
        {
            //stoneObj.transform.Rotate(0, 0, 5);
        }
    }
}
