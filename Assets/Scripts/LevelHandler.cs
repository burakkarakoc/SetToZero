using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class LevelHandler : MonoBehaviour
{
    public World world;

    public GameObject adsButton;

    public Sprite normalSprite; 
    public Sprite newSprite; 
    private Image imageComponent;

    private Button myButton;
    public TMP_Text levelText;
    private int level;

    private Sprite initialSprite;


    private void Awake()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(OpenLevel);

        initialSprite = GetComponent<Sprite>();

        imageComponent = GetComponent<Image>();

        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            //PlayerPrefs.SetInt("CurrentLevel",2);
            level = PlayerPrefs.GetInt("CurrentLevel");
        }

        Debug.Log(level);

        //// If there is a world object for board, then perform required initializations
        //if (world != null)
        //{
        //    //world.generateLevels();
        //    //world.firstTenLevelsStatic();

        //    world.fillLevelsToWorld();
        //}
        //else
        //{
        //    Debug.Log("World could not be found!!!!");
        //}
    }


    public void ChangeSprite()
    {
        int currentLives = currentLives = PlayerPrefs.GetInt("currentLives", 3);

        if (imageComponent && newSprite && currentLives <= 0)
        {
            imageComponent.sprite = newSprite;
            myButton.interactable = false;
            levelText.text = "Watch ads to earn life";
            adsButton.SetActive(true);
        }
        else
        {
            adsButton.SetActive(false);
            imageComponent.sprite = normalSprite;
            myButton.interactable = true;
            setLevel();
            if (level == 0)
            {
                levelText.text = "Tutorial";
            }
            else
            {
                levelText.text = level.ToString();
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        adsButton.SetActive(false);
        setLevel();
        if (level == 0)
        {
            levelText.text = "Tutorial";
        }
        else
        {
            levelText.text = level.ToString();
        }

        ChangeSprite();

        Debug.Log(level);

    }


    private void setLevel()
    {
        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            level = PlayerPrefs.GetInt("CurrentLevel");
        }
        else
        {
            PlayerPrefs.SetInt("CurrentLevel", 0);
        }
    }

    // Directs to level scene and sets the current level of the user
    public void OpenLevel()
    {
        // If there is a world object for board, then perform required initializations
        if (world != null)
        {
            //world.generateLevels();
            //world.firstTenLevelsStatic();

            world.fillLevelsToWorld();
        }
        else
        {
            Debug.Log("World could not be found!!!!");
        }

        SceneManager.LoadScene("LevelScene");
    }
}
