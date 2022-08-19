using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public float force = 3f;
    Rigidbody rbd;
    ParticleSystem explosion;

    void Start()
    {
        rbd = GetComponent<Rigidbody>();
        explosion = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rbd.velocity != Vector3.zero)
            return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if (x == 0 && z == 0)
            return;
        if (x > 0)
            rbd.velocity = new Vector3(force, 0, 0);
        else if (x < 0)
            rbd.velocity = new Vector3(-force, 0, 0);
        else if (z > 0)
            rbd.velocity = new Vector3(0, 0, force);
        else if (z < 0)
            rbd.velocity = new Vector3(0, 0, -force);

    }

    void OnCollisionEnter(Collision collision) {
        // reposition
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x - 0.5f) + 0.5f;
        pos.z = Mathf.Round(pos.z - 0.5f) + 0.5f;
        transform.position = pos;
    }

    void OnTriggerEnter(Collider collider) {
        Debug.Log("Entered collider" + collider.name);
        if (collider.gameObject.CompareTag("Goal")) {
            // explode
            rbd.velocity = Vector3.zero;
            gameObject.GetComponent<Renderer>().enabled = false;
            explosion.Play();
        }
    }
}