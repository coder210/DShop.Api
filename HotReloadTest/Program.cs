using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;

// ==================== 配置 ====================
var baseUrl = args.Length > 0 ? args[0] : "http://192.168.2.40:9988";
var concurrency = args.Length > 1 ? int.Parse(args[1]) : 5;
var endpoint = $"{baseUrl}/api/admin/Login/Login";
// 改用表单数据
var loginData = new Dictionary<string, string>
{
    ["userName"] = "admin",
    ["password"] = "123456",
    ["captcha"] = "aaa"
};
// 测试持续时间（秒）
const int TestDurationSeconds = 120;
// =============================================

ServicePointManager.DefaultConnectionLimit = 100;

using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, _) =>
{
    Console.WriteLine("\n[停止] 正在结束测试...");
    cts.Cancel();
};

Console.WriteLine($"├─ 目标: {endpoint}");
Console.WriteLine($"├─ 并发数: {concurrency}");
Console.WriteLine($"├─ 测试时长: {TestDurationSeconds} 秒");
Console.WriteLine($"├─ 开始测试...");
Console.WriteLine($"├─ 请在测试过程中编译并复制新版 DLL 到 Plugin 目录，触发热更新");
Console.WriteLine($"└─ 按 Ctrl+C 提前停止");
Console.WriteLine();

var globalSeq = new AtomicLong(0);
var failedCount = 0;
var stopwatch = Stopwatch.StartNew();

// 启动 concurrency 个并发 Worker
var tasks = Enumerable.Range(0, concurrency)
    .Select(i => RunWorkerAsync(i, http, endpoint, loginData, globalSeq, cts.Token))
    .ToArray();

// ===== 自动停止定时器 =====
_ = Task.Run(async () =>
{
    try
    {
        await Task.Delay(TimeSpan.FromSeconds(TestDurationSeconds), cts.Token);
        if (!cts.IsCancellationRequested)
        {
            Console.WriteLine($"\n[⏱️ {TestDurationSeconds}秒已到] 自动停止测试...");
            cts.Cancel();
        }
    }
    catch (OperationCanceledException) { }
});

// 后台打印进度
_ = Task.Run(async () =>
{
    while (!cts.Token.IsCancellationRequested)
    {
        try
        {
            await Task.Delay(2000, cts.Token);
            var elapsed = stopwatch.Elapsed;
            var total = globalSeq.Read();
            var rate = total / elapsed.TotalSeconds;
            Console.WriteLine($"[{elapsed:mm\\:ss}] 已请求: {total}  |  速率: {rate:F0} req/s  |  失败: {failedCount}");
        }
        catch (OperationCanceledException) { break; }
    }
});

// 等待所有 Worker 退出
try { await Task.WhenAll(tasks); } catch (OperationCanceledException) { }

stopwatch.Stop();
var finalTotal = globalSeq.Read();
Console.WriteLine();
Console.WriteLine($"测试结束: 总请求 {finalTotal}, 失败 {failedCount}, 耗时 {stopwatch.Elapsed:mm\\:ss}");

if (failedCount > 0)
    Console.WriteLine("❌ 测试失败 — 存在丢失的请求！");
else
    Console.WriteLine("✅ 测试通过 — 所有请求均正常返回！");

// ==================== Worker ====================
async Task RunWorkerAsync(int id, HttpClient http, string url, Dictionary<string, string> data, AtomicLong seq, CancellationToken ct)
{
    // 每个 Worker 独立创建 content，避免重复使用（HttpClient 线程安全，但 content 不建议复用）
    using var content = new FormUrlEncodedContent(data);
    while (!ct.IsCancellationRequested)
    {
        try
        {
            var count = seq.Increment();
            using var response = await http.PostAsync(url, content, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                Interlocked.Increment(ref failedCount);
                Console.Error.WriteLine($"[Worker {id}] ❌ 序号 {count} 返回 {(int)response.StatusCode}: {body[..Math.Min(100, body.Length)]}");
            }
        }
        catch (TaskCanceledException) { break; }
        catch (Exception ex)
        {
            Interlocked.Increment(ref failedCount);
            Console.Error.WriteLine($"[Worker {id}] ❌ 异常: {ex.Message}");
        }
    }
}

// ==================== 原子计数器 ====================
class AtomicLong
{
    private long _value;
    public AtomicLong(long initial) => _value = initial;
    public long Increment() => Interlocked.Increment(ref _value);
    public long Read() => Interlocked.Read(ref _value);
}