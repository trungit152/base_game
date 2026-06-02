# Pooling

Object pool **generic, thuần C#** để tái sử dụng các đối tượng tham chiếu, giảm cấp phát/GC trong
gameplay. Engine-agnostic, test được headless. Không phụ thuộc gì ngoài `System`.

## Các type

| Type | Mục đích |
|------|----------|
| `IObjectPool<T>` | Hợp đồng pool: `Get()`, `Release(item)`, `Clear()` + đếm `CountInactive` / `CountActive` / `CountAll`. (`T : class`) |
| `ObjectPool<T>` | Cài đặt mặc định dựa trên `Stack<T>`, có callback vòng đời và giới hạn kích thước. |
| `IPoolable` | (Tuỳ chọn) Để đối tượng tự xử lý vòng đời: `OnGet()` / `OnRelease()` — pool tự gọi nếu phần tử hiện thực. |

## Hành vi

- **`Get()`** — lấy phần tử từ pool nếu còn; nếu rỗng thì tạo mới qua `factory`. Gọi `onGet` cho phần tử trả ra.
- **`Release(item)`** — gọi `onRelease`, rồi đẩy phần tử về pool để tái dùng. Nếu pool đã đạt `maxSize`
  thì gọi `onDestroy` và **bỏ** phần tử thừa (không giữ lại).
- **`Clear()`** — gọi `onDestroy` cho mọi phần tử đang nằm trong pool rồi dọn sạch.
- **`Prewarm(count)`** — tạo trước `count` phần tử và nạp vào pool (giảm giật khi dùng lần đầu).
- **Đếm:** `CountInactive` (đang trong pool), `CountActive` (đã `Get` chưa `Release`), `CountAll` (tổng đang quản lý).
- **`maxSize <= 0`** nghĩa là **không giới hạn**.

> Pool **không** tự phát hiện việc `Release` cùng một phần tử hai lần — đó là trách nhiệm của bên gọi
> (giữ tối giản, không tốn thêm cấp phát cho việc kiểm tra).

## Cách dùng

```csharp
var pool = new ObjectPool<Bullet>(
    factory:   () => new Bullet(),
    onGet:     b => b.Reset(),          // chuẩn bị trước khi dùng
    onRelease: b => b.Disable(),        // dọn dẹp khi trả về
    onDestroy: b => b.Dispose(),        // khi pool đầy hoặc Clear()
    maxSize:   64);

pool.Prewarm(16);

var bullet = pool.Get();   // tái dùng nếu có, ngược lại tạo mới
// ... dùng bullet ...
pool.Release(bullet);      // trả về để tái dùng

pool.Clear();              // huỷ toàn bộ phần tử đang chờ trong pool
```

## Tự xử lý vòng đời: `IPoolable`

Thay vì truyền callback ở chỗ tạo pool, một đối tượng *bạn sở hữu* có thể tự khai báo logic vòng đời
bằng cách hiện thực `IPoolable`:

```csharp
public sealed class Bullet : IPoolable
{
    public void OnGet()     { /* đặt lại trạng thái khi được lấy ra */ }
    public void OnRelease() { /* dọn dẹp khi trả về pool */ }
}

// Không cần callback — pool tự gọi OnGet/OnRelease
var pool = new ObjectPool<Bullet>(() => new Bullet());
```

- **Opt-in:** chỉ áp dụng cho type hiện thực interface. Type không hiện thực vẫn pool được như thường
  (qua callback) — generic **không** bị ràng buộc `where T : IPoolable`, nên pool được cả type sealed / bên thứ ba.
- **Kết hợp được:** nếu vừa hiện thực `IPoolable` vừa truyền callback, **interface chạy trước, callback chạy sau**.
- Pool kiểm tra `is IPoolable` mỗi lần Get/Release (chi phí không đáng kể).

## Ghi chú

- Dành cho **đối tượng logic** (command, node tìm kiếm, struct-wrapper, hạt dữ liệu…). Với pooling
  `GameObject`/`Component` của Unity, hãy bọc ở **tầng adapter Unity** (Unity 2021+ còn có sẵn
  `UnityEngine.Pool.ObjectPool<T>`); core cố tình không biết tới Unity.
- Đơn luồng, tất định — khớp với phần còn lại của core.
