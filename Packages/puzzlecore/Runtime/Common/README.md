# Common

Các primitive nền tảng dùng chung cho mọi module khác. Không phụ thuộc gì ngoài `System`.

## Các type

| Type | Mục đích |
|------|----------|
| `Result` | Kết quả thao tác có thể thành/bại **không** kèm giá trị. |
| `Result<T>` | Tương tự, nhưng mang `Value` khi thành công. Chuyển đổi ngầm từ `T`. |
| `IClock` | Trừu tượng thời gian (`Now`, `DeltaTime`) để core không bao giờ chạm `UnityEngine.Time`. |

## Vì sao dùng `Result` thay cho exception

Các kết quả được dự kiến, không phải ngoại lệ — action không hợp lệ, kho rỗng, guard từ chối transition
— được mô hình hoá thành dữ liệu, không ném exception. Chỗ gọi luôn tường minh và test headless luôn
tất định (không dùng try/catch làm luồng điều khiển).

```csharp
Result r = inventory.TryConsume("bomb");
if (r.IsFailure)
{
    Log(r.Error);   // vd "insufficient: bomb"
    return;
}

Result<int> parsed = ParseLevel(raw);
if (parsed.IsSuccess) Load(parsed.Value);
```

Exception vẫn được dùng cho **lỗi lập trình** (đối số null, id không tồn tại trong `Add`) — những thứ mà
một caller đúng đắn không bao giờ gây ra.

## `IClock`

Seam duy nhất cho thời gian. Unity cấp `UnityClock` (dựa trên `UnityEngine.Time`); test cấp một
`ManualClock` mà `Advance(dt)` được kiểm soát hoàn toàn. Bất cứ thứ gì trong core cần thời gian đều nhận
`IClock` qua constructor — không bao giờ gọi tĩnh vào engine.
