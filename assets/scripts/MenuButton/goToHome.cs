// goToHome.cs
// 2025-11-28 by XERONAME

using UnityEngine;
using UnityEngine.UI; // required to use Button

public class goToHome : MonoBehaviour
{
    [SerializeField] private string nameOfHomeScene;

    public void goHome() { GM.open.transSceneWithLoading(nameOfHomeScene, "I REALLY SO MUCH HATE THE UNITY ENGINE"); }

    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(goHome); // connect function, to trigger when button clicked
    }
}
