using System;
using DG.Tweening;
using UnityEngine;

namespace VertigoDemo.Data
{
    [Serializable]
    public sealed class PunchAnimationSettings
    {
        #region Fields

        [SerializeField, Min(0.01f)] private float _duration;
        [SerializeField, Min(0f)] private float _strength;
        [SerializeField, Min(1)] private int _vibrato;
        [SerializeField, Range(0f, 1f)] private float _elasticity;
        [SerializeField] private Ease _ease;

        #endregion

        #region Properties

        public float Duration
        {
            get
            {
                return _duration;
            }
        }

        public float Strength
        {
            get
            {
                return _strength;
            }
        }

        public int Vibrato
        {
            get
            {
                return _vibrato;
            }
        }

        public float Elasticity
        {
            get
            {
                return _elasticity;
            }
        }

        public Ease Ease
        {
            get
            {
                return _ease;
            }
        }

        #endregion

        #region Constructors

        public PunchAnimationSettings(
            float duration,
            float strength,
            int vibrato,
            float elasticity,
            Ease ease = Ease.OutQuad)
        {
            _duration = duration;
            _strength = strength;
            _vibrato = vibrato;
            _elasticity = elasticity;
            _ease = ease;

            Validate();
        }

        #endregion

        #region Public Methods

        public void Validate()
        {
            _duration = Mathf.Max(0.01f, _duration);
            _strength = Mathf.Max(0f, _strength);
            _vibrato = Mathf.Max(1, _vibrato);
            _elasticity = Mathf.Clamp01(_elasticity);
        }

        #endregion
    }
}
