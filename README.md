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
## Ignore strategy

