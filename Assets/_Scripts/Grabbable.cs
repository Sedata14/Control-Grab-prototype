using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class Grabbable : MonoBehaviour
{
    private Rigidbody rb;

    private Outline outline;

    public bool isGrabbed;

    private Transform Hand;

    public bool CanBeGrabbed = true;


    private void Start()
    {
        outline = GetComponent<Outline>();
        rb = GetComponent<Rigidbody>();

    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    public IEnumerator MoveToPlayer(Transform destination)
    {
        rb.useGravity = false;
        rb.AddForce(transform.up * 10f,ForceMode.Impulse);

        yield return new WaitForSeconds(0.3f);
        Hand = destination;
        isGrabbed = true;
        
        rb.linearDamping = 7f;
        rb.excludeLayers = LayerMask.NameToLayer("Player");
    }

    public void Update()
    {
        if (isGrabbed )
        {
            Vector3 handDir = Hand.position - transform.position;
            if(handDir.magnitude > 0.5)
            {
                rb.AddForce(handDir *20f, ForceMode.Force);
            }
        }
    }

    public void ThrowToTarget(Transform destination)
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.linearDamping = 0f;
        rb.linearVelocity = Vector3.zero;
        CanBeGrabbed = false;
        rb.useGravity = true;


        Vector3 HorizonPoint = destination.position + (destination.forward * 50f);

        Vector3 Dir = (HorizonPoint - transform.position).normalized;

        rb.AddForce(Dir * 50f,ForceMode.Impulse);

        rb.excludeLayers = 0;
        isGrabbed =false;
    }

}
