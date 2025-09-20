using AK.Wwise; // Necesario para Wwise
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace HurricaneVR.TechDemo.Scripts
{
    [RequireComponent(typeof(DemoPassthroughSocket))]
    public class Wwise_DemoLock : MonoBehaviour
    {
        public DemoPassthroughSocket Socket;
        public HVRGrabbable FaceGrabbable;
        public GameObject Face;
        public Transform Key;
        public float AnimationTime = 1f;

        // Reemplazar AudioClip con AK.Wwise.Event
        public AK.Wwise.Event SFXUnlockedWwiseEvent;
        public AK.Wwise.Event SFXKeyInsertedWwiseEvent;

        public float LockThreshold = 89f;

        public UnityEvent Unlocked = new UnityEvent();

        private bool _unlocked;

        public void Start()
        {
            Socket = GetComponent<DemoPassthroughSocket>();
            Socket.Grabbed.AddListener(OnKeyGrabbed);
        }

        public void Update()
        {
            if (!_unlocked && FaceGrabbable.transform.localRotation.eulerAngles.x > LockThreshold)
            {
                _unlocked = true;
                Unlocked.Invoke();
                Debug.Log($"lock unlocked!");
                FaceGrabbable.ForceRelease();
                FaceGrabbable.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                FaceGrabbable.CanBeGrabbed = false;
                FaceGrabbable.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

                // Llamar al evento de Wwise cuando el candado se desbloquea
                if (SFXUnlockedWwiseEvent != null)
                {
                    SFXUnlockedWwiseEvent.Post(gameObject);
                }
            }
        }

        private void OnKeyGrabbed(HVRGrabberBase grabber, HVRGrabbable key)
        {
            StartCoroutine(MoveKey(key));
        }

        private IEnumerator MoveKey(HVRGrabbable key)
        {
            var start = key.transform.position;
            var startRot = key.transform.rotation;

            var elapsed = 0f;
            while (elapsed < AnimationTime)
            {
                key.transform.position = Vector3.Lerp(start, Key.position, elapsed / AnimationTime);
                key.transform.rotation = Quaternion.Lerp(startRot, Key.rotation, elapsed / AnimationTime);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Llamar al evento de Wwise cuando la llave se inserta
            if (SFXKeyInsertedWwiseEvent != null)
            {
                SFXKeyInsertedWwiseEvent.Post(gameObject);
            }

            FaceGrabbable.gameObject.SetActive(true);
            Face.SetActive(false);
            Destroy(key.gameObject);
        }
    }
}