﻿namespace userMS.Application.DTOs
{
    public class FirebaseAuthResponseDto
    {
        public string IdToken { get; set; }
        public string LocalId { get; set; }
        public string Email { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresIn { get; set; }
    }
}
