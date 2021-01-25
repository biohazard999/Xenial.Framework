using System;
using System.Collections.Generic;
using System.Text;

namespace MailClient.Module.BusinessObjects
{
    internal static class ModelTypeList
    {
        internal static readonly Type[] PersistentTypes = new[]
        {
            typeof(MailBaseObject),
            typeof(MailBaseObjectId)
        };
    }
}
