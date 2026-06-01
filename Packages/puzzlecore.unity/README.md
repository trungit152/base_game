# PuzzleCore Unity (`trungnhd.puzzlecore.Unity`)

Adapter Unity cho [`com.trungnhd.puzzlecore`](../com.trungnhd.puzzlecore). Nó chứa phần code **DUY NHẤT**
trong framework tham chiếu Unity hoặc DI container — mọi thứ còn lại vẫn engine-agnostic.

## Nội dung

| Type | Vai trò |
|------|---------|
| `Composition/GameLifetimeScope` | `LifetimeScope` của VContainer — composition root. Nơi duy nhất tham chiếu VContainer; wiring interface core → cài đặt. |
| `Time/UnityClock` | `IClock` dựa trên `UnityEngine.Time` — cầu nối duy nhất tới thời gian của Unity. |
| `Flow/GameFlowDriver` | Entry point VContainer (`IStartable`/`ITickable`) boot luồng game và tick state machine mỗi frame. |
| `Boosters/BoosterDefinitionAsset` | `ScriptableObject : IBoosterDefinition` (abstract) để tạo booster dạng asset. Tạo lớp con cho mỗi loại booster để cấp `IBoosterEffect`. |

## Phụ thuộc: VContainer

Package này phụ thuộc **VContainer** (`jp.hadashikick.vcontainer`), không đi kèm sẵn. Thêm qua scoped
registry OpenUPM trong `Packages/manifest.json`:

```json
{
  "scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": ["jp.hadashikick.vcontainer"]
    }
  ],
  "dependencies": {
    "jp.hadashikick.vcontainer": "1.16.9"
  }
}
```

(Hoặc cài bằng `.unitypackage` / git URL của VContainer — cách nào cũng được, miễn assembly `VContainer`
hiển diện cho `trungnhd.puzzlecore.Unity.asmdef`.)

## Cách dùng

1. Gắn component `GameLifetimeScope` vào một GameObject trong scene.
2. Tạo các lớp con `BoosterDefinitionAsset` cụ thể cho booster của bạn và kéo chúng vào list
   `Booster Definitions` của scope.
3. Bấm Play — `GameFlowDriver` vào `BootState` và tick machine. Subscribe các signal
   (`StateEnteredSignal`, `BoosterActivatedSignal`, …) từ UI để phản ứng.

## Vì sao VContainer chỉ nằm ở đây

Core dùng constructor injection trên interface thuần nên không hề biết container. Scope này có thể thay
thế: harness `dev/Headless` dựng đúng object graph đó bằng `new`, chính là bằng chứng framework không
phụ thuộc container.
