using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Kafka.Message.Service.Producer.Model
{
    /// <summary>
    /// Class for defining producer message model.
    /// </summary>
    public class MessageModel
    {
        /// <summary>
        /// Action create|update|delete
        /// </summary>
        public string? Action { get; set; }

        /// <summary>
        /// Gets or sets Identifier for each message.
        /// </summary>
        public string Key { get; set; }= Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets published date.
        /// </summary>
        public DateTime DatePublished { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the data to be passed as message.
        /// </summary>
        [Required]
        public string? Data { get; set; }

        /// <summary>
        /// Gets or sets version of the message.
        /// </summary>
        public string? Version { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
