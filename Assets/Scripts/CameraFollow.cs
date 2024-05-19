using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public int speed;

    private Transform target;

    private void Start() {
        target = FindObjectOfType<Viewer>().GetComponent<Transform>();
    }

    private void LateUpdate() {
        transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, transform.position.y, target.position.z), speed * Time.deltaTime);
    }
}
