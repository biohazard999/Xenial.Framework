using System;

using DevExpress.ExpressApp.Xpo;

namespace MailClient.Blazor.Services
{
    public class XpoDataStoreProviderAccessor
    {
        public IXpoDataStoreProvider? DataStoreProvider { get; set; }
    }
}
