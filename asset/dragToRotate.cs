// dragToRotate.cs
// 2025-11-25 by XERONAME

using UnityEngine;

public class dragToRotate : MonoBehaviour
{
    static private bool PRINT_INFO_DEBUG = false; 
    static private bool PRINT_WARN_DEBUG = true; 

    static private bool STAT_MOUSE_LEFTKEY, STAT_MOUSE_RIGHTKEY, STAT_MOUSE_WHEELKEY, STAT_MOUSE_ANYKEY;
    static private bool STAT_MOUSE_LEFTKEY_PREV = false;

    [SerializeField] private GameObject obj_pottery; 
    // 🔥 Rigidbody 제거됨
    // [SerializeField] private Rigidbody rb_pottery; 

    private float drag_startMouseX, drag_latestFrameMouseX, drag_xDistInScreen, drag_angleVelocity;
    private Vector3 drag_baseAngles;

    [SerializeField] private float drag_rotationScale = 1.0f; 
    [SerializeField] private float multiplier_airResistance = 0.99f;

    // 🔥 Rigidbody 대체용 관성 회전 속도
    private float simulatedAngularVelocity = 0f;

    void Start()
    {
        if (PRINT_WARN_DEBUG)
        {
            if (obj_pottery == null) { Debug.LogWarning("GameObject of pottery not found!"); }
            // if (rb_pottery == null) { Debug.LogWarning("Rigidbody of pottery-object not found!"); }
        }
    }

    void Update()
    {
        STAT_MOUSE_LEFTKEY = Input.GetMouseButton(0);
        STAT_MOUSE_RIGHTKEY = Input.GetMouseButton(1);
        STAT_MOUSE_WHEELKEY = Input.GetMouseButton(2);
        STAT_MOUSE_ANYKEY = (STAT_MOUSE_LEFTKEY || STAT_MOUSE_RIGHTKEY || STAT_MOUSE_WHEELKEY);

        // ⭐ 유약칠 중이면 회전 금지
        if (PotteryPainter.touchOnPottery)
            return;


        // ---------------------------
        // 드래그 중
        // ---------------------------
        if (STAT_MOUSE_LEFTKEY)
        {
            if (!STAT_MOUSE_LEFTKEY_PREV)
            {
                drag_baseAngles = obj_pottery.transform.eulerAngles;
                drag_startMouseX = Input.mousePosition.x;
                drag_latestFrameMouseX = Input.mousePosition.x;

                simulatedAngularVelocity = 0f; // 관성 초기화
            }

            drag_xDistInScreen = (Input.mousePosition.x - drag_startMouseX);

            // 프레임 간 속도 → 관성에 사용될 값
            drag_angleVelocity = -((Input.mousePosition.x - drag_latestFrameMouseX) * drag_rotationScale);

            // Rigidbody 없이 직접 회전
            obj_pottery.transform.eulerAngles =
                new Vector3(
                    drag_baseAngles.x,
                    drag_baseAngles.y - (drag_xDistInScreen * drag_rotationScale),
                    drag_baseAngles.z
                );

            drag_latestFrameMouseX = Input.mousePosition.x;
        }

        // ---------------------------
        // 드래그 끝 → 관성 회전 시작
        // ---------------------------
        else if (STAT_MOUSE_LEFTKEY_PREV)
        {
            if (PRINT_INFO_DEBUG) { Debug.Log("velocity: " + drag_angleVelocity); }

            simulatedAngularVelocity = drag_angleVelocity;
        }

        // ---------------------------
        // 관성 회전 지속 부분
        // ---------------------------
        else
        {
            obj_pottery.transform.Rotate(0f, simulatedAngularVelocity, 0f, Space.World);

            // 감속 (공기저항)
            simulatedAngularVelocity *= multiplier_airResistance;
        }


        STAT_MOUSE_LEFTKEY_PREV = STAT_MOUSE_LEFTKEY;
    }
}
