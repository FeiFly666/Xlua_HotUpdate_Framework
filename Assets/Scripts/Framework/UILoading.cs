using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoading : MonoBehaviour
{
    [SerializeField] Image progressValue;
    [SerializeField] Text progressText;
    [SerializeField] GameObject progressBar;
    [SerializeField] Text progressDiscription;

    float _Max;
    public void InitProgress(float max, string discription)
    {
        _Max = max;

        progressBar.SetActive(true);
        progressDiscription.gameObject.SetActive(true);

        progressDiscription.text = discription;
        progressValue.fillAmount = max > 0 ? 0 : 100;

        progressText.gameObject.SetActive(max > 0);
    }

    public void UpdateProgress(float progress)
    {
        progressValue.fillAmount = progress / _Max;
        progressText.text = string.Format("{0:0}%", progressValue.fillAmount * 100);
    }
}
