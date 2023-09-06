using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace userMS.Application.DTOs.Response
{
    public class FirebaseErrorResponseDto
    {
        [JsonPropertyName("error")]
        public FirebaseError Error { get; set; }
    }
}
