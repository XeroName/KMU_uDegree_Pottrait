using UnityEngine;

/// <summary>
/// 마우스 드래그로 타겟 주변을 공전하는 카메라 컨트롤러
/// - 단일 클릭 + 드래그 : 수평(Yaw) + 수직(Pitch) 회전
/// - 빠른 더블클릭 후 두 번째 클릭을 누른 상태에서 드래그 :
///     - 수평(Yaw) 회전 + 수직 드래그로 줌 인/아웃
/// - 수직 각도 및 줌 거리 범위를 public 변수로 제한 가능
/// - 회전/줌 속도는 해상도와 무관하게 '화면 비율' 기준
/// </summary>
public class DragOrbitCamera : MonoBehaviour
{
    [Header("참조 오브젝트")]
    [Tooltip("카메라가 바라보며 공전할 중심 오브젝트")]
    public Transform target;

    [Tooltip("실제로 움직일 카메라 객체 (Main Camera 등)")]
    public Camera orbitCamera;

    [Header("수평 회전 설정")]
    [Tooltip("화면 전체(가로)를 가로질러 드래그했을 때 회전할 수평 각도(°)")]
    public float horizontalFullScreenAngle = 120f;

    [Tooltip("체크하면 좌/우 드래그 방향이 반전됩니다.")]
    public bool invertX = false;

    [Header("수직 회전 설정 (단일 클릭 드래그용)")]
    [Tooltip("화면 전체(세로)를 가로질러 드래그했을 때 회전할 수직 각도(°)")]
    public float verticalFullScreenAngle = 60f;

    [Tooltip("체크하면 상/하 드래그 방향이 반전됩니다.")]
    public bool invertY = false;

    [Tooltip("수직 각도 최소값 (도 단위, 아래 방향). 예: -30")]
    public float minVerticalAngle = -15f;

    [Tooltip("수직 각도 최대값 (도 단위, 위 방향). 예: 70")]
    public float maxVerticalAngle = 40f;

    [Header("줌 설정 (더블클릭 드래그용)")]
    [Tooltip("카메라와 타겟 사이의 최소 거리")]
    public float minRadius = 0.8f;

    [Tooltip("카메라와 타겟 사이의 최대 거리")]
    public float maxRadius = 1.6f;

    [Tooltip("화면 전체(세로)를 가로질러 드래그했을 때 반지름이 변하는 양")]
    public float zoomFullScreenDistance = 1.2f;

    [Header("입력 설정")]
    [Tooltip("두 번의 클릭 사이가 이 시간(초) 이내면 더블클릭으로 인식")]
    public float doubleClickThreshold = 0.25f;

    // 내부 상태
    private bool isOrbitDragging = false;   // 단일 클릭 드래그(회전 모드)
    private bool isZoomDragging = false;    // 더블클릭 드래그(줌 모드)
    private Vector2 mouseDownPos;          // 드래그 시작 시 마우스 좌표

    private float startYawDeg;             // 드래그 시작 시 수평 각도
    private float startPitchDeg;           // 드래그 시작 시 수직 각도
    private float startRadius;             // 드래그 시작 시 반지름

    private float currentYawDeg;           // 현재 수평 각도
    private float currentPitchDeg;         // 현재 수직 각도
    private float radius;                  // 현재 반지름(타겟과 카메라 거리)

    private float lastClickTime = -1f;     // 마지막 클릭 시각(더블클릭 판정용)

    private void Start()
    {
        if (target == null || orbitCamera == null)
        {
            Debug.LogWarning("[DragOrbitCamera] target 또는 orbitCamera가 할당되지 않았습니다.");
            enabled = false;
            return;
        }

        InitOrbitFromCurrentCamera();
    }

    /// <summary>
    /// 현재 카메라 위치를 기준으로 반지름/수평 각도/수직 각도 계산
    /// </summary>
    private void InitOrbitFromCurrentCamera()
    {
        Vector3 offset = orbitCamera.transform.position - target.position;

        radius = offset.magnitude;
        if (radius < 0.0001f)
            radius = 0.0001f;

        // Yaw: XZ 평면 기준
        currentYawDeg = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;

        // Pitch: 반지름과 높이로부터
        float sinPitch = Mathf.Clamp(offset.y / radius, -1f, 1f);
        currentPitchDeg = Mathf.Asin(sinPitch) * Mathf.Rad2Deg;
        currentPitchDeg = Mathf.Clamp(currentPitchDeg, minVerticalAngle, maxVerticalAngle);
    }

