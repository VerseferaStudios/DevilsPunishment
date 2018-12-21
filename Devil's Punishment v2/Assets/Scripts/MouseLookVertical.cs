using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookVertical : MonoBehaviour
{
    [Header("Sensitivity parameters")]
    [SerializeField]
    private float verticalSensitivity = 60f;

    [SerializeField]
    private float minVertical = -0.5f;

    [SerializeField]
    private float maxVertical = 0.5f;

    private new Camera camera;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        Quaternion verticalRotation = camera.transform.localRotation * Quaternion.Euler(Input.GetAxis("Mouse Y") * -verticalSensitivity * Time.deltaTime, 0f, 0f);
        verticalRotation.x = Mathf.Clamp(verticalRotation.x, minVertical , maxVertical);
        camera.transform.localRotation = verticalRotation;
    }
}
