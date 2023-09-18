using userMS.Domain.Entities.Common;

namespace userMS.Domain.Entities
{
    public class User : Entity<Guid>
    {
        public override Guid Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string PhoneNo { get; set; }

        // createdby logic will change (think as if it is a placeholder for now)
        public override Guid CreatedBy { get; set; } = Guid.NewGuid();

        public bool IsEmailVerified = false;

        public bool IsPhoneNumberVerified = false;

        public List<ProviderData> ProviderData { get; set; } = new List<ProviderData>();
    }

    public class ProviderData
    {
        public string FirebaseUid { get; set; }

        public string Identifier { get; set; }

        public string Provider { get; set; }
    }
}
