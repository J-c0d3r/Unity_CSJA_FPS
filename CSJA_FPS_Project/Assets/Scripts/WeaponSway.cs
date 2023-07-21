using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{

    public float minAmount;
    public float maxAmount;
    public float smoothAmount;

    private Vector3 initialPos;

    void Start()
    {
        initialPos = transform.localPosition;
    }


    void Update()
    {
        float moveX = -Input.GetAxis("Mouse X") * minAmount;
        float moveY = -Input.GetAxis("Mouse Y") * minAmount;

        moveX = Mathf.Clamp(moveX, -maxAmount, maxAmount);
        moveY = Mathf.Clamp(moveY, -maxAmount, maxAmount);

        Vector3 finalPos = new Vector3(moveX, moveY, 0f);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPos + initialPos, Time.deltaTime * smoothAmount);
    }
}
