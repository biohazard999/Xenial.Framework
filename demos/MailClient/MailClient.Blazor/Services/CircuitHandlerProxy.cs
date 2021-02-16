using System;
using System.Threading;
using System.Threading.Tasks;

using DevExpress.ExpressApp.Blazor.Services;

using Microsoft.AspNetCore.Components.Server.Circuits;

namespace MailClient.Blazor.Services
{
    public class CircuitHandlerProxy : CircuitHandler
    {
        private readonly IScopedCircuitHandler scopedCircuitHandler;

        public CircuitHandlerProxy(IScopedCircuitHandler scopedCircuitHandler)
            => this.scopedCircuitHandler = scopedCircuitHandler;

        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
            => scopedCircuitHandler.OnCircuitOpenedAsync(cancellationToken);

        public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
            => scopedCircuitHandler.OnConnectionUpAsync(cancellationToken);

        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
            => scopedCircuitHandler.OnCircuitClosedAsync(cancellationToken);

        public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
            => scopedCircuitHandler.OnConnectionDownAsync(cancellationToken);
    }
}
