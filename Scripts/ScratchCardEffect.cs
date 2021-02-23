using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScratchCardEffect : MonoBehaviour
{
    public Camera maskCamera;
    public GameObject maskObject;
    public GameObject CoinObject;
    public Material EraserMaterial;
    public Vector2 erasorSize;
    public static int currentScore;

    private Texture2D tex;
    private Rect ScreenRect;
    private RenderTexture rt;
    private bool firstFrame;
    private Vector2? newHolePosition;
    private RectTransform rectTransform;
    private Camera mainCamera;
    private static bool[] Arr =new bool[18*9];//size of picture to scratch
    private int MAX_Score_to_finish = 23;//24 =max =all screen
    AudioSource sound;
    Vector2 v;//temp vector for mouse position 

    

    private string SceneName;
    private string NextSceneName;
    public static void ResetScore()
    {
        Arr = new bool[18 * 9];
        currentScore = 0;
    }
    void Start()
    {
        CoinObject= GameObject.Find("Coin");
        for (int i = 0; i < Arr.Length; i++)
        {
            Arr[i] = false;
        }

        SaveNextSceneName();

       // StartSound();
        
    }
   
    private void StartSound()
    {
        try
        {
            if (sound != null)
            {
                sound.Stop();
            }
            if (SceneName.Equals("Level_1"))//rainSound
            {
                //sound = gameObject.GetComponent<AudioSource>();
                //sound.Play();
            }
            
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }
    private void SaveNextSceneName()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneName = scene.name;
        Debug.Log(SceneName);
        int sceneNum = 0;
        bool isNum=int.TryParse(SceneName.Split('_')[1],out sceneNum);
        Debug.Log(isNum);
        NextSceneName = "Level_" + (sceneNum + 1);
        Debug.Log(NextSceneName);
    }
    private void ChangeLevel()
    {
        NextLevel.GoToNextLevel();
    }
    bool isOver()
    {
        int scratch_cnt = 0;
        for (int i = 0; i < Arr.Length; i++)
        {
            if (Arr[i]==true)
            {
                scratch_cnt++;
            }
        }
        if (scratch_cnt >= MAX_Score_to_finish)
        {//scratched 90% of the image
           // Debug.Log("GameOver");
            //sound = gameObject.GetComponent<AudioSource>();
            // sound.Play();
            //  SceneManager.LoadScene(NextSceneName);
            ChangeLevel();
            return true;
        }
        currentScore = scratch_cnt;
        //Debug.Log(scratch_cnt);
        return false;
    }
    private void EraseBrush(Vector2 imageSize, Vector2 imageLocalPosition)
    {
        Rect textureRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f); //this will get erase material texture part
        Rect positionRect = new Rect(
            (imageLocalPosition.x - 0.5f * EraserMaterial.mainTexture.width) / imageSize.x,
            (imageLocalPosition.y - 0.5f * EraserMaterial.mainTexture.height) / imageSize.y,
            EraserMaterial.mainTexture.width / imageSize.x,
            EraserMaterial.mainTexture.height / imageSize.y
        ); //This will Generate position of eraser according to mouse position and size of eraser texture

        //Draw Graphics Quad using GL library to render in target render texture of camera to generate effect
        GL.PushMatrix();
        GL.LoadOrtho();
        for (int i = 0; i < EraserMaterial.passCount; i++)
        {
            EraserMaterial.SetPass(i);
            GL.Begin(GL.QUADS);
            GL.Color(Color.white);
            GL.TexCoord2(textureRect.xMin, textureRect.yMax);
            GL.Vertex3(positionRect.xMin, positionRect.yMax, 0.0f);
            GL.TexCoord2(textureRect.xMax, textureRect.yMax);
            GL.Vertex3(positionRect.xMax, positionRect.yMax, 0.0f);
            GL.TexCoord2(textureRect.xMax, textureRect.yMin);
            GL.Vertex3(positionRect.xMax, positionRect.yMin, 0.0f);
            GL.TexCoord2(textureRect.xMin, textureRect.yMin);
            GL.Vertex3(positionRect.xMin, positionRect.yMin, 0.0f);
            GL.End();
        }
        GL.PopMatrix();
    }

    private void OnEnable()
    {
        maskCamera.GetComponent<CameraPostEffect>().OnPostEffect += OnPost;
        StartCoroutine(StartScratch());
    }

    private void OnDisable()
    {
        maskCamera.GetComponent<CameraPostEffect>().OnPostEffect -= OnPost;
        maskCamera.targetTexture = null;
        maskObject.GetComponent<Image>().material.SetTexture("_MaskTex", null);
    }

    public IEnumerator StartScratch()
    {
        firstFrame = true;
        //Get Erase effect boundary area
        rectTransform = maskObject.GetComponent<RectTransform>();
        ScreenRect = rectTransform.rect;
        mainCamera = Camera.main;

        //Create new render texture for camera target texture
        var renderTextureDesc = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.ARGB32, 0);
        rt = new RenderTexture(renderTextureDesc);
        yield return rt.Create();

        Graphics.Blit(tex, rt, maskObject.GetComponent<Image>().material);
        maskCamera.targetTexture = rt;

        yield return null;

        //Set Mask Texture to dust material to Generate Dust erase effect
        maskObject.GetComponent<Image>().material.SetTexture("_MaskTex", rt);
    }

    public void Update()
    {
        newHolePosition = null;

        if (Input.GetMouseButton(0)) //Check if MouseDown
        {
             v = Vector2.zero;
            Rect worldRect = ScreenRect;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, mainCamera, out Vector2 localPos))
            {
                v = localPos;
            }

            if (worldRect.Contains(v))
            {
                newHolePosition = new Vector2(erasorSize.x * (v.x - worldRect.xMin) / worldRect.width, erasorSize.y * (v.y - worldRect.yMin) / worldRect.height);
            }

            
           
        }
    }
    public void FixedUpdate()
    {//update coin position
        //CoinObject.gameObject.transform.position = new Vector3(v.x * 0.04f, v.y * 0.038f, 80f);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CoinObject.gameObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, 2f);

        //turnaround the coin effect
        //CoinObject.transform.Rotate(new Vector3(10, 10, 0) * Time.deltaTime*2);
        //Debug.Log(v.x * 0.04f);
        //Debug.Log(v.y * 0.038f);

        //check reveled parts
        //calcuate index in array by position
        int sum= (int) (worldPosition.x+ worldPosition.y);
        if (sum < 0)//no negative indexes
        {
            sum += 162;
        }
        Arr[sum] = true;
        //check gameOver
        isOver();

       
    }
    public void OnPost()
    {
        if (firstFrame)
        {
            firstFrame = false;
            GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
        }

        //Generate GL quad according to eraser material texture
        if (newHolePosition != null)
        {
            EraseBrush(new Vector2(erasorSize.x, erasorSize.y), newHolePosition.Value);
        }
    }
}