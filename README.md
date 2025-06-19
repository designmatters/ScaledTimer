# ScaledTimer

A lightweight .NET library that implements a consumption-based timer. The timer advances based on a dynamic input percentage. 

If the scale remains at 100%, it behaves like a regular timer. When the scale fluctuates, this is taken into account. This is useful for tracking the consumption of a resource in terms of time — for example, battery usage time in an electric vehicle or RC plane.

## 🚀 Example

```csharp
var timer = new ScaledTimer.ScaledTimer(intervalMs: 3000, startScale: 50);
timer.Start();
timer.SetScale(73);
...
timer.SetScale(49)
...

[Scale changed → 73%]
→ Time: 00:00:00.00
→ Time: 00:00:00.37
[Scale changed → 49%]
→ Time: 00:00:00.73
→ Time: 00:00:00.98
[Scale changed → 89%]
→ Time: 00:00:01.23
→ Time: 00:00:01.68
[Scale changed → 65%]
→ Time: 00:00:02.12
→ Time: 00:00:02.45
[Scale changed → 39%]
→ Time: 00:00:02.77
→ Time: 00:00:02.97
[⏰ Elapsed] Scaled Time: 3.00s | Scale: 39% | Timestamp: 09:34:07.361
