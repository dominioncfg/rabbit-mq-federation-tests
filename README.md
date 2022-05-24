# Rabbit Mq Federation Tests

Reference application using two applications connected to different RabbitMq Services and communicating through rabbit's federation plugin.

## Steps to execute

1. Run Rabbit MQ:

```bash
docker-compose -p RabbitMq.Federation.Tests up -d
```

2. Connect to the second Docker container

```bash
docker container exec -it federation-datacenter-2 bin/bash
```

3. Inside the container create the Federation Upstream:

```bash
rabbitmqctl set_parameter federation-upstream datacenter1 \
'{"uri":"amqp://rabbitmq-datacenter1","expires":3600000}'
```

4. Inside the container create the Federation Policy:

```bash
rabbitmqctl set_policy --apply-to exchanges outbox-federation "outbound$" '{"federation-upstream-set":"all"}'
```

5. Run the application with Datacenter 1 Configuration in VS (launchSettings.json) to produce messages and with Datacenter 2 to consume them.
