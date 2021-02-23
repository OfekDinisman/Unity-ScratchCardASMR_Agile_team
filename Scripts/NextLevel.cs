using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class NextLevel : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    #region IPointerClickHandler implementation

    public void OnPointerClick(PointerEventData eventData)
    {
        MyOwnEventTriggered();
    }
    #endregion

    //my event
    [Serializable]
    public class MyOwnEvent : UnityEvent { }

    [SerializeField]
    private MyOwnEvent myOwnEvent = new MyOwnEvent();
    public MyOwnEvent onMyOwnEvent { get { return myOwnEvent; } set { myOwnEvent = value; } }
    private string SceneName;
    private string NextSceneName;
    Slider progressBar;
    bool showLoading = false;

    public void MyOwnEventTriggered()
    {
       // SaveNextSceneName();
        GoToNextLevel();

    }
    public static void GoToNextLevel()
    {
        changeImages();
       
    }
    

    void Start()
    {
        imgNumberCount = 2;
    }
    private static void GoNextLvl(string maskImg,string BGImg, int lvlNum,string sound_name="")
    {
        //reset slider
        GameObject gObject = GameObject.Find("Slider");
        gObject.SetActive(false);
        ScratchCardEffect.ResetScore();
        gObject.SetActive(true);
        //change sound 
        if (!String.IsNullOrEmpty(sound_name))
        {
            soundObject.clip = (AudioClip)Resources.Load("Sounds/"+ sound_name);
            soundObject.Play();
            Debug.Log(sound_name);
            Debug.Log(soundObject.name);
        }
        //change lvl number
        txtLvl.text = "Level " + lvlNum;
        Sprite sprite;
        //background image
        sprite = Resources.Load<Sprite>("Images/" + BGImg);
        RawImageObject.GetComponent<Image>().sprite = sprite;
        //mask image
        maskObject.gameObject.SetActive(false);
        sprite = Resources.Load<Sprite>("Images/" + maskImg);
        Debug.Log(sprite.name);
        maskObject.GetComponent<Image>().sprite = sprite;
        imgNumberCount++; //increase count so it gets higher and switches to different sprite
        maskObject.gameObject.SetActive(true);

    }
    public static int imgNumberCount;
    private static GameObject maskObject;
    private static GameObject RawImageObject;
    private static AudioSource soundObject;
    private static TextMeshProUGUI txtLvl;
    public static void changeImages() // make sure to attach this to event trigger
    {
        Debug.Log(imgNumberCount);
        Sprite sprite;
        string imageName;
        string next_imageName;
        string sound_name;
         maskObject = GameObject.FindGameObjectWithTag("MaskImg");
         RawImageObject = GameObject.FindGameObjectWithTag("RawImg");
        soundObject= GameObject.FindObjectOfType<AudioSource>();
        //soundObject.Stop();
        txtLvl = GameObject.FindObjectOfType<TextMeshProUGUI>();
        Debug.Log(txtLvl.text);
        switch (imgNumberCount)
        {

            case 2:
                imageName = "window";
                next_imageName = "home";
                sound_name = "city";
                GoNextLvl(imageName, next_imageName, imgNumberCount, sound_name);

                break;
            case 3:
                imageName = "home";
                next_imageName = "sea";
                sound_name = "sea";
                GoNextLvl(imageName, next_imageName, imgNumberCount, sound_name);
                break;
            case 4:
                imageName = "sea";
                next_imageName = "underSea";
                sound_name = "";
                GoNextLvl(imageName, next_imageName, imgNumberCount, sound_name);
                break;
            case 5:
                imageName = "underSea";
                next_imageName = "boat";
                sound_name = "boat";
                GoNextLvl(imageName, next_imageName, imgNumberCount, sound_name);
                break;
            case 6:
                imageName = "boat";
                next_imageName = "party";
                sound_name = "party";
                GoNextLvl(imageName, next_imageName, imgNumberCount, sound_name);
                break;
            case 7:
                imageName = "party";
                next_imageName = "EndGame";
                sound_name = "";
                GoNextLvl(imageName, next_imageName, imgNumberCount, sound_name);
                break;
            default:
                Debug.Log("end");
                break;
        }
    }

private void SaveNextSceneName()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneName = scene.name;
        Debug.Log(SceneName);
        int sceneNum = 0;
        bool isNum = int.TryParse(SceneName.Split('_')[1], out sceneNum);
        Debug.Log(isNum);
        NextSceneName = "Level_" + (sceneNum + 1);
        Debug.Log(NextSceneName);
    }

    public GameObject loadingScreen;



}
