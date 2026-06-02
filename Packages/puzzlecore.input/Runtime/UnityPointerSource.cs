// Adapter chỉ biên dịch khi backend Input System mới đang bật (cài com.unity.inputsystem +
// Active Input Handling = Input System/Both). Khi tắt, cả file rỗng nên project vẫn compile,
// và các bộ nhận diện C# thuần trong cùng package vẫn dùng/test headless bình thường.
#if ENABLE_INPUT_SYSTEM
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace trungnhd.puzzlecore.input
{
    /// <summary>
    /// Nguồn con trỏ dùng Unity Input System. Poll <c>Pointer.current</c> sau mỗi nhịp cập nhật input
    /// (<see cref="InputSystem.onAfterUpdate"/>) — gộp sẵn chuột + cảm ứng + pen. Phát Began khi nhấn,
    /// Moved MỖI FRAME khi đang nhấn (vừa cập nhật vị trí cho drag, vừa là nhịp thời gian cho hold), và
    /// Ended khi nhả. Mọi recognizer (swipe/tap/drag/hold) tiêu thụ chung dòng này.
    /// </summary>
    public sealed class UnityPointerSource : IPointerSource, IDisposable
    {
        private bool _wasPressed;

        public event Action<PointerSample> Pointer;

        public UnityPointerSource()
        {
            InputSystem.onAfterUpdate += OnAfterUpdate;
        }

        private void OnAfterUpdate()
        {
            var device = UnityEngine.InputSystem.Pointer.current;
            if (device == null)
            {
                return;
            }

            bool pressed = device.press.isPressed;
            Vector2 p = device.position.ReadValue();
            float time = Time.unscaledTime;

            if (pressed && !_wasPressed)
            {
                _wasPressed = true;
                Emit(p, PointerPhase.Began, time);
            }
            else if (pressed)
            {
                Emit(p, PointerPhase.Moved, time);
            }
            else if (_wasPressed)
            {
                _wasPressed = false;
                Emit(p, PointerPhase.Ended, time);
            }
        }

        private void Emit(Vector2 p, PointerPhase phase, float time)
            => Pointer?.Invoke(new PointerSample(p.x, p.y, phase, time));

        public void Dispose()
        {
            InputSystem.onAfterUpdate -= OnAfterUpdate;
        }
    }
}
#endif
