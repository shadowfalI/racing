using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RestartGame : MonoBehaviour {

    private GameObject againButton;
    private GameObject overButton;
    private GameObject againText;
    private GameObject overText;

	void Start () {
        againButton = GameObject.Find("AgainButton");
        overButton = GameObject.Find("OverButton");
        againText = GameObject.Find("AgainButton/AgainButtonText");
        overText = GameObject.Find("OverButton/OverButtonText");
        againButton.GetComponent<Button>().onClick.AddListener(OnAgainButtonClick);
        overButton.GetComponent<Button>().onClick.AddListener(OnOverButtonClick);
        Hide();
	}

    private void OnAgainButtonClick()
    {
        GameObject.FindGameObjectWithTag("Car").GetComponent<Driving>().enabled = false;  //控制系统关闭.
        GameObject.Find("Car").GetComponent<Transform>().position = new Vector3(893.4f, 101.8f, 852.6f);  //车辆归位.
        GameObject.Find("FinishCanvas/Finish").GetComponent<CarIdleEngine>().enabled = true; //车辆闲置引擎开启.
        GameObject.Find("Canvas/Timer").GetComponent<Text>().enabled = false;  //计时器关闭.
        //GameObject.Find("Canvas/TimeCount").GetComponent<Text>().enabled = true; //倒计时开始.
        GameObject.Find("Canvas/TimeCount").GetComponent<TimeCount>().StartTime(); //重置倒计时.
        GameObject.Find("FinishCanvas/Finish").GetComponent<GameController>().isEnd = false;
        Hide();
    }

    private void OnOverButtonClick()
    {
        Application.Quit();
    }

    public void Show()
    {
        gameObject.GetComponent<Text>().enabled = true;
        againButton.GetComponent<Image>().enabled = true;
        againButton.GetComponent<Button>().enabled = true;
        againText.GetComponent<Text>().enabled = true;
        overButton.GetComponent<Image>().enabled = true;
        overButton.GetComponent<Button>().enabled = true;
        overText.GetComponent<Text>().enabled = true;
    }

    public void Hide()
    {
        gameObject.GetComponent<Text>().enabled = false;
        againButton.GetComponent<Image>().enabled = false;
        againButton.GetComponent<Button>().enabled = false;
        againText.GetComponent<Text>().enabled = false;
        overButton.GetComponent<Image>().enabled = false;
        overButton.GetComponent<Button>().enabled = false;
        overText.GetComponent<Text>().enabled = false;
    }
}
