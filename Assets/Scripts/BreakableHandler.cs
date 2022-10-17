using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace StarterAssets
{
    public class BreakableHandler : MonoBehaviour
    {
        public GameObject breakedBox;
        public float pieceSleepCheckDelay = 10f;
        public float pieceDestroyDelay = 5f;

        public void Break()
        {
            GameObject breaked = Instantiate(breakedBox, transform.position, transform.rotation);
            Rigidbody[] rbs = breaked.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbs)
            {
                rb.AddExplosionForce(250, transform.position, 30);
            }
            Destroy(gameObject);
        }
    }
}
