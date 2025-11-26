using UnityEngine;
using UnityEngine.UI; // UI 사용


public class PotteryPainter : MonoBehaviour
{
    public Camera cam;                     // 카메라
    public ComputeShader paintShader;      // 페인트용 컴퓨트셰이더
    public RenderTexture glossMask;        // 유약 마스크
    public Texture2D brushTexture;         // 원형 브러시 텍스처
    public Slider paintProgressSlider;

    public static bool touchOnPottery = false;
    private float checkInterval = 0.5f; // 0.5초마다 계산
    private float timer = 0f;

    int kernel;

    void Start()
    {
        kernel = paintShader.FindKernel("CSMain");

        glossMask.enableRandomWrite = true;
        glossMask.Create();

        // 🔥 RenderTexture 완전 초기화
        ClearRenderTexture(glossMask);

        paintShader.SetTexture(kernel, "GlossMask", glossMask);
        paintShader.SetTexture(kernel, "Brush", brushTexture);
    }

    void ClearRenderTexture(RenderTexture rt)
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = rt;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = active;
    }

    void Update()
    {

        // 첫 터치 시 회전 금지 여부 결정
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 도자기만 색칠 가능하게 하려면 태그 사용 권장
                if (hit.collider.gameObject == this.gameObject)
                    touchOnPottery = true;   // 유약칠 모드
                else
                    touchOnPottery = false;  // 회전 모드
            }
            else
            {
                touchOnPottery = false;      // 빈 공간 터치 = 회전 모드
            }
        }
        if (Input.GetMouseButton(0) && touchOnPottery)
        {
            Paint();
        }

        // ⭐ 손가락 뗐을 때 초기화
        if (Input.GetMouseButtonUp(0))
        {
            touchOnPottery = false;
        }
            
        
        timer += Time.deltaTime;

        if (timer >= checkInterval)   // checkInterval = 0.5f
        {
            timer = 0f;

            float percent = CalculatePaintPercentage();
            paintProgressSlider.value = percent * 3.3f;
        }
    }

    void Paint()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector2 uv = hit.textureCoord;

            paintShader.SetFloats("paintUV", uv.x, uv.y);
            paintShader.SetFloat("brushStrength", 1.0f);

            paintShader.SetInt("texWidth", glossMask.width);
            paintShader.SetInt("texHeight", glossMask.height);
            paintShader.SetFloat("brushSize", 0.05f);

            int kernel = paintShader.FindKernel("CSMain");

            paintShader.Dispatch(kernel,
                glossMask.width / 8,
                glossMask.height / 8,
                1);
        }
    }
    Texture2D ConvertRTToTexture2D(RenderTexture rt)
    {
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.R8, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        return tex;
    }
    float CalculatePaintPercentage()
    {
        Texture2D tex = ConvertRTToTexture2D(glossMask);
        Color32[] pixels = tex.GetPixels32();
        int total = pixels.Length;

        float sum = 0f;
        for (int i = 0; i < total; i++)
            sum += pixels[i].r / 255f;

        return (sum / total) * 100f;
    }

    

}
