using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;

namespace StarterAssets
{
    [RequireComponent(typeof(Animator))]
    public class GOWController : MonoBehaviour
    {
        private Animator _animator;
        private Rigidbody _weaponRb;
        private float _returnTime;
        private ThirdPersonController _thirdPersonController;
        private WeaponHandler _weaponHandler;
        private StarterAssetsInputs _input;

        private Vector3 _axePosition;
        private Vector3 _axeRotation;
        private Vector3 _pullPosition;

        [Header("Public References")]
        public Transform weapon;
        public Transform hand;
        public Transform spine;
        public Transform curvePoint;
        public CinemachineImpulseSource impulseFollowSource;
        public CinemachineImpulseSource impulseAimSource;
        public ParticleSystem glowParticle;
        public ParticleSystem catchParticle;
        public ParticleSystem trailParticle;
        public TrailRenderer trailRenderer;

        [Space]
        [Header("Parameters")]
        public float throwPower = 40f;

        [Space]
        [Header("Bools")] 
        public bool walking = true;
        public bool aiming = false;
        public bool hasWeapon = true;
        public bool pulling = false;

        [Space]
        [Header("UI")]
        public Image reticle;

        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponent<Animator>();
            _weaponRb = weapon.GetComponent<Rigidbody>();
            _weaponHandler = weapon.GetComponent<WeaponHandler>();
            _axePosition = weapon.localPosition;
            _axeRotation = weapon.localEulerAngles;
            _thirdPersonController = GetComponent<ThirdPersonController>();
            _input = GetComponent<StarterAssetsInputs>();
            reticle.DOFade(0, 0);
        }

        // Update is called once per frame
        void Update()
        {
            if (aiming)
            {
                _thirdPersonController.RotateToCamera();
            }

            _animator.SetBool("Pulling", pulling);
            walking = _thirdPersonController.speed > 0;

            if (_input.aim && hasWeapon)
            {
                Aim(true, true, 0f);
            }

            if (!_input.aim && hasWeapon)
            {
                Aim(false, true, 0f);
            }

            if (hasWeapon)
            {
                if (aiming && _input.throwing)
                {
                    _animator.SetTrigger("Throw");
                }
            } else {
                if (_input.throwing && !pulling)
                {
                    WeaponStartPull();
                }    
            }

            if (pulling)
            {
                if (_returnTime < 1f)
                {
                    weapon.position = GetQuadraticCurvePoint(_returnTime, _pullPosition, curvePoint.position, hand.position);
                    _returnTime += Time.deltaTime * 1f;
                } else
                {
                    WeaponCatch();
                }
            }
        }

        void Aim(bool state, bool changeCamera, float delay)
        {
            aiming = state;

            _animator.SetBool("Aiming", aiming);

            float fade = state ? 1f : 0f;
            reticle.DOFade(fade, 1f);

            if (!changeCamera)
            {
                return;
            }

            _thirdPersonController.ToggleCamera(state);

            if (state)
            {
                glowParticle.Play();
            } else
            {
                glowParticle.Stop();
            }
        }

        public void WeaponThrow()
        {
            Aim(false, true, 1f);
            hasWeapon = false;
            _weaponHandler.activated = true;
            _weaponRb.isKinematic = false;
            _weaponRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            weapon.parent = null;
            weapon.eulerAngles = new Vector3(0, -90 + transform.eulerAngles.y, 0);
            weapon.transform.position += transform.right / 5;
            _weaponRb.AddForce(Camera.main.transform.forward * throwPower + transform.up * 2, ForceMode.Impulse);

            trailRenderer.emitting = true;
            trailParticle.Play();
        }

        public void WeaponStartPull()
        {
            pulling = true;
            _pullPosition = weapon.position;
            _weaponRb.Sleep();
            _weaponRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _weaponRb.isKinematic = true;
            weapon.DORotate(new Vector3(-90, -90, 0), .2f).SetEase(Ease.InOutSine);
            weapon.DOBlendableLocalRotateBy(Vector3.right * 90, .5f);
            _weaponHandler.activated = true;
        }

        public void WeaponCatch()
        {
            _returnTime = 0;
            pulling = false;
            weapon.parent = hand;
            _weaponHandler.activated = false;
            weapon.localEulerAngles = _axeRotation;
            weapon.localPosition = _axePosition;
            hasWeapon = true;

            catchParticle.Play();
            trailRenderer.emitting = false;
            trailParticle.Stop();

            impulseAimSource.GenerateImpulse(Vector3.right);
            impulseFollowSource.GenerateImpulse(Vector3.right);
        }

        Vector3 GetQuadraticCurvePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            return (uu * p0) + (2 * u * t * p1) + (tt * p2);
        }
    }
}