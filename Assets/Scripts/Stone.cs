using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stone : MonoBehaviour
{

    public stoneType type;

    private MeshRenderer meshRend;
    private MeshFilter mesh;

    private Tile target;

    private bool isSpinning;

    private Material mat;

    // Start is called before the first frame update
    void Start()
    {
        //int startType = Random.Range(0, 4);
        //SetType((stoneType)startType);

        //gameObject.AddComponent<MeshRenderer>();
        //gameObject.AddComponent<MeshFilter>();

        //meshRend = GetComponent<MeshRenderer>();
        //mesh = GetComponent<MeshFilter>();

    }

    private void Awake()
    {
        int startType = Random.Range(0, 5);

        int diamondChance = Random.Range(0, 10);

        if(diamondChance == 0)
        {
            startType = 5;
        }

        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshFilter>();
        //gameObject.AddComponent<BoxCollider>();

        meshRend = GetComponent<MeshRenderer>();
        mesh = GetComponent<MeshFilter>();
        
        SetType((stoneType)startType);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetType(stoneType t)
    {
        type = t;

        switch(t)
        {
            case stoneType.RUBY:
                meshRend.material = Resources.Load<Material>("Materials/Ruby_mat");
                mesh.mesh = Resources.Load<Mesh>("Models/Ruby");
                break;
            case stoneType.EMERALD:
                meshRend.material = Resources.Load<Material>("Materials/Emerald_mat");
                mesh.mesh = Resources.Load<Mesh>("Models/Emerald");
                break;
            case stoneType.AMETHYST:
                meshRend.material = Resources.Load<Material>("Materials/Amethyst_mat");
                mesh.mesh = Resources.Load<Mesh>("Models/Amethyst");
                break;
            case stoneType.HELIODOR:
                meshRend.material = Resources.Load<Material>("Materials/Heliodor_mat");
                mesh.mesh = Resources.Load<Mesh>("Models/Heliodor");
                break;
            case stoneType.SAPPHIRE:
                meshRend.material = Resources.Load<Material>("Materials/Sapphire_mat");
                mesh.mesh = Resources.Load<Mesh>("Models/_Sapphire");
                break;
            case stoneType.DIAMOND:
                meshRend.material = Resources.Load<Material>("Materials/Diamond_mat");
                mesh.mesh = Resources.Load<Mesh>("Models/Diamond");
                break;
        }

        mat = meshRend.material;
    }

    public void MoveToTile(Tile t)
    {
        target = t;

        StartCoroutine("LerpTo");
    }

    public void LongMoveToTile(Tile t)
    {
        target = t;

        StartCoroutine("LongLerpTo");
    }

    IEnumerator LerpTo()
    {
        transform.parent.GetComponent<Tile>().empty = true;

        for(int i = 0; i < 11; i++)
        {
            transform.position = Vector3.Lerp(transform.parent.position, target.transform.position, 0.1f * i);
            yield return new WaitForSeconds(0.01f);
        }

        transform.parent = target.transform;
        
        target.SetStone(gameObject);

        //Tile t = transform.parent.GetComponent<Tile>();

        //t.grid.ScanCheck(t.grid.swap1.x, t.grid.swap1.y);
        //t.grid.ScanCheck(t.grid.swap2.x, t.grid.swap2.y);

        //Debug.Log("FAST LERP");
    }

    IEnumerator LongLerpTo()
    {
        //Debug.Log("COMMENCING SLOW LERP");
        transform.parent.GetComponent<Tile>().empty = true;
        Vector3 startPos = transform.position;

        for (int i = 0; i < 11; i++)
        {
            transform.position = Vector3.Lerp(startPos, target.transform.position, 0.1f * i);
            yield return new WaitForSeconds(0.02f);

            
        }

        transform.parent = target.transform;

        target.SetStone(gameObject);

        //Debug.Log("SLOW LERP COMPLETE");
    }

    public void StartSpinning()
    {
        isSpinning = true;
        StartCoroutine("Spin");
    }

    public void StopSpinning()
    {
        isSpinning = false;
    }

    IEnumerator Spin()
    {
        for(int i = 0; i < 36; i++)
        {
            transform.Rotate(0.0f, 5.0f, 0.0f);
            yield return new WaitForSeconds(0.02f);
        }

        if(isSpinning)
        {
            StartCoroutine("Spin");
        }
    }

    public void BeamOut()
    {
        StartCoroutine("FadeOut");

        Instantiate(Resources.Load<GameObject>("TransporterParticles"), transform.position, Quaternion.Euler(90,0,0));
    }

    IEnumerator FadeOut()
    {
        Color baseCol = mat.color;

        for(int i = 0; i < 21; i++)
        {
            mat.color = Color.Lerp(baseCol, Color.clear, i/20.0f);
            yield return new WaitForSeconds(0.02f);
        }


        transform.position = new Vector3(-1000, 0, 0);

        yield return new WaitForSeconds(0.9f);

        transform.parent.GetComponent<Tile>().empty = true;




    }

    public enum stoneType
    {
        RUBY,
        EMERALD,
        AMETHYST,
        HELIODOR,
        SAPPHIRE,
        DIAMOND
    }
}
