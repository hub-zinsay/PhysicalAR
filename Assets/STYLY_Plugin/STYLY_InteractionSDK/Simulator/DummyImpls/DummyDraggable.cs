using System;
using System.Collections;
using System.Collections.Generic;
using STYLY.Interaction.SDK.V1;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace STYLY.Interaction.SDK.Dummy
{
    /// <summary>
    /// Draggableダミー実装のImplSetupper。
    /// </summary>
    public class DummyDraggableSetupper : IImplSetupper
    {
        public void Setup(Component srcComponent)
        {
            var ifComp = srcComponent as IDraggable;
            var impl = srcComponent.gameObject.AddComponent<DummyDraggable>();
            impl.OnBeginDragging = ifComp.OnBeginDragging;
            impl.OnEndDragging = ifComp.OnEndDragging;
        }
    }
    
    /// <summary>
    /// Draggableダミー実装。
    /// </summary>
    [AddComponentMenu("Scripts/DummyScript")]
    public class DummyDraggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public UnityEvent OnBeginDragging;
        public UnityEvent OnEndDragging;
        private Camera mainCamera;
        private Rigidbody mRigidbody;

        private void Start()
        {
            mainCamera = Camera.main;
            mRigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (isDragging)
            {
                MovePosition();
            }
        }

        [SerializeField]
        private bool isDragging;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            distanceFromCamera = Vector3.Distance(mainCamera.transform.position, this.transform.position);
            OnBeginDragging.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
            OnEndDragging.Invoke();
        }

        private const float duration = 0.1f;
        private float distanceFromCamera;
        private Vector3 moveTo;
        private void MovePosition() 
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = distanceFromCamera;

            moveTo = mainCamera.ScreenToWorldPoint(mousePos);

            if (mRigidbody && mRigidbody.isKinematic == false)
            {
                SetRigidbodyPosition(mRigidbody, moveTo, duration);
                SetRigidbodyRotation(mRigidbody);
            }
            else
            {
                transform.position = moveTo;
                transform.rotation = mainCamera.transform.rotation;
            }
        }

        void SetRigidbodyPosition(Rigidbody rb, Vector3 moveTo, float duration)
        {
            var diffPos = moveTo - gameObject.transform.position;
            if (Mathf.Approximately(diffPos.sqrMagnitude, 0f))
            {
                rb.velocity = Vector3.zero;
            }
            else
            {
                rb.velocity = diffPos / duration;
            }
        }

        void SetRigidbodyRotation(Rigidbody rb)
        {
            rb.angularVelocity = Vector3.zero;
        }
    }
}