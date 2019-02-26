using UnityEngine;
using System.Collections;

public class CanEnd : MonoBehaviour {

   

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.root.tag == "Car")
        {
            GameObject.Find("FinishCanvas/Finish").GetComponent<GameController>().isEnd = true;
        }
    }
}
