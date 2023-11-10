using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;


public class DownloadHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        

        bool internet;

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            internet = false;
        }
        else
        {
            internet = true;
        }

        StartCoroutine(tryDownload(internet)); // Download required
        DeleteFilesStartingWith(); // Delete required

    }


    void DeleteFilesStartingWith()
    {
        //string folderPath = Application.persistentDataPath;
        // Get all .txt files in the folder
        //string[] txtFiles = Directory.GetFiles(folderPath, "*");
        DirectoryInfo d = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] txtFiles = d.GetFiles();

        foreach (FileInfo file in txtFiles)
        {
            // Read the content of the file
            string content = File.ReadAllText(file.FullName);


            // Check if the content starts with '<'
            if (!string.IsNullOrEmpty(content) && content[0] == '<')
            {
                Debug.Log("iiiii"+content[0]);
                file.Delete();
                Debug.Log($"Deleted: {file.FullName}");
            }

            //if () { } // Here is for delete the past levels since they have been passed and no longer needed.
        }
    }

    IEnumerator tryDownload(bool checkInternet)
    {
        int counter = 0;

        while (!checkInternet && counter < 30)
        {
            yield return new WaitForSeconds(.3f);

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                checkInternet = false;
            }
            else
            {
                checkInternet = true;
            }
        }
        Download();
    }


    private void Download()
    {
        DirectoryInfo d = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] Files = d.GetFiles();
        Debug.Log("******" + Files.Length);

        int currentLevel = 0;
        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        }


        // If download required.
        if (Files.Length < currentLevel + 1)
        {
            // Download the remaining levels when required.
            // ***********************************
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("Error. Check internet connection!");
            }
            else
            {
                int lvl = currentLevel;
                string url;
                int counter = 0;

                while (counter <= 10)
                {
                    string level_name = lvl.ToString();
                    url = "https://settozero.s3.eu-north-1.amazonaws.com/" + level_name;

                    StartCoroutine(DownloadFile(url, level_name)); // Download unit.
                    lvl++;
                    counter++;
                }
            }
        }
        else if (Files.Length < currentLevel + 2)
        {
            // Download the remaining levels when required.
            // ***********************************
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("Error. Check internet connection!");
            }
            else
            {
                int lvl = currentLevel + 1;
                string url;
                int counter = 0;

                while (counter <= 10)
                {
                    string level_name = lvl.ToString();
                    url = "https://settozero.s3.eu-north-1.amazonaws.com/" + level_name;

                    StartCoroutine(DownloadFile(url, level_name)); // Download unit.
                    lvl++;
                    counter++;
                }
            }


        }
    }


    // Download file unit. To be called iteratively in co-routine in start method.
    IEnumerator DownloadFile(string url, string level_name)
    {
        var uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        //Debug.Log(uwr.url.ToString());
        string path = Path.Combine(Application.persistentDataPath + "/", level_name);
        //Debug.Log(path);
        if (!Directory.Exists(path))
        {
            // This try catch is only for avoiding a dummy error on windows.
            try
            {
                uwr.downloadHandler = new DownloadHandlerFile(path);
            }
            // No need to catch any error or perform any operation
            // Because windows gives a dummy error without it despite all functionality exist.
            catch
            { }
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
                Debug.LogError(uwr.error);
            //else
            //    Debug.Log("File successfully downloaded and saved to " + path);  
        }
    }
}
