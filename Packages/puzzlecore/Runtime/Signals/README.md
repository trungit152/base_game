# Signals

Một hub **publish/subscribe** nhỏ gọn, đồng bộ. Cho phép puzzle session, flow state và booster phát sự
kiện mà không cần tham chiếu lẫn nhau hay tham chiếu tầng trình bày. Không phụ thuộc gì ngoài `System`.

## Các type

| Type | Mục đích |
|------|----------|
| `ISignal` | Marker cho mọi message. Sự kiện mới = một type `ISignal` mới. |
| `ISignalBus` | `Subscribe<T>` / `Unsubscribe<T>` / `Publish<T>`. |
| `SignalBus` | Cài đặt mặc định. |

## Hành vi (theo thiết kế)

- **Đồng bộ** — `Publish` gọi handler ngay, theo thứ tự đăng ký. Tất định, dễ test.
- **An toàn re-entrancy** — khi publish sẽ lặp trên một *snapshot* danh sách handler, nên handler có thể
  subscribe/unsubscribe trong lúc phát mà không làm hỏng vòng lặp.
- **Đơn luồng** — không khoá. Core đơn luồng; đừng publish từ background thread.

## Cách dùng

```csharp
var bus = new SignalBus();

void OnActivated(BoosterActivatedSignal s) => Console.WriteLine(s.BoosterId + " x" + s.RemainingCount);
bus.Subscribe<BoosterActivatedSignal>(OnActivated);

bus.Publish(new BoosterActivatedSignal("bomb", 2));   // OnActivated chạy ngay bây giờ

bus.Unsubscribe<BoosterActivatedSignal>(OnActivated); // nhớ gỡ (vd trong OnDisable/Exit)
```

## Mở rộng (Open/Closed)

Định nghĩa một sự kiện mới bằng cách viết một class hiện thực `ISignal` — bus không bao giờ phải sửa. Các
loại signal hiện có nằm cạnh module phát ra chúng: `Puzzle/Signals/`, `Flow/Signals/`, `Boosters/Signals/`.

## Ghi chú

Cố tình tối giản. Nếu về sau cần async, buffering hay toán tử reactive, một adapter ở tầng Unity có thể
bọc `ISignalBus` (vd bằng UniRx) — core vẫn không phụ thuộc gì.
