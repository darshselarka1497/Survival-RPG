using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Fade : MonoBehaviour
{
    [SerializeField]
    private Image fadeImage;
    [SerializeField]
    private Text fadeText;
    private float imgFdIn;
    private float imgFdOut;
    private float txtFdIn;
    private float txtFdOut;

    public void setFadeTimer(float imageFadeIn, float imageFadeOut, float textFadeIn, float textFadeOut)
    {
        imgFdIn = imageFadeIn;
        imgFdOut = imageFadeOut;
        txtFdIn = textFadeIn;
        txtFdOut = textFadeOut;
    }

    public void setText(string txt)
    {
        fadeText.text = txt;
    }

    public IEnumerator fadeIn()
    {
        fadeImage.CrossFadeAlpha(0,0,true);
        fadeText.CrossFadeAlpha(0,0,true);
        Time.timeScale = 0;
        fadeImage.CrossFadeAlpha(1,imgFdIn,true);
        yield return new WaitForSecondsRealtime(imgFdIn-txtFdIn);
        fadeText.CrossFadeAlpha(1,txtFdIn,true);
        yield return new WaitForSecondsRealtime(txtFdIn);
        fadeText.CrossFadeAlpha(0,txtFdOut,true);
    }

    public IEnumerator fadeOut()
    {
        fadeImage.CrossFadeAlpha(0,imgFdOut,true);
        yield return new WaitForSecondsRealtime(imgFdOut);
        Time.timeScale = 1;
        Destroy(this.gameObject);

    }
}
