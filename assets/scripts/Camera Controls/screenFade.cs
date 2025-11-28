// screenFade.cs
// demonstrates fade-in and fade-out of screen; apply this script to 'Main Camera'.
// created at 2025-11-28 by XERONAME

/* ======== referenced articles: ========
https://discussions.unity.com/t/free-basic-camera-fade-in-script/686081
https://www.reddit.com/r/Unity2D/comments/rsa7a9/keep_getting_this_issue_when_i_try_to_create_a/
======== */

using UnityEngine;
using System.Threading.Tasks; // required to use threading and Task.Run()



public class screenFade : MonoBehaviour
{
    private static Texture2D Texture_fade; // 2d-texture for screen fade; width=1, height=1 (unit in pixel)
    private static Color Color_texture = new Color(0.0f, 0.0f, 0.0f); // RGB, black color
    private static float Alpha_texture = 0.0f; // alpha of texture color
    private static AnimationCurve Curve_fadeOut = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 0.0f));

    /* ======== DEMO-SHOWCASE-CODE ========
    private float fadeLvl = 0.0f;
    private bool fadeProcessing = false;
    [SerializeField] private float fadeWaitDelay = 2.0f;
    */


    // single-step of fade-out transition process
    public static bool fadeOutStep(float process) {
        Alpha_texture = Curve_fadeOut.Evaluate(process); // set alpha of texture by process in animation-curve
        return (process < 1.0f);
    }

    // single-step of fade-in transition process
    public static bool fadeInStep(float process) {
        Alpha_texture = (1.0f -Curve_fadeOut.Evaluate(process)); // set alpha of texture by process in animation-curve
        return (process < 1.0f);
    }


    // Unity built-in function, Start()
    void Start()
    {
        // Texture2D MUST be assigned only in main thread!
        Texture_fade = new Texture2D(1,1); // assign new 2d-texture to variable; width=1, height=1 (unit in pixel)
    }

    // Unity built-in function, Update() is called once per frame
    void Update()
    {
        /* ======== DEMO-SHOWCASE-CODE ========
        if (Input.GetKeyDown(KeyCode.Space)) {
            fadeLvl = 0.0f;
            fadeProcessing = true;
        }
        */
        if (Input.GetKeyDown(KeyCode.Space)) { GM.open.transScene("CameraFade"); }
    }

    // Unity built-in function, OnGUI()
    void OnGUI()
    {
        /* ======== DEMO-SHOWCASE-CODE ========
        if (fadeProcessing) {
            fadeLvl += Time.deltaTime;
            if (fadeLvl < 1.0f) { fadeInStep(fadeLvl); }
            else if (fadeLvl < (fadeWaitDelay +1.0f)) { fadeOutStep(fadeLvl -fadeWaitDelay); }
            else { fadeProcessing = false; }
        }
        */

        // set pixel attribute of texture, and apply it
        Texture_fade.SetPixel(0,0,
            new Color(Color_texture.r, Color_texture.g, Color_texture.b, Alpha_texture)
        ); // set pixel of texture, with RGBA values
        Texture_fade.Apply(); // apply the texture

        GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), Texture_fade); // draw texture on screen; MUST be run in OnGUI() function
    }
}
