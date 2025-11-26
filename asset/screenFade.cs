// screenFade.cs
// demonstrates fade-in and fade-out of screen; apply this script to 'Main Camera'.
// created at 2025-11-08 by XERONAME
// referenced article: https://discussions.unity.com/t/free-basic-camera-fade-in-script/686081

using UnityEngine;



public class screenFade : MonoBehaviour
{
    private Color Color_texture = new Color(0.0f, 0.0f, 0.0f); // RGB, black color
    public AnimationCurve Curve_fadeOut = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.6f, 0.7f, -1.8f, -1.2f), new Keyframe(1, 0));

    private float fadeLvl = 0.0f;
    private bool fadeProcessing = false;

    [SerializeField] private float fadeWaitDelay = 2.0f;


    // set & apply parameters to fade texture
    private void applyTexture(out Texture2D texture_reference, float alpha_texture) {
        texture_reference = new Texture2D(1,1); // assign new 2d-texture; width=1, height=1 (unit in pixel)

        texture_reference.SetPixel(0,0,
            new Color(Color_texture.r, Color_texture.g, Color_texture.b, alpha_texture)
        ); // set pixel of texture, with RGBA values
        texture_reference.Apply(); // apply the texture
    }

    // single-step of fade transition process, for internal-use; must run in OnGUI() function
    private void guiFadeStep(float alpha_texture) {
        Texture2D _fadeTexture;
        applyTexture(out _fadeTexture, alpha_texture); // set & apply RGBA to texture
        GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), _fadeTexture); // draw texture on screen
    }


    // single-step of fade-out transition process
    public bool fadeOutStep(float process) {
        float _textureAlpha = Curve_fadeOut.Evaluate(process); // set alpha of texture by process in animation-curve
        guiFadeStep(_textureAlpha);

        return (process < 1.0f);
    }

    // single-step of fade-in transition process
    public bool fadeInStep(float process) {
        float _textureAlpha = (1.0f -Curve_fadeOut.Evaluate(process)); // set alpha of texture by process in animation-curve
        guiFadeStep(_textureAlpha);

        return (process < 1.0f);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fadeLvl = 1.0f;
        fadeProcessing = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            fadeLvl = 0.0f;
            fadeProcessing = true;
        }
    }

    void OnGUI()
    {
        if (fadeProcessing) {
            fadeLvl += Time.deltaTime;
            if (fadeLvl < 1.0f) { fadeInStep(fadeLvl); }
            else if (fadeLvl < (fadeWaitDelay +1.0f)) { fadeOutStep(fadeLvl -fadeWaitDelay); }
            else { fadeProcessing = false; }
        }
    }
}