using Confluent.Kafka;
using System.Text.Json;

namespace sample_stream_demo
{
    public class LoginMessage : ISerializer<LoginMessage>, IDeserializer<LoginMessage>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool IsLoggedIn { get; set; }

        public LoginMessage Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonSerializer.Deserialize<LoginMessage>(data.ToArray());
        }

        public byte[] Serialize(LoginMessage data, SerializationContext context)
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
