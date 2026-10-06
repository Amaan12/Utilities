using UnityEngine;
using LitMotion;
using System;

namespace Utilities.TimeControl
{
    public static class TimeController
    {
        static MotionHandle timeTween;
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

            timeTween = LMotion.Create(Time.timeScale, targetScale, duration)
                .WithEase(ease)
                .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                .Bind(Set);
        }

        public static void FreezeFrame(float duration, float resumeScale)
        {
            StopTween();

            Physics.simulationMode = SimulationMode.Script;

            Set(0f);

            timeTween = LMotion.Create(0f, 0f, duration)
                .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                .WithOnComplete(() =>
                {
                    Physics.simulationMode = SimulationMode.FixedUpdate;
                    Set(resumeScale);
                })
                .RunWithoutBinding();
        }

        public static bool IsTweening()
        {
            return timeTween.IsActive();
        }

        public static void StopTween()
        {
            if (timeTween.IsActive())
                timeTween.Cancel();
        }
    }
}