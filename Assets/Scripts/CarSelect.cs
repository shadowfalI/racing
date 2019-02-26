using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarSelect : MonoBehaviour {

    private Image loading;
    private float targetValue = 0;
    private AsyncOperation operation;
    private GameObject BG;

	void Start () {
        loading = GameObject.Find("HelpCanvas/BG/Empty/Red").GetComponent<Image>();
        BG = GameObject.Find("HelpCanvas/BG");
        BG.SetActive(false);
        gameObject.GetComponent<Button>().onClick.AddListener(CarSelectSureClick);
	}

     IEnumerator Loading()
	{
        while (true)
        {
            targetValue = operation.progress;

            if (operation.progress >= 0.9f)
            {
                //operation.progress的值最大为0.9
                targetValue = 1.0f;
            }

            if (targetValue != loading.fillAmount)
            {
                //插值运算
                loading.fillAmount = Mathf.Lerp(loading.fillAmount, targetValue, Time.deltaTime);
                if (Mathf.Abs(loading.fillAmount - targetValue) < 0.01f)
                {
                    loading.fillAmount = targetValue;
                }
            }


            if ((int)(loading.fillAmount * 100) == 100)
            {
                //允许异步加载完毕后自动切换场景
                operation.allowSceneActivation = true;
            }
            yield return new WaitForSeconds(0.1f);
        }
	}

    private void CarSelectSureClick()
    {
        //Application.LoadLevel(2);
        BG.SetActive(true);
        StartCoroutine(AsyncLoading());
        StartCoroutine(Loading());
    }

    IEnumerator AsyncLoading()
	{
		operation = SceneManager.LoadSceneAsync("Game");
		//阻止当加载完成自动切换
		operation.allowSceneActivation = false;
 
		yield return operation;
	}
}
