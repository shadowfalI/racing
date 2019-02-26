using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    private Timer m_Timer;
    private GameObject againDes;
    public bool isEnd = false;

    void Start()
    {
        againDes = GameObject.Find("Canvas/AgainDes");
        m_Timer = GameObject.Find("Canvas/Timer").GetComponent<Timer>();
    }

	void OnTriggerEnter(Collider col)
    {
        if (isEnd)
        {
            if (col.transform.root.tag == "Car")
            {
                m_Timer.Stop();
                float nowTotleTime = m_Timer.totalTime;
                float oldTotalTime = 9999;
                if (PlayerPrefs.HasKey("bestTime"))
                {
                    oldTotalTime = PlayerPrefs.GetFloat("bestTime");
                }
                m_Timer.GetComponent<Text>().text = "当前纪录：" + Mathf.Round(nowTotleTime) + "s\n" + "最高纪录" + Mathf.Round(oldTotalTime) + "s";
                if (nowTotleTime < oldTotalTime)
                {
                    oldTotalTime = nowTotleTime;
                }
                PlayerPrefs.SetFloat("bestTime", oldTotalTime);
                againDes.GetComponent<RestartGame>().Show();
            }
        }
    }
}
