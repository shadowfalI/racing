using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    private Text timertext;

    public float totalTime = 0;
    private bool isStart = false;

	void Start () {
        timertext = gameObject.GetComponent<Text>();
        timertext.enabled = false;
        totalTime = 0;
	}
	
	
	void Update () {
        if (isStart)
        {
            totalTime += Time.deltaTime;
            timertext.text = Mathf.Round(totalTime).ToString() + " s";
        }
	}


    public void Show()
    {
        isStart = true;
        timertext.enabled = true;
    }

    public void Stop()
    {
        isStart = false;
    }
}
