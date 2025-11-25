// dragToRotate.cs
// 2025-11-25 by XERONAME

using UnityEngine;



public class dragToRotate : MonoBehaviour
{
    static private bool PRINT_INFO_DEBUG = false; // if set to true, the debug information message will shown in Unity editor
    static private bool PRINT_WARN_DEBUG = true; // if set to true, the debug warning message will shown in Unity editor

    static private bool STAT_MOUSE_LEFTKEY, STAT_MOUSE_RIGHTKEY, STAT_MOUSE_WHEELKEY, STAT_MOUSE_ANYKEY;
    static private bool STAT_MOUSE_LEFTKEY_PREV = false;

    [SerializeField] private GameObject obj_pottery; // game-object of pottery
    [SerializeField] private Rigidbody rb_pottery; // rigid-body of pottery-object

    private float drag_startMouseX, drag_latestFrameMouseX, drag_xDistInScreen, drag_angleVelocity;
    private Vector3 drag_baseAngles;
    [SerializeField] private float drag_rotationScale = 1.0f; // rotation scale when dragging; affects to rotation-velocity
    [SerializeField] private float multiplier_airResistance = 0.99f; // air-resistance multiplier



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PRINT_WARN_DEBUG) {
            if (obj_pottery == null) { Debug.LogWarning("GameObject of pottery not found!"); } // write the  warn message, if object not assigned
            if (rb_pottery == null) { Debug.LogWarning("Rigidbody of pottery-object not found!"); } // write the warn message, if rigidbody not assigned
        }
    }



    // Update is called once per frame
    void Update()
    {
        STAT_MOUSE_LEFTKEY = Input.GetMouseButton(0);
        STAT_MOUSE_RIGHTKEY = Input.GetMouseButton(1);
        STAT_MOUSE_WHEELKEY = Input.GetMouseButton(2);
        STAT_MOUSE_ANYKEY = (STAT_MOUSE_LEFTKEY || STAT_MOUSE_RIGHTKEY || STAT_MOUSE_WHEELKEY);


        if (STAT_MOUSE_LEFTKEY) {
            if (!STAT_MOUSE_LEFTKEY_PREV) {
                drag_baseAngles = obj_pottery.transform.eulerAngles; // get euler-angle of pottery-object, to set base rotation angle
                drag_startMouseX = Input.mousePosition.x;
            }
            drag_xDistInScreen = (Input.mousePosition.x -drag_startMouseX);
            drag_angleVelocity = ( -((Input.mousePosition.x -drag_latestFrameMouseX) *drag_rotationScale) );
            rb_pottery.angularVelocity = new Vector3(0.0f, 0.0f, 0.0f); // stop angular-velocity of pottery

            // set angle of target object, by drag distance
            obj_pottery.transform.eulerAngles = new Vector3(drag_baseAngles.x, (drag_baseAngles.y -(drag_xDistInScreen *drag_rotationScale)), drag_baseAngles.z);

            drag_latestFrameMouseX = Input.mousePosition.x; // update the mouse-x location of recent-frame
        }

        else if (STAT_MOUSE_LEFTKEY_PREV) {
            if (PRINT_INFO_DEBUG) { Debug.Log("velocity: " +drag_angleVelocity); }

            // apply angular-velocity to rigidbody of pottery
            rb_pottery.angularVelocity = new Vector3(0.0f, drag_angleVelocity, 0.0f);
        }

        else {
            rb_pottery.angularVelocity = new Vector3(
                rb_pottery.angularVelocity.x,
                (rb_pottery.angularVelocity.y *multiplier_airResistance),
                rb_pottery.angularVelocity.z
            );
        }


        STAT_MOUSE_LEFTKEY_PREV = STAT_MOUSE_LEFTKEY; // update the previous status of key-down, to use at next-frame behaviour
    }
}
