using UnityEngine;
using TouchControlsKit;
public class FirstPersonController : MonoBehaviour
{
    bool binded;
    Transform myTransform, cameraTransform;
    CharacterController controller;
    float rotation;
    public bool prevGrounded, canRotate = true;
    [SerializeField]
    private float speed = 1f;
    public ItemManager bag;

    // Awake
    void Awake()
    {
        myTransform = transform;
        cameraTransform = Camera.main.transform;
        controller = GetComponent<CharacterController>();
    }

    // Update
    void Update()
    {
        if (canRotate)
        {
#if UNITY_ANDROID
            Vector2 look = TCKInput.GetAxis("Touchpad");
            PlayerRotation(look.x, look.y);
#endif

#if UNITY_STANDALONE || UNITY_EDITOR
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            PlayerRotation(mouseX / 2, mouseY / 2);
#endif
        }
    }

    // FixedUpdate
    void FixedUpdate()
    {
#if UNITY_ANDROID
        Vector2 move = TCKInput.GetAxis("Joystick");
#endif
#if UNITY_STANDALONE || UNITY_EDITOR
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        PlayerMovement(moveX / 10, moveY / 10);
#endif
        PlayerMovement(move.x, move.y);
    }

    // PlayerMovement
    private void PlayerMovement(float horizontal, float vertical)
    {
        bool grounded = controller.isGrounded;

        Vector3 moveDirection = myTransform.forward * vertical;
        moveDirection += myTransform.right * horizontal;

        moveDirection.y = -10f;

        if (grounded)
            moveDirection *= 7f;

        controller.Move(moveDirection * speed * Time.fixedDeltaTime);

        if (!prevGrounded && grounded)
            moveDirection.y = 0f;

        prevGrounded = grounded;
    }

    // PlayerRotation
    public void PlayerRotation(float horizontal, float vertical)
    {
        myTransform.Rotate(0f, horizontal * 12f, 0f);
        rotation += vertical * 12f;
        rotation = Mathf.Clamp(rotation, -60f, 60f);
        cameraTransform.localEulerAngles = new Vector3(-rotation, cameraTransform.localEulerAngles.y, 0f);
    }
};