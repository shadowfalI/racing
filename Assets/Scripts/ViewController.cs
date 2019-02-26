using UnityEngine;
using System.Collections;

public class ViewController : MonoBehaviour {

    private bool isFirstView = false;
    private GameObject mainCamera;
    private GameObject firstViewCamera;

	void Start () {
        mainCamera = GameObject.Find("MainCamera");
        firstViewCamera = GameObject.Find("Camera");
	}
	
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (isFirstView)
            {
                mainCamera.SetActive(true);
                firstViewCamera.SetActive(false);
                isFirstView = false;
            }
            else
            {
                mainCamera.SetActive(false);
                firstViewCamera.SetActive(true);
                isFirstView = true;
            }
        }
	}
}
