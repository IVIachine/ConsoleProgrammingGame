using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Script that fades out and changes to a target scene, will also fade into target scene
/// </summary>
public class FadeScript : MonoBehaviour
{

    //Cite:https://www.youtube.com/watch?v=0HwZQt94uHQ
    public Texture2D theBlackScreen;
    public float fadeSpeed = .8f;
    private static float speed;
    private int drawDepth = -1000;
    private float alpha = 1.0f;
    private static int fadeDirection = -1;

    private void Start()
    {
        speed = fadeSpeed;
    }

    void OnGUI()
    {
        alpha += fadeDirection * fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), theBlackScreen);
    }

    public float beginFade(float newFadingSpeed, int direction, string targetLevel)
    {
        if (newFadingSpeed != 0)
            fadeSpeed = newFadingSpeed;

        fadeDirection = direction;
        if (targetLevel != "")
            StartCoroutine(sceneSwitch(targetLevel));

        return (speed);
    }

    void Awake()
    {
        beginFade(fadeSpeed, -1, "");
    }

    IEnumerator sceneSwitch(string targetLevel)
    {
        yield return new WaitForSeconds(.8f);
        SceneManager.LoadScene(targetLevel);
        yield break;
    }
}
