using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unator.Email;

public interface UEmailSender
{
    public bool IsLimitAllow();

    public Task<Exception?> SendEmail(string from, string to, string subject, string html);
}