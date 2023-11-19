using System;
using System.Collections;
using System.Collections.Generic;
using STYLY.Interaction.SDK.V1;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.Dummy
{
    /// <summary>
    /// Moverダミー実装のImplSetupper。
    /// </summary>
    public class DummyMoverSetupper : IImplSetupper
    {
        public void Setup(Component srcComponent)
        {
            var ifComp = srcComponent as IMover;
            var impl = srcComponent.gameObject.AddComponent<DummyMover>();
            impl.Target = ifComp.Target;
            impl.Time = ifComp.Time;
            impl.OnComplete = ifComp.OnComplete;
            ifComp.StartMoveListener += impl.Move;
        }
    }
    
    /// <summary>
    /// Moverダミー実装。
    /// </summary>
    [AddComponentMenu("Scripts/DummyScript")]
    public class DummyMover : MonoBehaviour, IMover
    {
        [SerializeField] private Transform target;
        [SerializeField, Tooltip("Sec")]
        private float time;
        [SerializeField]
        private UnityEvent onComplete;
        
        public Transform Target { get => target; set => target = value; }
        public float Time { get => time; set => time = value; }

        private bool isMoving = false;

        private Vector3 startPosition;
        private Quaternion startRotation;
        private float distance;
        
        private float movingTime = 0;
        public void Move()
        {
            Debug.Log("DummyMover:StartMove ");
            distance = Vector3.Distance(gameObject.transform.position, target.position);
            startPosition = gameObject.transform.position;
            startRotation = gameObject.transform.rotation;
            movingTime = 0;
            isMoving = true;
        }

        public UnityAction StartMoveListener { get; set; }
        public UnityEvent OnComplete { get => onComplete; set => onComplete = value; }

        
        private void Update()
        {
            if (isMoving)
            {
                movingTime += UnityEngine.Time.deltaTime;

                var movePer =  movingTime / time ;    
                gameObject.transform.position = Vector3.Lerp(startPosition, target.transform.position, movePer);
                gameObject.transform.rotation = Quaternion.Lerp(startRotation, target.transform.rotation, movePer);
                
                if (movingTime > time)
                {
                    isMoving = false;
                    var rb = gameObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.velocity = Vector3.zero;
                    }
                    
                    onComplete.Invoke();
                }

            }
            
        }
    }

    
}
