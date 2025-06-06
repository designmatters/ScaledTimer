# ScaledTimer
A lightweight .NET library that implements a consumption-based timer. The timer advances based on a dynamic input percentage. 

var timer = new ScaledTimer.ScaledTimer(intervalMs: 3000, startScale: 50);
timer.Start()
timer.SetScale(73);

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
[⏰ Elapsed] Scaled Time: 3,00s | Scale: 39% | Timestamp: 09:34:07.361
