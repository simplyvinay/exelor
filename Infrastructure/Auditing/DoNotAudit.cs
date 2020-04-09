using System;

namespace Exelor.Infrastructure.Auditing
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DoNotAudit : Attribute
    {
        
    }
}