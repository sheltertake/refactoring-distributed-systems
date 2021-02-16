# refactoring-distributed-systems

## No exception handling

 - 110 random errors
 - 5733 orders stored
   - 67 payments missed
   - 88 mails not sent
     - 21 caused by mailer error
     - 67 caused by payments error
   - 110 bus events missed
     - 22 caused by bus error
     - 21 caused by mailer error
     - 67 caused by payments error

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

