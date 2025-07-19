| Failure Type                                 | Scenario                                                    | Behaviour                                               |
|----------------------------------------------|-------------------------------------------------------------|---------------------------------------------------------|
| Connectivity / Network Failures              | 🔌 Kafka broker goes down mid-consumption                   | Consumer may retry, throw timeout or `KafkaException`   |
| Connectivity / Network Failures              | 🌐 Temporary network loss                                   | Message lag increases; may cause rebalances             |
| Connectivity / Network Failures              | 🔁 Broker comes back with stale metadata                    | Misrouted messages or missed partitions                 |
| Connectivity / Network Failures              | 🔒 Firewall dynamically blocks Kafka port                   | Silent consumption halt or delayed failures             |
| Partition Rebalancing / Group Failures       | 🧍 Another consumer joins the group                         | Triggers a rebalance, causing a brief consumption pause |
| Partition Rebalancing / Group Failures       | 🚫 Consumer lags in polling (> `max.poll.interval.ms`)      | Kafka kicks the instance out; rebalancing starts        |
| Partition Rebalancing / Group Failures       | ⚠️ Consumer crashes ungracefully                             | Remaining consumers take over partitions (rebalance hit) |
| Offset Management Pitfalls                   | 🔁 Manual commit succeeds but processing fails              | Message is lost (ack'd but not processed)               |
| Offset Management Pitfalls                   | 🧼 Duplicate processing due to uncommitted offset           | Message redelivery on restart                           |
| Offset Management Pitfalls                   | ❌ `OffsetOutOfRangeException` if topic retention deletes unseen messages | Consumer can't resume                     |
| Deserialization Failures                     | 🔀 Consumer receives malformed or unexpected data           | Throws `SerializationException`                         |
| Deserialization Failures                     | 🧩 Schema changes break compatibility                       | Runtime exceptions; partial processing stops            |
| Deserialization Failures                     | ❓ Null message fields break model binding                  | Fails inside handler logic or mapping layer             |
| Application Logic Failures                   | ❌ Unhandled exception in handler logic                     | Crashes consumer loop unless wrapped                    |
| Application Logic Failures                   | ⏱️ Message processing takes too long                        | Triggers `max.poll.interval.ms` breach → rebalance      |
| Application Logic Failures                   | 🔂 Infinite loop or recursion                               | High CPU usage or app freeze                            |
| Application Logic Failures                   | 📥 Consuming is fast but downstream (DB/API) is slow        | Lag builds up rapidly                                   |

| Resource Exhaustion                          | 💽 Disk full for logs, Serilog sinks, temp files            | Logging fails silently or fatally                       |
| Resource Exhaustion                          | 🧠 Memory leak or large message batching                    | `OutOfMemoryException` or GC storms                     |
| Resource Exhaustion                          | 📊 High CPU from inefficient deserialization or regex usage | Delays polling; risk of lag and rebalancing             |

| Concurrency / Thread Safety Hazards          | 🔀 Shared state across threads (e.g., DB context, logger)   | Race conditions, deadlocks, data loss                   |
| Concurrency / Thread Safety Hazards          | 💣 Thread crash in background processor                     | Partial consumption, silent failure                     |
| Concurrency / Thread Safety Hazards          | ❌ Not using async-safe collections in parallel handlers    | Data corruption or unpredictable behavior               

| Backpressure & Downstream Failures           | 🚫 Downstream service fails repeatedly                      | Message retries clog up consumer loop                   |
| Backpressure & Downstream Failures           | ⛔ DLQ system is down                                       | Poison messages get stuck or dropped silently           |

| Observability & Monitoring Gaps              | 📉 No metrics for consumer lag                              | Delays go unnoticed until it’s too late                 |
| Observability & Monitoring Gaps              | ❌ No health check on consumer loop                         | Liveness/Readiness probes can't detect stuck loop       |
| Observability & Monitoring Gaps              | ❗ No trace ID or correlation for each message              | Impossible to debug failures in prod                    |
| Observability & Monitoring Gaps              | 💬 Logging volume too high or disabled                      | Important context lost during failure                   |
