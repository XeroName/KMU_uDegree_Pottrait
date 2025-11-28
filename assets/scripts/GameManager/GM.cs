// GM.cs
// Game-Manager of Pottrait game
// 2025-11-28 by XERONAME
// i really hate unity engine

/* ==== Referenced articles: ====
https://glikmakesworld.tistory.com/2
https://kimyc1223.github.io/blog/2024/06/23/TechPost.html
https://wlsdn629.tistory.com/entry/%EC%9C%A0%EB%8B%88%ED%8B%B0-%EC%8B%B1%EA%B8%80%ED%86%A4%ED%8C%A8%ED%84%B4%EC%97%90-%EB%8C%80%ED%95%B4%EC%84%9C-Singleton-Pattern
*/

using UnityEngine;
using UnityEngine.SceneManagement; // required to load scene
using System.Collections; // required for coroutine



public class GM : MonoBehaviour
{
    private string sceneName_loadingScreen = "LoadingScreen"; // string-name of LoadingScreen scene
    private static float duration_loadingScreen = 3.0f;


    // private instance of GM
    private static GM inst_private;

    // public instance of GM
    public static GM open
    {
        // enable read-access for inst_private(private GM)
        get {
            if (inst_private == null) {
                inst_private = FindFirstObjectByType<GM>(); // find GameManager object, if inst_private is not valid

                if (inst_private == null) {
                    GameObject _obj = new GameObject("GameManager"); // create new GameManager object, if cannot find GM object
                    inst_private = _obj.AddComponent<GM>(); // add GM class to object as component
                    DontDestroyOnLoad(_obj); // set object not to destroy
                }
            }
            return inst_private;
        }
    }


    // switch to desired scene
    private void loadScene(string nameOfScene, LoadSceneMode loadSceneMode=LoadSceneMode.Single) {
        // LoadSceneMode.Single (default): Closes all currently loaded scenes and loads the new scene.
        // LoadSceneMode.Additive: Adds the new scene to the existing loaded scenes, allowing for multi-scene setups (e.g., loading environmental chunks or separate UI scenes).
        if (nameOfScene != null) { SceneManager.LoadScene(nameOfScene, loadSceneMode); }
    }

    // process full screen fade-in or fade-out transition; co-routine function for async-looking task
    private IEnumerator fullScreenFade(float fadeTime, bool isFadeOut) {
        float _process = 0.0f; // initialize the process value
        Debug.Log("GM: fade process started");

        // fade-out processing
        if (isFadeOut) {
            while (screenFade.fadeOutStep(_process)) {
                _process += (Time.deltaTime /fadeTime);
                yield return null; // wait until next-frame
            }
        }

        // fade-in processing
        else {
            while (screenFade.fadeInStep(_process)) {
                _process += (Time.deltaTime /fadeTime);
                yield return null; // wait until next-frame
            }
        }
    }


    // switch scene with fade-in and fade-out; co-routine function for async-looking task
    private IEnumerator switchSceneWithFade(string nameOfScene, float fadeTime) {
        yield return StartCoroutine(fullScreenFade(fadeTime, false)); // process fade-in screen transition; wiat until it finished

        loadScene(nameOfScene, LoadSceneMode.Single); // load target scene

        yield return StartCoroutine(fullScreenFade(fadeTime, true)); // process fade-out screen transition; wiat until it finished
    }

    // switch scene with fade-in and fade-out; passes through LoadingScreen for duration; co-routine function for async-looking task
    private IEnumerator switchSceneWithLoading(string nameOfScene, float fadeTime, string loadingText) {
        yield return StartCoroutine(fullScreenFade(fadeTime, false)); // process fade-in screen transition; wiat until it finished
        loadScene(sceneName_loadingScreen, LoadSceneMode.Single); // load LoadingScreen scene
        StartCoroutine(fullScreenFade(fadeTime, true)); // process fade-out screen transition

        LoadingCanvasCtrl.setLoadingText(loadingText); // set text to visible at LoadingScreen
        yield return new WaitForSeconds(duration_loadingScreen); // wait for duration

        yield return switchSceneWithFade(nameOfScene, fadeTime);
    }


    // start smooth-transition of scene with screen fade
    public void transScene(string nameOfScene, float fadeTime=1.0f)
    { StartCoroutine(switchSceneWithFade(nameOfScene, fadeTime)); }

    // start smooth-transition of scene with screen fade and LoadingScreen
    public void transSceneWithLoading(string nameOfScene, string loadingText, float fadeTime=1.0f)
    { StartCoroutine(switchSceneWithLoading(nameOfScene, fadeTime, loadingText)); }
}
