using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unator.Email;

public class LimitReachedException : Exception
{ }

public class SenderServerFailException : Exception
{ }