using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using UnityEngine.SceneManagement;


public class Bake : MonoBehaviour
{
    public Slider healthslider; //채력
    public Slider progressslider; //진행도
    public Slider qualityslider; //품질
    public Slider temperatureslider; //온도
    public float health = 100.0f; //채력
    public float quality = 1.0f; //품질
    public float progress = 0; //진행도
    public float temperature = 0; //온도
    private float maxtemperature = 1500.0f;
    private float mintemp = 1200f;
    private float maxtemp = 1350f;
    public bool isClear = false;
    public bool isAlive = true;
    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (progress >= 100.0f)
        {
            SceneManager.LoadScene("4_PotterGlaze");
        }
        if (health <= 0f)
        {
            SceneManager.LoadScene("1_MainMenu");
        }
        if (temperature > 0f)
        timer += Time.deltaTime;
        else if (temperature <= 0f)
        timer = 0f;
        if (Input.GetMouseButtonDown(0))
        {
            temperature += 40f;
        }
        else
        {
            temperature -= 0.5f * (1f + temperature * 0.002f);
        }
        temperature = math.clamp(temperature, 0, maxtemperature);
        if (timer > 6f)
        {
            progress +=  2.5f * Time.deltaTime; //100초 너무 느림 / 초당 2.5%
            if (temperature < mintemp || temperature > maxtemp)
            {
                quality -= 0.01f * Time.deltaTime; //퀄리티 감소 / 초당 -1%
                health -= 3.0f * Time.deltaTime; //채력 감소 / 초당 -3%
            }
        }
        //ui 갱신
        healthslider.value = health * 0.01f;
        progressslider.value = progress * 0.01f;
        qualityslider.value = quality;
        temperatureslider.value = temperature / maxtemperature;
    
    }
}
