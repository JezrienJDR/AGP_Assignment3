using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{

    public Transform up;
    public Transform down;
    public Transform left;
    public Transform right;
    // Start is called before the first frame update

    private Vector3 upVec;
    private Vector3 downVec;
    private Vector3 leftVec;
    private Vector3 rightVec;

    private Vector3 upIn;
    private Vector3 downIn;
    private Vector3 leftIn;
    private Vector3 rightIn;

    private Vector3 upOut;
    private Vector3 downOut;
    private Vector3 leftOut;
    private Vector3 rightOut;


    private Vector3 forwardVec;

    public bool hasSelection;

    public OreGrid grid;

    bool outPointsDefined;

    private void Start()
    {
        outPointsDefined = false;

        grid = FindObjectOfType<OreGrid>();

        upVec = new Vector3(0, 2f, 0);
        downVec = new Vector3(0, -2f, 0);
        leftVec = new Vector3(-2f, 0, 0);
        rightVec = new Vector3(2f, 0, 0);

        forwardVec = new Vector3(0, 0, -1.0f);

        upIn = up.position;
        downIn = down.position;
        leftIn = left.position;
        rightIn = right.position;

        DropTarget();

        

    }


    public void AcquireTarget(Vector3 newPosition)
    {
        transform.position = newPosition + forwardVec;
        
        StartCoroutine("ZoomIn");
        hasSelection = true;
    }

    public void DropTarget()
    {
        StartCoroutine("ZoomOut");
        hasSelection = false;
    }

    IEnumerator ZoomIn()
    {
        for(int i = 0; i < 10; i++)
        {
            up.position -= upVec;
            down.position -= downVec;
            left.position -= leftVec;
            right.position -= rightVec;

            yield return new WaitForSeconds(0.01f);
        }

        //up.localPosition    = upIn   ;
        //down.localPosition  = downIn ;
        //left.localPosition  = leftIn ;
        //right.localPosition = rightIn;
    }

    IEnumerator ZoomOut()
    {
        for (int i = 0; i < 10; i++)
        {
            up.position += upVec;
            down.position += downVec;
            left.position += leftVec;
            right.position += rightVec;

            yield return new WaitForSeconds(0.01f);
        }

        if (outPointsDefined == false)
        {
            upOut = up.localPosition;
            downOut = down.localPosition;
            leftOut = left.localPosition;
            rightOut = right.localPosition;

            outPointsDefined = true;
        }
        else
        {
            up.localPosition = upOut;
            down.localPosition = downOut;
            left.localPosition = leftOut;
            right.localPosition = rightOut;
        }
    }


}
