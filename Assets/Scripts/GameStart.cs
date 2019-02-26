using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameStart : MonoBehaviour {

    private InputField usernanmeInpute;
    private Button button;
	
	void Start () {
        usernanmeInpute = GameObject.Find("InputField").GetComponent<InputField>();
        if (PlayerPrefs.HasKey("name"))
        {
            usernanmeInpute.text = PlayerPrefs.GetString("name");
        }

        button = GameObject.Find("Button").GetComponent<Button>();
        button.onClick.AddListener(OnButtonSureClick);
	}

    private void OnButtonSureClick()
    {
        PlayerPrefs.SetString("name", usernanmeInpute.text);
        Application.LoadLevel(1);
    }
}
