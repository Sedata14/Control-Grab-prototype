using System.Collections;
using UnityEngine;

using DG.Tweening;
using UnityEngine.InputSystem;
public class CameraController : MonoBehaviour
{
    PlayActions actions;


    float AngleX = 0;
    float AngleY = 0;
    [SerializeField] Vector2 RotationDelta;

    [SerializeField] private float duration;


    public Camera cam;

    [SerializeField] private float MouseSensitivity = 5f;
    [SerializeField] private float GamepadSensitivity;

    private float Sensitivity;
    public void Awake()
    {
        actions = new();

        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnEnable()
    {
        actions.Movement.Enable();
        actions.Movement.Look.performed += Look;
    }

    private void OnDisable()
    {
        actions.Movement.Look.performed -= Look;

        actions.Movement.Disable();
    }

    private void Update()
    {
        RotationDelta = actions.Movement.Look.ReadValue<Vector2>();
        AngleX -= RotationDelta.y * Sensitivity * Time.deltaTime;
        AngleY += RotationDelta.x * Sensitivity * Time.deltaTime;


        transform.rotation = Quaternion.Euler(Mathf.Clamp(AngleX,-20f,45f), AngleY, 0);;



    }

    public void Look(InputAction.CallbackContext context)
    {
        if(context.control.device is Gamepad)
        {
            Sensitivity = GamepadSensitivity;
        }
        else if(context.control.device is Mouse)
        {
            Sensitivity = MouseSensitivity;
        }
    }

    public IEnumerator ShootShaking()
    {
        yield return cam.transform.DOLocalMoveX(-3f, 0.2f);

        cam.transform.DOLocalMoveX(0f, 0.2f);
    }
}
