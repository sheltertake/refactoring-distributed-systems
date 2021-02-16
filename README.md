# refactoring-distributed-systems

## No exception handling
 
 - 5733 requests (5380 + 389 timeout -> 5769)
 
 - 110 main errors
 - 5733 orders stored
   - 67 payments missed
   - 88 mails not sent
     - 21 caused by mailer error
     - 67 caused by payments error
   - 110 bus events missed
     - 22 caused by bus error
     - 21 caused by mailer error
     - 67 caused by payments error

 - 267.75 requests/sec    
 - 389 timeout

```json
{
    "counter": 5733,
    "counters": {
        "orders": 5733,
        "payments": 5666,
        "mails": 5645,
        "events": 5623
    },
    "duplicates": {
        "orders": 0,
        "payments": 0,
        "mails": 0,
        "events": 0
    },
    "errors": {
        "orders": 0,
        "payments": 67,
        "mails": 21,
        "events": 22
    },
    "requests": {
        "orders": 5733,
        "payments": 5733,
        "mails": 5666,
        "events": 5645
    },
    "counterErrors": 110
}
```

```cmd
Running 20s test @ http://app/Cart
  12 threads and 400 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency     1.32s   174.29ms   1.98s    70.57%
    Req/Sec    29.50     27.47   180.00     83.23%
  Latency Distribution
     50%    1.31s 
     75%    1.44s 
     90%    1.53s 
     99%    1.71s 
  5380 requests in 20.09s, 1.11MB read
  Socket errors: connect 0, read 0, write 0, timeout 389
  Non-2xx or 3xx responses: 107
Requests/sec:    267.75
Transfer/sec:     56.43KB
```

```cmd
app       | fail: app.Controllers.CartController[0]
app       |       Response status code does not indicate success: 500 (Internal Server Error).
app       |       System.Net.Http.HttpRequestException: Response status code does not indicate success: 500 (Internal Server Error).
app       |          at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
app       |          at app.Services.PayService.PostPaymentAsync(Order order) in /app/Services/PayService.cs:line 37
app       |          at app.Controllers.CartController.PostAsync(Cart model) in /app/Controllers/CartController.cs:line 58
app       | fail: app.Controllers.CartController[0]
app       |       Response status code does not indicate success: 500 (Internal Server Error).
app       |       System.Net.Http.HttpRequestException: Response status code does not indicate success: 500 (Internal Server Error).
app       |          at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
app       |          at app.Services.PayService.PostPaymentAsync(Order order) in /app/Services/PayService.cs:line 37
app       |          at app.Controllers.CartController.PostAsync(Cart model) in /app/Controllers/CartController.cs:line 58
app       | fail: app.Controllers.CartController[0]
app       |       Response status code does not indicate success: 500 (Internal Server Error).
app       |       System.Net.Http.HttpRequestException: Response status code does not indicate success: 500 (Internal Server Error).
app       |          at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
app       |          at app.Services.MailerService.SendPaymentSuccessEmailAsync(Order order) in /app/Services/MailerService.cs:line 30
app       |          at app.Controllers.CartController.PostAsync(Cart model) in /app/Controllers/CartController.cs:line 58
app       | fail: app.Controllers.CartController[0]
app       |       Response status code does not indicate success: 500 (Internal Server Error).
app       |       System.Net.Http.HttpRequestException: Response status code does not indicate success: 500 (Internal Server Error).
app       |          at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
app       |          at app.Services.BusService.Publish(Order order) in /app/Services/BusService.cs:line 30
app       |          at app.Controllers.CartController.PostAsync(Cart model) in /app/Controllers/CartController.cs:line 58
app       | fail: app.Controllers.CartController[0]
app       |       Response status code does not indicate success: 500 (Internal Server Error).
app       |       System.Net.Http.HttpRequestException: Response status code does not indicate success: 500 (Internal Server Error).
app       |          at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
app       |          at app.Services.PayService.PostPaymentAsync(Order order) in /app/Services/PayService.cs:line 37
app       |          at app.Controllers.CartController.PostAsync(Cart model) in /app/Controllers/CartController.cs:line 58
app       | fail: app.Controllers.CartController[0]
app       |       Response status code does not indicate success: 500 (Internal Server Error).
app       |       System.Net.Http.HttpRequestException: Response status code does not indicate success: 500 (Internal Server Error).
app       |          at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
app       |          at app.Services.PayService.PostPaymentAsync(Order order) in /app/Services/PayService.cs:line 37
app       |          at app.Controllers.CartController.PostAsync(Cart model) in /app/Controllers/CartController.cs:line 58
```
## Ignore strategy


 - 3249 requests (2987 + 2093 timeout -> 5080)
 
 - 0 main errors (IGNORED)
 - 3249 orders stored
   - 30 payments missed
   - 13 mails not sent
   - 13 bus events missed

 - 148.70 requests/sec    
 - 2093 timeout
  

