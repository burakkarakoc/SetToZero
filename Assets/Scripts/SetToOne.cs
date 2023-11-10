using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//using System.IO;

public class SetToOne : MonoBehaviour
{

    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SetOne);
        //DirectoryInfo d = new DirectoryInfo(Application.persistentDataPath);
        //print(d);
    }

    public void SetOne()
    {
        PlayerPrefs.SetInt("CurrentLevel", 0);
        SceneManager.LoadScene("Main");
    }
}
