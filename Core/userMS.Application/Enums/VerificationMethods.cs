using System.ComponentModel;
using System.Text.Json.Serialization;

namespace userMS.Application.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum VerificationMethods
    {
        [Description(nameof(Email))]
        Email = 0,
        [Description(nameof(Phone))]
        Phone = 1
    }
}
