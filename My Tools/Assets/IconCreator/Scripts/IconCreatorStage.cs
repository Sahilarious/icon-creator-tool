using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SahilariousGames.IconCreator
{
    public class IconCreatorStage : MonoBehaviour
    {
        public Camera Camera { get { return m_camera; } }

        public Transform Pivot { get { return m_pivot; } }

        public Light LightOne { get { return m_lightOne; } }

        public Light LightTwo { get { return m_lightTwo; } }

        [SerializeField]
        private Camera m_camera;

        [SerializeField]
        private Transform m_pivot;

        [SerializeField]
        private Light m_lightOne;

        [SerializeField]
        private Light m_lightTwo;
    }
}