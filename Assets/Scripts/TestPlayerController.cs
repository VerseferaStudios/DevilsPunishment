using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerController : MonoBehaviour
{

    private float journeyLength = 1f;
    private float speed = 25f;

    private float yRotation, xRotation;
    private float currentXRotation;
    private float lookSensitivity = 5f;
    private float xRotationV;
    private float lookSmoothDamp = 0.1f;
    private float currentYRotation;
    private float yRotationV;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
        xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        currentXRotation = Mathf.SmoothDamp(currentXRotation, xRotation, ref xRotationV, lookSmoothDamp);
        currentYRotation = Mathf.SmoothDamp(currentYRotation, yRotation, ref yRotationV, lookSmoothDamp);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            //Debug.Log("W");
            //playerRigidbody.velocity = new Vector3(0, 0, 2.5f);

            // Set our position as a fraction of the distance between the markers.
            Vector3 targetPos = transform.position + ((transform.forward - transform.right) * journeyLength);
            targetPos.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            //Debug.Log("W");
            //playerRigidbody.velocity = new Vector3(0, 0, 2.5f);

            // Set our position as a fraction of the distance between the markers.
            Vector3 targetPos = transform.position + ((transform.forward + transform.right) * journeyLength);
            targetPos.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            //Debug.Log("W");
            //playerRigidbody.velocity = new Vector3(0, 0, 2.5f);

            // Set our position as a fraction of the distance between the markers.
            Vector3 targetPos = transform.position + ((-transform.forward - transform.right) * journeyLength);
            targetPos.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            //Debug.Log("W");
            //playerRigidbody.velocity = new Vector3(0, 0, 2.5f);

            // Set our position as a fraction of the distance between the markers.
            Vector3 targetPos = transform.position + ((-transform.forward + transform.right) * journeyLength);
            targetPos.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
        }

        else if (Input.GetKey(KeyCode.W))
        {
            //Debug.Log("W");
            //playerRigidbody.velocity = new Vector3(0, 0, 2.5f);

            // Set our position as a fraction of the distance between the markers.
            Vector3 targetPos = transform.position + (transform.forward * journeyLength);
            targetPos.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            //Debug.Log("S");
            //playerRigidbody.velocity = new Vector3(0, 0, -2.5f);

            // Set our position as a fraction of the distance between the markers.
            Vector3 targetPos = transform.position + (transform.forward * -journeyLength);
            targetPos.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            //Debug.Log("A");
            //playerRigidbody.velocity = new Vector3(-2.5f, 0, 0);

            // Set our position as a fraction of the distance between the markers.
            Vector3 targetPos = transform.position + (transform.right * -journeyLength);
            targetPos.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //Debug.Log("D");
            //playerRigidbody.velocity = new Vector3(2.5f, 0, 0);

            // Set our position as a fraction of the distance between the markers.
            Vector3 targetPos = transform.position + (transform.right * journeyLength);
            targetPos.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);

        }
    }
}
