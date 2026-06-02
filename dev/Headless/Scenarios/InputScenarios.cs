using System;
using trungnhd.puzzlecore.input;

namespace trungnhd.puzzlecore.Headless.Scenarios
{
    /// <summary>
    /// Kiểm chứng input core thuần trên một dòng con trỏ giả (không cần Unity): swipe (Resolve + stream),
    /// tap/click, drag, và hold/long-press.
    /// </summary>
    public static class InputScenarios
    {
        /// <summary>Nguồn con trỏ giả để bơm mẫu Began/Moved/Ended/Canceled (kèm mốc thời gian) trong test.</summary>
        private sealed class FakePointerSource : IPointerSource
        {
            public event Action<PointerSample> Pointer;

            public void Push(float x, float y, PointerPhase phase, float time = 0f, int id = 0)
                => Pointer?.Invoke(new PointerSample(x, y, phase, time, id));
        }

        public static void Run()
        {
            Console.WriteLine("[Input]");

            Swipe();
            Tap();
            Drag();
            Hold();
        }

        private static void Swipe()
        {
            // Phần toán thuần
            Asserts.Expect(SwipeRecognizer.Resolve(100f, 5f, 50f) == SwipeDirection.Right, "độ dời ngang dương -> Right");
            Asserts.Expect(SwipeRecognizer.Resolve(-100f, 5f, 50f) == SwipeDirection.Left, "độ dời ngang âm -> Left");
            Asserts.Expect(SwipeRecognizer.Resolve(5f, 100f, 50f) == SwipeDirection.Up, "độ dời dọc dương -> Up");
            Asserts.Expect(SwipeRecognizer.Resolve(5f, -100f, 50f) == SwipeDirection.Down, "độ dời dọc âm -> Down");
            Asserts.Expect(SwipeRecognizer.Resolve(10f, 10f, 50f) == null, "dưới ngưỡng -> null (tap)");
            Asserts.Expect(SwipeRecognizer.Resolve(40f, 40f, 50f) == SwipeDirection.Right, "hòa trục, trên ngưỡng -> ưu tiên ngang");

            var source = new FakePointerSource();
            var recognizer = new SwipeRecognizer(source, new SwipeConfig(50f));
            SwipeDirection? last = null;
            int count = 0;
            recognizer.Swiped += d => { last = d; count++; };

            source.Push(0f, 0f, PointerPhase.Began);
            source.Push(120f, 0f, PointerPhase.Ended);
            Asserts.Expect(count == 1 && last == SwipeDirection.Right, "Began->Ended sang phải -> phát Swipe Right");

            source.Push(0f, 0f, PointerPhase.Began);
            source.Push(10f, 0f, PointerPhase.Ended);
            Asserts.ExpectEqual(1, count, "vuốt ngắn dưới ngưỡng -> không phát");

            source.Push(0f, 0f, PointerPhase.Began);
            source.Push(0f, 0f, PointerPhase.Canceled);
            source.Push(200f, 0f, PointerPhase.Ended);
            Asserts.ExpectEqual(1, count, "bị Canceled thì Ended sau đó không tính");

            recognizer.Dispose();
            source.Push(0f, 0f, PointerPhase.Began);
            source.Push(0f, 200f, PointerPhase.Ended);
            Asserts.ExpectEqual(1, count, "sau Dispose không còn nhận pointer");
        }

        private static void Tap()
        {
            var src = new FakePointerSource();
            var tap = new TapRecognizer(src, new TapConfig(20f, 0.3f));
            int taps = 0;
            Point2 at = default;
            tap.Tapped += p => { taps++; at = p; };

            src.Push(100f, 100f, PointerPhase.Began, 0f);
            src.Push(102f, 101f, PointerPhase.Ended, 0.1f);
            Asserts.Expect(taps == 1 && at.X == 102f, "chạm-nhả nhanh tại chỗ -> Tap tại vị trí nhả");

            src.Push(0f, 0f, PointerPhase.Began, 1f);
            src.Push(100f, 0f, PointerPhase.Moved, 1.05f);
            src.Push(100f, 0f, PointerPhase.Ended, 1.1f);
            Asserts.ExpectEqual(1, taps, "dịch quá MaxDistance -> không Tap");

            src.Push(0f, 0f, PointerPhase.Began, 2f);
            src.Push(1f, 1f, PointerPhase.Ended, 2.5f);
            Asserts.ExpectEqual(1, taps, "giữ quá MaxDuration -> không Tap");
        }

        private static void Drag()
        {
            var src = new FakePointerSource();
            var drag = new DragRecognizer(src, new DragConfig(10f));
            int begins = 0, moves = 0, ends = 0;
            DragEvent lastEv = default;
            drag.Dragging += e =>
            {
                if (e.Phase == DragPhase.Begin) begins++;
                else if (e.Phase == DragPhase.Move) moves++;
                else ends++;
                lastEv = e;
            };

            src.Push(0f, 0f, PointerPhase.Began, 0f);
            src.Push(5f, 0f, PointerPhase.Moved, 0.05f);
            Asserts.ExpectEqual(0, begins, "dưới ngưỡng -> chưa Begin drag");

            src.Push(20f, 0f, PointerPhase.Moved, 0.1f);
            Asserts.ExpectEqual(1, begins, "vượt ngưỡng -> Begin drag");

            src.Push(30f, 0f, PointerPhase.Moved, 0.15f);
            Asserts.Expect(moves == 1 && lastEv.Delta.X == 10f, "Move kèm delta so với mẫu trước");

            src.Push(30f, 0f, PointerPhase.Ended, 0.2f);
            Asserts.ExpectEqual(1, ends, "nhả -> End drag");

            src.Push(0f, 0f, PointerPhase.Began, 1f);
            src.Push(3f, 0f, PointerPhase.Moved, 1.05f);
            src.Push(3f, 0f, PointerPhase.Ended, 1.1f);
            Asserts.Expect(begins == 1 && ends == 1, "chạm-nhả không vượt ngưỡng -> không sinh drag");
        }

        private static void Hold()
        {
            var src = new FakePointerSource();
            var hold = new HoldRecognizer(src, new HoldConfig(0.5f, 20f));
            int holds = 0;
            Point2 at = default;
            hold.Held += p => { holds++; at = p; };

            src.Push(50f, 50f, PointerPhase.Began, 0f);
            src.Push(50f, 50f, PointerPhase.Moved, 0.2f);
            Asserts.ExpectEqual(0, holds, "giữ chưa đủ lâu -> chưa Hold");

            src.Push(51f, 50f, PointerPhase.Moved, 0.6f);
            Asserts.Expect(holds == 1 && at.X == 50f, "giữ đủ lâu tại chỗ -> Hold (tại điểm chạm)");

            src.Push(51f, 50f, PointerPhase.Moved, 0.9f);
            Asserts.ExpectEqual(1, holds, "Hold chỉ phát 1 lần mỗi lần nhấn");
            src.Push(51f, 50f, PointerPhase.Ended, 1.0f);

            src.Push(0f, 0f, PointerPhase.Began, 2f);
            src.Push(100f, 0f, PointerPhase.Moved, 2.6f);
            Asserts.ExpectEqual(1, holds, "dịch quá MoveTolerance -> không Hold");
        }
    }
}
