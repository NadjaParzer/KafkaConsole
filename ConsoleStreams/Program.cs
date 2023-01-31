using Confluent.Kafka;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.SerDes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Streamiz.Kafka.Net.Table;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Security.Permissions;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Streamiz.Kafka.Net.Mock;
using Streamiz.Kafka.Net.Processors;
using Streamiz.Kafka.Net.Processors.Public;
using Streamiz.Kafka.Net.State;
using Streamiz.Kafka.Net.Stream;
using Confluent.Kafka.Admin;

namespace sample_stream_demo
{
    class Program
    {
        public static async Task Main()
        {
            await CreateTopics("stream", "join-topic");
            var config = new StreamConfig<StringSerDes, StringSerDes>();
            config.ApplicationId = "test-app";
            config.BootstrapServers = "localhost:29092";

            StreamBuilder builder = new StreamBuilder();

            var kstream = builder.Stream<string, string>("stream");
            var ktable = builder.Table("table", InMemory.As<string, string>("table-store"));



            kstream.Join(ktable, (v, v1) => $"{v}-{v1}")
                   .To("join-topic");

            Topology t = builder.Build();
            KafkaStream stream = new KafkaStream(t, config);

            Console.CancelKeyPress += (o, e) => {
                stream.Dispose();
            };

            await stream.StartAsync();
            Console.ReadLine();
        }

        private static async Task CreateTopics(string input, string output)
        {
            AdminClientConfig config = new AdminClientConfig();
            config.BootstrapServers = "localhost:29092";

            AdminClientBuilder builder = new AdminClientBuilder(config);
            var client = builder.Build();
            try
            {
                await client.CreateTopicsAsync(new List<TopicSpecification>
                {
                    new TopicSpecification() {Name = input},
                    new TopicSpecification() {Name = output},
                    new TopicSpecification() {Name = "table"},
                });
            }
            catch (Exception e)
            {
                // do nothing in case of topic already exist
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}