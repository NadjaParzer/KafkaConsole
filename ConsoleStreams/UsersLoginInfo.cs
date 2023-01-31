using Confluent.Kafka;
using System.Text.Json;

namespace sample_stream_demo
{
    public class UsersLoginInfo : ISerializer<UsersLoginInfo>, IDeserializer<UsersLoginInfo>
    {
        public List<LoginMessage> Users { get; set; }
        public string Type { get; set; }

        public UsersLoginInfo Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonSerializer.Deserialize<UsersLoginInfo>(data.ToArray());
        }

        public byte[] Serialize(UsersLoginInfo data, SerializationContext context)
        {
            using (var ms = new MemoryStream())
            {
                string jsonString = JsonSerializer.Serialize(data);
                var writer = new StreamWriter(ms);

                writer.Write(jsonString);
                writer.Flush();
                ms.Position = 0;

                return ms.ToArray();
            }

        }
    }
}
