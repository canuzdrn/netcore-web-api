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
        public override Guid CreatedBy { get; set; }

        public bool IsEmailVerified = false;

        public bool IsPhoneNumberVerified = false;

    }
}
