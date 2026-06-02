# PuzzleCore Input

`com.trungnhd.puzzlecore.input` — module input tuỳ chọn trong họ **PuzzleCore**, dành cho game Unity.

Tách rõ hai tầng:

- **Nhận diện cử chỉ (recognizer)** — C# **thuần**, tất định, không phụ thuộc engine → test được headless.
- **Bắt input (adapter)** — `UnityPointerSource` đọc Unity Input System, được bọc `#if ENABLE_INPUT_SYSTEM` nên khi chưa bật Input System (hoặc ở build headless) file rỗng, project vẫn biên dịch.

> Đây là **một package Unity duy nhất** (không tách pure/unity như core `puzzlecore`). Recognizer giữ thuần để test headless; phần Unity guard bằng `#if`. Quy ước cho mọi feature package về sau.

## Kiến trúc

```
Thiết bị ──► UnityPointerSource ──► IPointerSource ──► các Recognizer (thuần) ──► event gesture
 (chuột/                (Input System,        (dòng PointerSample:        Swipe / Tap / Drag / Hold
  cảm ứng/               #if guard)            Began/Moved/Ended/Canceled
  pen)                                         + Time)
```

- `IPointerSource` là **seam** duy nhất: phát `PointerSample` (vị trí pixel screen-space, pha, mốc thời gian).
- `UnityPointerSource` poll `Pointer.current` mỗi nhịp `InputSystem.onAfterUpdate`: phát `Began` khi nhấn, **`Moved` mỗi frame** khi đang nhấn (vừa cập nhật vị trí cho drag, vừa là *nhịp thời gian* cho hold), `Ended` khi nhả.
- Mỗi recognizer tiêu thụ chung dòng đó và **độc lập** với nhau — game tự chọn đăng ký cái nào.

## Các cử chỉ có sẵn

| Cử chỉ | Interface | Recognizer | Config (mặc định) | Phát ra |
|--------|-----------|------------|-------------------|---------|
| Swipe | `ISwipeInput` | `SwipeRecognizer` | `SwipeConfig` (min 50px) | `SwipeDirection` (Up/Down/Left/Right) |
| Tap/Click | `ITapInput` | `TapRecognizer` | `TapConfig` (20px, 0.3s) | `Point2` vị trí tap |
| Drag | `IDragInput` | `DragRecognizer` | `DragConfig` (10px) | `DragEvent` (Begin/Move/End + Delta) |
| Hold | `IHoldInput` | `HoldRecognizer` | `HoldConfig` (0.5s, 20px) | `Point2` điểm giữ |

> "Press/nhả thô" (nhấn xuống / nhả lên kèm vị trí) **không cần recognizer** — nghe thẳng `IPointerSource` (pha `Began`/`Ended`).

## Dùng trong Unity (VContainer)

```csharp
using trungnhd.puzzlecore.input;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameInputScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // 1 nguồn con trỏ nuôi tất cả recognizer
        builder.Register<IPointerSource, UnityPointerSource>(Lifetime.Singleton);

        float minSwipe = Mathf.Min(Screen.width, Screen.height) * 0.05f;
        builder.Register<ISwipeInput>(r =>
            new SwipeRecognizer(r.Resolve<IPointerSource>(), new SwipeConfig(minSwipe)), Lifetime.Singleton);

        builder.Register<ITapInput>(r =>
            new TapRecognizer(r.Resolve<IPointerSource>(), TapConfig.Default), Lifetime.Singleton);

        builder.Register<IDragInput>(r =>
            new DragRecognizer(r.Resolve<IPointerSource>(), DragConfig.Default), Lifetime.Singleton);

        builder.Register<IHoldInput>(r =>
            new HoldRecognizer(r.Resolve<IPointerSource>(), HoldConfig.Default), Lifetime.Singleton);
    }
}
```

Tiêu thụ:

```csharp
public class Handler : IStartable, IDisposable
{
    private readonly ISwipeInput _swipe;
    public Handler(ISwipeInput swipe) => _swipe = swipe;
    public void Start() => _swipe.Swiped += OnSwiped;
    public void Dispose() => _swipe.Swiped -= OnSwiped;
    private void OnSwiped(SwipeDirection dir) => Debug.Log($"Swipe {dir}");
}
```

> `UnityPointerSource`, `SwipeRecognizer`, … đều `IDisposable`; đăng ký Singleton để VContainer huỷ đúng lúc scope kết thúc.

## Yêu cầu

Để **bắt input thật** trong Editor/build: cài package **Input System** (`com.unity.inputsystem`) và bật backend (Player Settings → *Active Input Handling* = Input System hoặc Both). Khi đó `ENABLE_INPUT_SYSTEM` được định nghĩa và `UnityPointerSource` sống dậy.

Chưa cài cũng không sao: phần recognizer thuần vẫn biên dịch/dùng/test bình thường, chỉ `UnityPointerSource` tạm ngủ.

## Test headless

Recognizer là C# thuần nên kiểm chứng được ngoài Unity. Harness `dev/Headless` glob `Packages/puzzlecore.input/Runtime/**/*.cs` (file `UnityPointerSource` rỗng vì `#if` tắt) và chạy `InputScenarios` bằng một `IPointerSource` giả. Xem `dev/Headless/Scenarios/InputScenarios.cs`.

## Mở rộng

Thêm cử chỉ mới = **một recognizer thuần** mới tiêu thụ `IPointerSource`, kèm một interface `I…Input`. Không cần đụng `UnityPointerSource` (nó đã phát đủ Began/Moved/Ended + Time). Cần đo thời gian/giữ thì dùng `PointerSample.Time` và nhịp `Moved` theo frame (như `HoldRecognizer`).
