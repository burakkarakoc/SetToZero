using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainFadeController : MonoBehaviour
{

    public Animator panelAnim;
    public GameObject settingPanel;
    public GameObject fadePanel;

    private void Awake()
    {
        //panelAnim.SetBool("Out", true);
    }

    public void OpenSettings()
    {
        fadePanel.SetActive(true);
        panelAnim.SetBool("Out", false);
        panelAnim.SetBool("Stop Game", true);
        settingPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        panelAnim.SetBool("Out", true);
        panelAnim.SetBool("Stop Game", false);
        fadePanel.SetActive(false);
        //settingPanel.SetActive(false);
    }
}
