using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STYLY.Interaction.SDK.Dummy
{
    /// <summary>
    /// シミュレーター用プレイヤーコントローラー
    /// </summary>
    public class DummyPlayerController : MonoBehaviour
    {
        [SerializeField] bool fixHeight = false;

        [SerializeField] float moveSpeed = 0.1f;

        [SerializeField] private float turnSpeed = 10.0f;


        private bool onMouseDown = false;
        private Vector3 startMousePosition;
        private float dragRotationSpeed = 0.2f;

        // Update is called once per frame
        void Update()
        {
            if (fixHeight)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    gameObject.transform.position +=
                        new Vector3(gameObject.transform.forward.x, 0f, gameObject.transform.forward.z).normalized *
                        moveSpeed * Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.S))
                {
                    gameObject.transform.position -=
                        new Vector3(gameObject.transform.forward.x, 0f, gameObject.transform.forward.z).normalized *
                        moveSpeed * Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.D))
                {
                    gameObject.transform.position +=
                        new Vector3(gameObject.transform.right.x, 0f, gameObject.transform.right.z).normalized *
                        moveSpeed *
                        Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.A))
                {
                    gameObject.transform.position -=
                        new Vector3(gameObject.transform.right.x, 0f, gameObject.transform.right.z).normalized *
                        moveSpeed *
                        Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.E))
                {
                    gameObject.transform.position += Vector3.up * moveSpeed * Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.Q))
                {
                    gameObject.transform.position -= Vector3.up * moveSpeed * Time.deltaTime;
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.W))
                {
                    gameObject.transform.position += gameObject.transform.forward * moveSpeed * Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.S))
                {
                    gameObject.transform.position -= gameObject.transform.forward * moveSpeed * Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.D))
                {
                    gameObject.transform.position += gameObject.transform.right * moveSpeed * Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.A))
                {
                    gameObject.transform.position -= gameObject.transform.right * moveSpeed * Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.E))
                {
                    gameObject.transform.position += gameObject.transform.up * moveSpeed * Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.Q))
                {
                    gameObject.transform.position -= gameObject.transform.up * moveSpeed * Time.deltaTime;
                }
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                gameObject.transform.eulerAngles += new Vector3(turnSpeed * Time.deltaTime, 0f, 0f);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                gameObject.transform.eulerAngles -= new Vector3(turnSpeed * Time.deltaTime, 0f, 0f);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                gameObject.transform.eulerAngles += new Vector3(0f, turnSpeed * Time.deltaTime, 0f);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                gameObject.transform.eulerAngles -= new Vector3(0f, turnSpeed * Time.deltaTime, 0f);
            }

            // マウス操作
            if (Input.GetMouseButtonDown(0))
            {
                onMouseDown = true;
                startMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                onMouseDown = false;
            }

            if (onMouseDown)
            {
                Vector3 mousePos = Input.mousePosition;

                var xDiff = mousePos.x - startMousePosition.x;
                var yDiff = mousePos.y - startMousePosition.y;
                gameObject.transform.eulerAngles = new Vector3(
                    gameObject.transform.eulerAngles.x - yDiff * dragRotationSpeed,
                    gameObject.transform.eulerAngles.y + xDiff * dragRotationSpeed, gameObject.transform.eulerAngles.z);
                startMousePosition = mousePos;
            }
        }

    }
}