// LoadingCanvasCtrl.cs
// 2025-11-28 by XERONAME
// put this script into 'LoadingCanvas' canvas.

using UnityEngine;
using UnityEngine.UI; // required to use UI
using TMPro; // required to use TextMeshPro



public class LoadingCanvasCtrl : MonoBehaviour
{
    private GameObject obj_loadingCanvas; // LoadingCanvas object
    private GameObject obj_screenPanel; // ScreenPanel object
    private GameObject obj_loadingText; // LoadingText object

    private Canvas comp_loadingCanvas; // Canvas component of LoadingCanvas object
    private Image comp_screenPanel; // Image component of ScreenPanel object
    private TMP_Text comp_loadingText; // TextMeshPro-text component of LoadingText object

    private RectTransform rctTrsf_loadingCanvas; // RectTransform component of Canvas
    private RectTransform rctTrsf_loadingText; // RectTransform component of TextMeshPro-text

    private string str_screenPanel = "ScreenPanel"; // string-name of ScreenPanel
    private string str_loadingText = "LoadingText"; // string-name of LoadingText

    private float widthScale_loadingText = 0.85f;
    private float fontSizeScale_loadingText = 0.05f;

    private static float fontSize_loadingText;
    private static string text_toInform;


    // set loading text to desired string
    public static void setLoadingText(string textToShow, float fontSize=0) {
        text_toInform = textToShow;
        fontSize_loadingText = fontSize;
    }

    // refresh the whole loading screen
    private void refreshLoadingScreen(bool useAutomaticFontSize=true) {
        // set width of LoadingText
        rctTrsf_loadingText.sizeDelta = new Vector2(
            (rctTrsf_loadingCanvas.rect.width *widthScale_loadingText),
            rctTrsf_loadingText.sizeDelta.y
        );

        // set font size of LoadingText
        if (useAutomaticFontSize)
        { comp_loadingText.fontSize = (rctTrsf_loadingText.sizeDelta.x *fontSizeScale_loadingText); }

        // set text of LoadingText
        if (text_toInform != null)
        { comp_loadingText.text = text_toInform; }
    }


    // build-in function: Start()
    void Start()
    {
        // get object and components
        obj_loadingCanvas = (GameObject)gameObject;
        obj_screenPanel = obj_loadingCanvas.transform.Find(str_screenPanel).gameObject;
        obj_loadingText = obj_loadingCanvas.transform.Find(str_loadingText).gameObject;

        comp_loadingCanvas = obj_loadingCanvas.GetComponent<Canvas>();
        comp_screenPanel = obj_screenPanel.GetComponent<Image>();
        comp_loadingText = obj_loadingText.GetComponent<TMP_Text>();

        rctTrsf_loadingCanvas = comp_loadingCanvas.GetComponent<RectTransform>();
        rctTrsf_loadingText = comp_loadingText.GetComponent<RectTransform>();
    }

    // build-in function: Update()
    void Update()
    {
        refreshLoadingScreen(true); // refresh the loading screen
    }
}
