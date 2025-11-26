// menuButtonHandler.cs
// 2025-11-25 by XERONAME
// i did not use ChatGPT on this code, btw.

using UnityEngine;
using UnityEngine.UI; // required to use button



public class menuButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] childButtons;
    [SerializeField] private float transitionSpeed = 2.0f;
    [SerializeField] private float buttonSpacingOffset = 32.0f;

    private Button menuButton;
    private bool collapseMenuBar = true;
    private static float transitionProcess = 0.0f;


    public static float getTransitionProcess() { return transitionProcess; }

    private void menuBtnClicked() {
        Debug.Log("clicked");
        collapseMenuBar = (!collapseMenuBar);
        singleTransitionProcess();
    }

    private void singleTransitionProcess()
    { transitionProcess += (Time.deltaTime *transitionSpeed *( (System.Convert.ToSingle(!collapseMenuBar) *2.0f) -1.0f )); }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transitionProcess = 0.0f;
        collapseMenuBar = true;

        menuButton = gameObject.GetComponent<Button>();
        menuButton.onClick.AddListener(menuBtnClicked);
    }

    // Update is called once per frame
    void Update()
    {
        // process the transition, if transitionProcess is within valid range
        if ((0.0f < transitionProcess) && (transitionProcess < 1.0f))
        { singleTransitionProcess(); }

        else if (transitionProcess < 0.0f) { transitionProcess = 0.0f; } // clip transitionProcess to minimum value of range
        else if (1.0f < transitionProcess) { transitionProcess = 1.0f; } // clip transitionProcess to maximum value of range

        // handle the each child-buttons
        int _cnt = 0; // declare the temporary-use counter variable
        foreach (GameObject _btnObj in childButtons) {
            if (transitionProcess == 0.0f) { _btnObj.SetActive(false); } // hide and disable the button

            else {
                Button _btn = _btnObj.GetComponent<Button>(); // get button component of child-button-object
                //_btn.image.color = new Color(_btn.image.color.r, _btn.image.color.g, _btn.image.color.b, transitionProcess);
                if (transitionProcess == 1.0f) { _btn.interactable = true; }
                else { _btn.interactable = false; }
                _btnObj.SetActive(true);

                // set position of target child-button-object
                float _btnOfsY = -(buttonSpacingOffset *(_cnt +1) *transitionProcess);
                _btnObj.transform.position = new Vector3(
                    gameObject.transform.position.x,
                    (gameObject.transform.position.y +_btnOfsY),
                    _btnObj.transform.position.z
                );
                _cnt++; // increase the temp counter
            }
        }
    }
}
