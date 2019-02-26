using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeCount : MonoBehaviour {

    private int startCount = 5;
    private Text timeCount;
    private Driving driving;

	void Start () {
        driving = GameObject.FindGameObjectWithTag("Car").GetComponent<Driving>();
        timeCount = GameObject.Find("Canvas/TimeCount").GetComponent<Text>();
        StartCoroutine("Count");
	}

    public void StartTime()
    {
        startCount = 5;
        timeCount.enabled = true;
        StartCoroutine("Count");
    }

    IEnumerator Count()
    {
        timeCount.text = startCount.ToString();
        while (startCount > 0)
        {
            yield return new WaitForSeconds(1.0f);
            startCount--;
            timeCount.text = startCount.ToString();
        }
        yield return new WaitForSeconds(0.1f);
        driving.enabled = true;
        timeCount.enabled = false;
        GameObject.Find("FinishCanvas/Finish").GetComponent<CarIdleEngine>().DisableSelf();
        GameObject.Find("Canvas/Timer").GetComponent<Timer>().Show();
        
    }
	
}