```json
{
    "counter": 3249,
    "counters": {
        "orders": 3249,
        "payments": 3219,
        "mails": 3236,
        "events": 3236
    },
    "duplicates": {
        "orders": 0,
        "payments": 0,
        "mails": 0,
        "events": 0
    },
    "errors": {
        "orders": 0,
        "payments": 30,
        "mails": 13,
        "events": 13
    },
    "requests": {
        "orders": 3249,
        "payments": 3249,
        "mails": 3249,
        "events": 3249
    },
    "counterErrors": 0
}
```


```cmd
Running 20s test @ http://app/Cart
  12 threads and 400 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency     1.91s    53.24ms   2.00s    71.81%
    Req/Sec    23.57     21.45   141.00     74.67%
  Latency Distribution
     50%    1.91s 
     75%    1.95s 
     90%    1.98s 
     99%    2.00s 
  2987 requests in 20.09s, 617.32KB read
  Socket errors: connect 0, read 0, write 0, timeout 2093
Requests/sec:    148.70
Transfer/sec:     30.73KB
```

```cmd
app       | warn: app.Services.MailerService[0]
app       |       InternalServerError
app       | warn: app.Services.BusService[0]
app       |       InternalServerError
app       | warn: app.Services.PayService[0]
app       |       InternalServerError
app       | warn: app.Services.BusService[0]
app       |       InternalServerError
app       | warn: app.Services.MailerService[0]
app       |       InternalServerError
```

## DB Transaction + Payment

 - 5847 orders created == 5847 payments created
 
 - 5847 requests (5537 + 400 timeout -> 5937)
 
 - 59 main errors (payments)
 - 5847 orders stored
   - 59 orders+payments missed
   - 0 payments missed
   - 28 mails not sent
   - 15 bus events missed

 - 275.55 requests/sec    
 - 400 timeout
  

```json
{
    "counter": 5847,
    "counters": {
        "orders": 5847,
        "payments": 5788,
        "mails": 5760,
        "events": 5773
    },
    "duplicates": {
        "orders": 0,
        "payments": 0,
        "mails": 0,
        "events": 0
    },
    "errors": {
        "orders": 0,
        "payments": 59,
        "mails": 28,
        "events": 15
    },
    "requests": {
        "orders": 5847,
        "payments": 5847,
        "mails": 5788,
        "events": 5788
    },
    "counterErrors": 59
}
```

```cmd
Running 20s test @ http://app/Cart
  12 threads and 400 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency     1.30s   212.60ms   1.99s    69.92%
    Req/Sec    34.81     37.40   232.00     86.75%
  Latency Distribution
     50%    1.28s 
     75%    1.49s 
     90%    1.60s 
     99%    1.81s 
  5537 requests in 20.09s, 1.13MB read
  Socket errors: connect 0, read 0, write 0, timeout 400
  Non-2xx or 3xx responses: 54
Requests/sec:    275.55
Transfer/sec:     57.52KB

```

