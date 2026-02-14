using System;
using System.Threading;

public static class ThreadUtils {
    public static void WaitOrThrow(this SemaphoreSlim semaphore, int ms, Action final) {
        try {
            if (!semaphore.Wait(ms)) {
                throw new TimeoutException("信号量等待超时！");
            }
        } finally {
            final?.Invoke();
        }
    }
}