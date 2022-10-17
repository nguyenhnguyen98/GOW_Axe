using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets { 
    public class WeaponHandler : MonoBehaviour
    {
        [Header("Public References")]
        public ParticleSystem hitParticle;
        public ParticleSystem hitDustParticle;

        [Space]
        [Header("Parameters")]
        public bool activated;
        public float rotationSpeed;

        private BoxCollider _collider;

        // Start is called before the first frame update
        void Start()
        {
            _collider = GetComponent<BoxCollider>();
        }

        // Update is called once per frame
        void Update()
        {
            if (activated)
            {
                transform.localEulerAngles += Vector3.forward * rotationSpeed * Time.deltaTime;
                _collider.enabled = true;
            } else
            {
                _collider.enabled = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == 11 || collision.gameObject.layer == 0)
            {
                GetComponent<Rigidbody>().Sleep();
                GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                GetComponent<Rigidbody>().isKinematic = true;
                activated = false;
                hitDustParticle.Play();
                hitParticle.Play();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Breakable"))
            {
                if (other.GetComponent<BreakableHandler>() != null) 
                {
                    other.GetComponent<BreakableHandler>().Break();
                }
            }
        }
    }
}
