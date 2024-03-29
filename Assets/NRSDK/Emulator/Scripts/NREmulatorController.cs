﻿/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/         
* 
*****************************************************************************/

namespace NRKernal
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class NREmulatorController : MonoBehaviour
    {
        public float HeadMoveSpeed = 1.0f; //regular speed
        public float HeadRotateSpeed = 1.0f; //How sensitive it with mouse

        public GameObject DefaultControllerPanel;
        public GameObject ImageDefault;
        public GameObject ImageApp;
        public GameObject ImageConfirm;
        public GameObject ImageHome;
        public GameObject ImageLeft;
        public GameObject ImageRight;
        public GameObject ImageUp;
        public GameObject ImageDown;

        private const int kWidth = 2;
        private const int kHeight = 2;

        private TouchActionState m_TouchAction;
        private int m_TouchActionCurFrame;
        private GameObject m_Target;

        enum TouchActionState
        {
            Idle,
            Left,
            Right,
            Up,
            Down,
        };

#if UNITY_EDITOR
        void Start()
        {
            DontDestroyOnLoad(this);
            m_Target = new GameObject("NREmulatorControllerTarget");
            m_Target.transform.rotation = Quaternion.identity;
            DontDestroyOnLoad(m_Target);
        }
#endif

#if UNITY_EDITOR
        void LateUpdate()
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                UpdateControllerRotateByInput();
            }

            if (NRInput.EmulateVirtualDisplayInEditor)
            {
                DefaultControllerPanel.SetActive(false);
                UpdateVirtualControllerButtons();
            }
            else
            {
                DefaultControllerPanel.SetActive(true);
                UpdateDefaultControllerButtons();
            }

            NREmulatorManager.Instance.NativeEmulatorApi.SetControllerSubmit();

            if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
            {
                NRDebugger.Log("Click down Trigger button !!!");
            }
            else if (NRInput.GetButtonDown(ControllerButton.APP))
            {
                NRDebugger.Log("Click down App button !!!");
            }
            else if (NRInput.GetButtonDown(ControllerButton.HOME))
            {
                NRDebugger.Log("Click down Home button !!!");
            }
        }

        private void UpdateDefaultControllerButtons()
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetConfirmButton(true);
                ImageConfirm.SetActive(true);
            }
            if (Input.GetMouseButtonUp(0))
            {
                SetConfirmButton(false);
                ImageConfirm.SetActive(false);
            }
            if (Input.GetMouseButtonDown(1))
            {
                SetAppButton(true);
                ImageApp.SetActive(true);
            }
            if (Input.GetMouseButtonUp(1))
            {
                SetAppButton(false);
                ImageApp.SetActive(false);
            }
            if (Input.GetMouseButtonDown(2))
            {
                SetHomeButton(true);
                ImageHome.SetActive(true);
            }
            if (Input.GetMouseButtonUp(2))
            {
                SetHomeButton(false);
                ImageHome.SetActive(false);
            }
            if (m_TouchAction != TouchActionState.Idle)
            {
                UpdateTouchAction();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    ImageLeft.SetActive(true);
                    m_TouchAction = TouchActionState.Left;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    ImageRight.SetActive(true);
                    m_TouchAction = TouchActionState.Right;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    ImageUp.SetActive(true);
                    m_TouchAction = TouchActionState.Up;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    ImageDown.SetActive(true);
                    m_TouchAction = TouchActionState.Down;
                }
                else if (Input.GetKeyUp(KeyCode.DownArrow)
                    | Input.GetKeyUp(KeyCode.UpArrow)
                    | Input.GetKeyUp(KeyCode.RightArrow)
                    | Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    NREmulatorManager.Instance.NativeEmulatorApi.SetControllerIsTouching(false);
                }
            }
        }

        private void UpdateVirtualControllerButtons()
        {
            NREmulatorManager.Instance.NativeEmulatorApi.SetControllerButtonState(0);
            NREmulatorManager.Instance.NativeEmulatorApi.SetControllerIsTouching(true);
            NREmulatorManager.Instance.NativeEmulatorApi.SetControllerTouchPoint(NRVirtualDisplayer.GetEmulatorScreenTouch().x, NRVirtualDisplayer.GetEmulatorScreenTouch().y);
        }
