using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewer : MonoBehaviour
{
    public float speed;

    private void Update() {
        transform.position += Input.GetAxisRaw("Horizontal") * speed * Vector3.right * Time.deltaTime;
        transform.position += Input.GetAxisRaw("Vertical") * speed * Vector3.forward * Time.deltaTime;
    }
}
