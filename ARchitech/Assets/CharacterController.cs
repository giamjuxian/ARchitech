using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private Camera ARCamera;
    float moveSpeed = 4f;
    Vector3 forward, right;
    public bool onGround;
    private Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        onGround = true;
        rb = GetComponent<Rigidbody>();
        forward = ARCamera.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
    }

    // Update is called once per frame
    void Update()
    {
        if (onGround)
        {
            if (CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector3(0, 7f, 0);
                onGround = false;
            }
        }
        float x = CrossPlatformInputManager.GetAxis("Horizontal");
        float y = CrossPlatformInputManager.GetAxis("Vertical");

        Vector3 direction = new Vector3(x, 0, y);
        Vector3 rightMovement = right * moveSpeed * Time.deltaTime * x;
        Vector3 upMovement = forward * moveSpeed * Time.deltaTime * y;

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;

        //if (x != 0 && y != 0) {
        //    transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Atan2(x, y) * Mathf.Rad2Deg, transform.eulerAngles.z);
        //}
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }
    }
}
