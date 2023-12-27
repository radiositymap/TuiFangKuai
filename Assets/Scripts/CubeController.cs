using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public float force = 3f;
    public Action OnGoalReached;
    Rigidbody rbd;
    Collider collider;
    ParticleSystem explosion;
    float x;
    float z;
    Queue<Vector2> motionQueue;

    void Awake()
    {
        rbd = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        explosion = GetComponentInChildren<ParticleSystem>();
        motionQueue = new Queue<Vector2>();
    }

    void Update()
    {
        if (GameMgr.currMode != GameMgr.GameMode.PlayMode)
            return;

        if (rbd.velocity != Vector3.zero)
            return;

        if (motionQueue.Count > 0) {
            Vector2 motion = motionQueue.Dequeue();
            x = motion.x;
            z = motion.y;
        }
        else {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
        }
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
        if (GameMgr.currMode != GameMgr.GameMode.PlayMode)
            return;

        // reposition
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x - 0.5f) + 0.5f;
        pos.z = Mathf.Round(pos.z - 0.5f) + 0.5f;
        transform.position = pos;
    }

    void OnTriggerEnter(Collider collider) {
        if (GameMgr.currMode != GameMgr.GameMode.PlayMode)
            return;

        if (collider.gameObject.CompareTag("Goal")) {
            // explode
            rbd.velocity = Vector3.zero;
            gameObject.GetComponent<Renderer>().enabled = false;
            explosion.Play();
            if (OnGoalReached != null)
                OnGoalReached();
        }
    }

    public void StartPlayMode() {
        collider.isTrigger = false;
    }

    public void SimulateMotion(float xMotion, float zMotion) {
        motionQueue.Enqueue(new Vector2(xMotion, zMotion));
    }
}