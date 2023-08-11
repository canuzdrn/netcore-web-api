using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace userMS.Infrastructure.Com
{
    public class AppSettings
    {
        public EmailSettings EmailSettings { get; set; }

        public EmailContent EmailContent { get; set; }

        public JwtSettings JwtSettings { get; set; }
    }
}
