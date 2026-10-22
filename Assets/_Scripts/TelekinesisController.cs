using System.Collections;
using UnityEngine;

public class TelekinesisController : MonoBehaviour
{
    [SerializeField] private LayerMask GrabbableLayerMask, PlayerLayerMask;
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform Hand;
    Grabbable LastSelectedGrabbable;

    private PlayActions actions;


    bool isGrabbing = false;
    private Transform CurrentHoldGrabbable;

    [SerializeField] private Camera cam;

    [SerializeField] private CameraController cameraController;
    private void Awake()
    {
        actions = new();

        cam = Camera.main;
    }

    private void OnEnable()
    {
        actions.Interaction.Enable();
        actions.Interaction.Grab.performed += ctx => {  isGrabbing = true; };
        actions.Interaction.Grab.canceled += ctx => isGrabbing = false;
    }
    private void OnDisable()
    {
        actions.Interaction.Disable();
    }
    public void FixedUpdate()
    {
        RaycastHit hit;


        
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100f, GrabbableLayerMask) && !CurrentHoldGrabbable)
        {
            if (LastSelectedGrabbable)
            {
                if (LastSelectedGrabbable != hit.transform.GetComponent<Grabbable>())
                {
                    Debug.Log("JE le connais celui-ci barre toi");
                    LastSelectedGrabbable.DisableOutline();
                    LastSelectedGrabbable = null;
                }
            }
            LastSelectedGrabbable = hit.transform.GetComponent<Grabbable>();
            LastSelectedGrabbable.EnableOutline();

            if (isGrabbing && !CurrentHoldGrabbable && LastSelectedGrabbable.GetComponent<Grabbable>().CanBeGrabbed) 
            {
                Debug.Log("Je vais grab celui-ci");
                isGrabbing = false;
                CurrentHoldGrabbable = LastSelectedGrabbable.transform;
                StartCoroutine(LastSelectedGrabbable.MoveToPlayer(Hand));

            }
        }
        else
        {
            if (LastSelectedGrabbable)
            {
     
                LastSelectedGrabbable.DisableOutline();
                LastSelectedGrabbable = null;
            }

            if (isGrabbing && CurrentHoldGrabbable)
            {


                StartCoroutine(ThrowGrabbable());

            }
        }


        Debug.DrawRay(cam.transform.position, cam.transform.forward * 15f, Color.green);


       
    }



    
    IEnumerator ThrowGrabbable()
    {
        yield return StartCoroutine(cameraController.ShootShaking());

        CurrentHoldGrabbable.GetComponent<Grabbable>().ThrowToTarget(cam.transform);
        CurrentHoldGrabbable = null;
        isGrabbing = false;
    }
    


}