    private void LateUpdate()
    {
        if (target == null || orbitCamera == null)
            return;

        // 마우스 버튼 Down → 단일 클릭/더블클릭 판정
        if (Input.GetMouseButtonDown(0))
        {
            float now = Time.time;
            bool isDoubleClick = (now - lastClickTime) <= doubleClickThreshold;
            lastClickTime = now;

            mouseDownPos = Input.mousePosition;

            // 현재 상태 기준으로 각도/반지름 초기화
            InitOrbitFromCurrentCamera();
            startYawDeg = currentYawDeg;
            startPitchDeg = currentPitchDeg;
            startRadius = radius;

            if (isDoubleClick)
            {
                // 더블클릭 → 줌 모드 시작
                isZoomDragging = true;
                isOrbitDragging = false;
            }
            else
            {
                // 단일 클릭 → 회전 모드 시작
                isOrbitDragging = true;
                isZoomDragging = false;
            }
        }
        // 마우스 버튼 Up → 모든 드래그 모드 종료
        else if (Input.GetMouseButtonUp(0))
        {
            isOrbitDragging = false;
            isZoomDragging = false;
        }

        if (!isOrbitDragging && !isZoomDragging)
            return;

        if (Screen.width <= 0 || Screen.height <= 0)
            return;

        // 드래그 비율 (해상도 무관)
        float deltaXNorm = (Input.mousePosition.x - mouseDownPos.x) / Screen.width;
        float deltaYNorm = (Input.mousePosition.y - mouseDownPos.y) / Screen.height;

        if (invertX) deltaXNorm = -deltaXNorm;
        if (invertY) deltaYNorm = -deltaYNorm;

        if (isOrbitDragging)
        {
            // 단일 클릭 드래그: 회전 전용 (줌 없음)
            float yawDeltaDeg   = deltaXNorm * horizontalFullScreenAngle;
            float pitchDeltaDeg = deltaYNorm * verticalFullScreenAngle;

            currentYawDeg   = startYawDeg   + yawDeltaDeg;
            currentPitchDeg = startPitchDeg + pitchDeltaDeg;
            currentPitchDeg = Mathf.Clamp(currentPitchDeg, minVerticalAngle, maxVerticalAngle);

            radius = startRadius; // 회전 모드에서는 반지름 고정
        }
        else if (isZoomDragging)
        {
            // 더블클릭 드래그: 수평 회전 + 수직 드래그로 줌
            float yawDeltaDeg = deltaXNorm * horizontalFullScreenAngle;
            currentYawDeg = startYawDeg + yawDeltaDeg;

            // 위로 드래그 → deltaYNorm > 0 → radius 감소(줌 인)
            float radiusDelta = -deltaYNorm * zoomFullScreenDistance;
            radius = Mathf.Clamp(startRadius + radiusDelta, minRadius, maxRadius);

            // 줌 모드에서는 Pitch는 고정 (현재 시점 유지)
            currentPitchDeg = startPitchDeg;
        }

        // 각도 → 라디안
        float yawRad   = currentYawDeg   * Mathf.Deg2Rad;
        float pitchRad = currentPitchDeg * Mathf.Deg2Rad;

        // 구면 좌표 → 직교 좌표
        Vector3 offset;
        offset.x = radius * Mathf.Cos(pitchRad) * Mathf.Sin(yawRad);
        offset.y = radius * Mathf.Sin(pitchRad);
        offset.z = radius * Mathf.Cos(pitchRad) * Mathf.Cos(yawRad);

        // 최종 카메라 위치/방향 적용
        Vector3 newCamPos = target.position + offset;
        orbitCamera.transform.position = newCamPos;
        orbitCamera.transform.LookAt(target.position, Vector3.up);
    }
}
