using UnityEngine;
using DG.Tweening;
using System;

namespace Utilities.TimeControl
{
    public static class TimeController
    {
        static Tween timeTween;
        static readonly float baseFixedDeltaTime = Time.fixedDeltaTime;
        public static bool IsPaused => Time.timeScale == 0f;
        public static event Action<float> OnTimeScaleChanged;

        public static void Set(float scale)
        {
            Time.timeScale = scale;
            Time.fixedDeltaTime = baseFixedDeltaTime * scale;

            OnTimeScaleChanged?.Invoke(scale);
        }

        public static void Pause()
        {
            StopTween();
            Set(0f);
        }

        public static void Resume()
        {
            StopTween();
            Set(1f);
        }

        public static void PauseSmooth(float duration = 0.25f, Ease ease = Ease.OutQuad)
        {
            TweenTimeScale(0f, duration, ease);
        }

        public static void ResumeSmooth(float duration = 0.25f, Ease ease = Ease.OutQuad)
        {
            TweenTimeScale(1f, duration, ease);
        }

        public static void EnterSlowMo(float targetScale = 0.2f, float duration = 0.25f, Ease ease = Ease.OutQuad)
        {
            TweenTimeScale(targetScale, duration, ease);
        }

        public static void ExitSlowMo(float targetScale = 1f, float duration = 0.25f, Ease ease = Ease.OutQuad)
        {
            TweenTimeScale(targetScale, duration, ease);
        }

        public static void TweenTimeScale(float targetScale, float duration, Ease ease)
        {
            StopTween();

            timeTween = DOTween
                .To(() => Time.timeScale, Set, targetScale, duration)
                .SetEase(ease)
                .SetUpdate(true);
        }

        public static void FreezeFrame(float duration, float resumeScale)
        {
            StopTween();

            Physics.simulationMode = SimulationMode.Script;

            Set(0f);

            DOVirtual.DelayedCall(duration, () =>
            {
                Physics.simulationMode = SimulationMode.FixedUpdate;
                Set(resumeScale);
            }).SetUpdate(true);
        }

        public static bool IsTweening()
        {
            return timeTween != null && timeTween.IsActive() && timeTween.IsPlaying();
        }

        public static void StopTween()
        {
            if (timeTween != null && timeTween.IsActive())
                timeTween.Kill();
        }
    }
}