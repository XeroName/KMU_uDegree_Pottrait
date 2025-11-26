using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Rotation : MonoBehaviour
{
    private float accelerate = 2.0f; //가속
    public float maxspeed = 1000.0f; //최대속도
    [SerializeField] private float rotspeed = 0; //현제속도
    
    public TextMeshProUGUI gameoverText; //게임오버
    public TextMeshProUGUI clearText; //클리어
    public float minspinspeed = 300.0f; //최소 요구 속도
    public float qualityspinspeed = 500.0f; //적정 요구 속도
    public float maxspinspeed = 800.0f; //최대 요구 속도
    [SerializeField] private float rotationtimer; // 물레를 돌린 시간
    public Slider healthslider; //채력
    public Slider progressslider; //진행도
    public Slider speedslider; //속도
    public Slider qualitycheck; //품질
    public float health = 100.0f; //체력
    public float quality = 1.0f; //품질
    public float progress = 0f; //진행도
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() //추후 업데이트 수정 예정
    {
        // 1) 메뉴가 열려 있으면 게임 입력 아예 무시
        if (menuButtonHandler.getTransitionProcess() > 0f)
        {
            return;
        }

        if (progress >= 100.0f)
        {
            SceneManager.LoadScene("3_PotterKiln");
        }
        if (health <= 0f)
        {
            SceneManager.LoadScene("1_MainMenu");
        }

        if (rotspeed > 0f) //물레가 돌아갈때 타이머 증가
        rotationtimer += Time.deltaTime;
        else if (rotspeed <= 0f) //물레가 멈췄을때 돌린시간 초기화
        rotationtimer = 0f;
        if (Input.GetMouseButtonDown(0))
        {
            rotspeed += 30 * accelerate; //가속
        }
        else
        {
            rotspeed -= accelerate; //감속
        }
        rotspeed = math.clamp(rotspeed, 0, maxspeed); //최대속도 제한
        transform.Rotate(Vector3.up * rotspeed * Time.deltaTime); //회전
        if (rotationtimer > 2.5f) //일정속도가 넘어가자마자 품질 등급이 감소하면 어색하기 때문에 넣음
        {
            progress +=  2.5f * Time.deltaTime; //100초 너무 느림 / 초당 2.5%
            if (rotspeed >= minspinspeed && rotspeed < qualityspinspeed)
            {
                quality -= 0.01f * Time.deltaTime; //느릴때 퀄리티 감소 / 초당 -1%
            }
            else if (rotspeed > maxspinspeed)
            {
                quality -= 0.01f * Time.deltaTime;
                health -= 5.0f * Time.deltaTime; //빠를때 채력까지 감소 / 초당 -5%
            }
        }
        //ui 갱신
        speedslider.value = rotspeed * 0.001f; 
        healthslider.value = health * 0.01f;
        progressslider.value = progress * 0.01f;
        qualitycheck.value = quality;
        
    }
}
