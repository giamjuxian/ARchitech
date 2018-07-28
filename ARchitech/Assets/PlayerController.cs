using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // ATTRIBUTES
    [SerializeField]
    private float moveSpeed = 4f;

    Vector3 forward, right;
    public bool onGround;
    private Rigidbody rb;
    private Animator anim;

    // Use this for initialization
    void Start()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
        onGround = true;
        rb = GetComponent<Rigidbody>();
        forward = Camera.main.transform.forward;
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
                rb.velocity = new Vector3(0, 5f, 0);
                onGround = false;
            }
        }
        float x = CrossPlatformInputManager.GetAxis("Horizontal");
        float y = CrossPlatformInputManager.GetAxis("Vertical");

        if (x != 0 || y != 0)
        {
            anim.SetInteger("AnimationPar", 1);
        }
        else
        {
            anim.SetInteger("AnimationPar", 0);
        }

        Vector3 direction = new Vector3(x, 0, y);
        Vector3 rightMovement = right * moveSpeed * Time.deltaTime * x;
        Vector3 upMovement = forward * moveSpeed * Time.deltaTime * y;
        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);
        if (x != 0 || y != 0)
        {
            transform.forward = heading;
        }

        transform.position += rightMovement;
        transform.position += upMovement;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }
        if (collision.gameObject.CompareTag("Complete"))
        {
            GameObject[] img = GameObject.FindGameObjectsWithTag("CompleteImg");
            foreach (GameObject image in img)
            {
                Debug.Log("hello");
                Debug.Log(image);
                RawImage showImage = image.GetComponentInChildren<RawImage>();
                showImage.enabled = true;
            }
        }
    }
}