#endif

        private void UpdateTouchAction()
        {
            NREmulatorManager.Instance.NativeEmulatorApi.SetControllerIsTouching(true);
            const int kActionMaxFrame = 20;
            float touchx = 0;
            float touchy = 0;
            if (m_TouchAction == TouchActionState.Left)
            {
                touchy = kHeight / 2;
                touchx = 0.1f * kWidth + ((float)(kActionMaxFrame - m_TouchActionCurFrame) / kActionMaxFrame) * (0.8f * kWidth);
            }
            else if (m_TouchAction == TouchActionState.Right)
            {
                touchy = kHeight / 2;
                touchx = 0.1f * kWidth + ((float)m_TouchActionCurFrame / kActionMaxFrame) * (0.8f * kWidth);
            }
            else if (m_TouchAction == TouchActionState.Up)
            {
                touchx = kWidth / 2;
                touchy = 0.1f * kHeight + ((float)(kActionMaxFrame - m_TouchActionCurFrame) / kActionMaxFrame) * (0.8f * kHeight);
            }
            else if (m_TouchAction == TouchActionState.Down)
            {
                touchx = kWidth / 2;
                touchy = 0.1f * kHeight + ((float)m_TouchActionCurFrame / kActionMaxFrame) * (0.8f * kHeight);
            }

            NREmulatorManager.Instance.NativeEmulatorApi.SetControllerTouchPoint(touchx - 1f, touchy - 1f);

            if (m_TouchActionCurFrame == kActionMaxFrame)
            {
                m_TouchActionCurFrame = 0;
                m_TouchAction = TouchActionState.Idle;
                ImageLeft.SetActive(false);
                ImageRight.SetActive(false);
                ImageUp.SetActive(false);
                ImageDown.SetActive(false);
            }

            m_TouchActionCurFrame++;

        }

        private void UpdateControllerRotateByInput()
        {
            float mouse_x = Input.GetAxis("Mouse X") * HeadRotateSpeed;
            float mouse_y = Input.GetAxis("Mouse Y") * HeadRotateSpeed;

            Vector3 mouseMove = new Vector3(m_Target.transform.eulerAngles.x - mouse_y, m_Target.transform.eulerAngles.y + mouse_x, 0);
            Quaternion q = Quaternion.Euler(mouseMove);
            m_Target.transform.rotation = q;
            NREmulatorManager.Instance.NativeEmulatorApi.SetControllerRotation(new Quaternion(q.x, q.y, q.z, q.w));

        }

        public void SetAppButton(bool touch)
        {
            if (touch)
            {
                NREmulatorManager.Instance.NativeEmulatorApi.SetControllerTouchPoint(0f, -0.95f);
                NREmulatorManager.Instance.NativeEmulatorApi.SetControllerIsTouching(true);
                NREmulatorManager.Instance.NativeEmulatorApi.SetControllerButtonState(1);
            }
            else
            {
                NREmulatorManager.Instance.NativeEmulatorApi.SetControllerButtonState(0);
                NREmulatorManager.Instance.NativeEmulatorApi.SetControllerIsTouching(false);
            }
        }

        public void SetHomeButton(bool touch)
        {
            if (touch)
            {
                NREmulatorManager.Instance.NativeEmulatorApi.SetControllerTouchPoint(0f, 0.95f);
                NREmulatorManager.Instance.NativeEmulatorApi.SetControllerIsTouching(true);
                NREmulatorManager.Instance.NativeEmulatorApi.SetControllerButtonState(1);
            }
            else
            {
                NREmulatorManager.Instance.NativeEmulatorApi.SetControllerButtonState(0);
                NREmulatorManager.Instance.NativeEmulatorApi.SetControllerIsTouching(false);
            }
        }

        public void SetConfirmButton(bool touch)
        {
            if (touch)
            {
                NREmulatorManager.Instance.NativeEmulatorApi.SetControllerTouchPoint(0f, 0.01f);
                NREmulatorManager.Instance.NativeEmulatorApi.SetControllerIsTouching(true);
                NREmulatorManager.Instance.NativeEmulatorApi.SetControllerButtonState(1);
            }
            else
            {
                NREmulatorManager.Instance.NativeEmulatorApi.SetControllerButtonState(0);
                NREmulatorManager.Instance.NativeEmulatorApi.SetControllerIsTouching(false);
            }
        }

    }
}