```cmd
app       | fail: app.Controllers.CartController[0]
app       |       Response status code does not indicate success: 500 (Internal Server Error).
app       |       System.Net.Http.HttpRequestException: Response status code does not indicate success: 500 (Internal Server Error).
app       |          at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
app       |          at app.Services.PayService.PostPaymentAsync(Order order) in /app/Services/PayService.cs:line 38
app       |          at app.Controllers.CartController.PostAsync(Cart model) in /app/Controllers/CartController.cs:line 62
app       | warn: app.Services.MailerService[0]
app       |       InternalServerError
app       | warn: app.Services.BusService[0]
app       |       InternalServerError
app       | warn: app.Services.BusService[0]
app       |       InternalServerError
```

## Polly Retry (Mailer)


 - 5752 orders created == 5752 payments created == 5752 emails sent
 
 - 5801 requests (5507 + 410 timeout -> 5917)
 
 - 49 main errors (payments)
 - 5847 orders stored
   - 49 orders+payments missed
   - 0 mails not sent (retry - 23 errors managed with retry)
   - 30 bus events missed

 - 274.06 requests/sec    
 - 410 timeout
  

```json
{
    "counter": 5801,
    "counters": {
        "orders": 5752,
        "payments": 5752,
        "mails": 5752,
        "events": 5722
    },
    "duplicates": {
        "orders": 0,
        "payments": 0,
        "mails": 0,
        "events": 0
    },
    "errors": {
        "orders": 49,
        "payments": 49,
        "mails": 23,
        "events": 30
    },
    "requests": {
        "orders": 5801,
        "payments": 5801,
        "mails": 5775,
        "events": 5752
    },
    "counterErrors": 49
}
```


```csharp
services.AddHttpClient(nameof(MailerService), c => c.BaseAddress = options.GetValue<Uri>("MailerUrl"))
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
        .AddPolicyHandler(GetRetryPolicy());


static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}            
```

```cmd
Running 20s test @ http://app/Cart
  12 threads and 400 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency     1.29s   145.56ms   1.79s    78.10%
    Req/Sec    31.84     26.45   140.00     72.54%
  Latency Distribution
     50%    1.25s 
     75%    1.35s 
     90%    1.46s 
     99%    1.69s 
  5507 requests in 20.09s, 1.12MB read
  Socket errors: connect 0, read 0, write 0, timeout 410
  Non-2xx or 3xx responses: 48
Requests/sec:    274.06
Transfer/sec:     57.16KB

```

## Offline strategy - Worker Bus

 - 7719 orders created == 7719 payments created == 7719 emails sent == 7719 bus events sent
 
 - 7803 requests (7467 + 367 timeout -> 7.834)
 
 - 84 main errors (payments)
 - 7719 orders stored
   - 84 orders+payments missed
   - 0 mails not sent (retry - 42 errors managed with retry)
   - 0 bus events missed (offline loop - 23 errors managed with offline loop)

 - 371.54 requests/sec    
 - 367 timeout
  

```json
{
    "counter": 7803,
    "counters": {
        "orders": 7719,
        "payments": 7719,
        "mails": 7719,
        "events": 7719
    },
    "duplicates": null,
    "errors": {
        "orders": 84,
        "payments": 84,
        "mails": 42,
        "events": 23
    },
    "requests": {
        "orders": 7803,
        "payments": 7803,
        "mails": 7761,
        "events": 7742
    },
    "counterErrors": 84
}
```

```cmd
Running 20s test @ http://app/Cart
  12 threads and 400 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency   983.71ms  149.11ms   2.00s    77.08%
    Req/Sec    45.14     59.68   306.00     84.43%
  Latency Distribution
     50%  951.49ms
     75%    1.04s 
     90%    1.17s 
     99%    1.42s 
  7467 requests in 20.10s, 1.66MB read
  Socket errors: connect 0, read 0, write 0, timeout 367
  Non-2xx or 3xx responses: 80
Requests/sec:    371.54
Transfer/sec:     84.83KB

```