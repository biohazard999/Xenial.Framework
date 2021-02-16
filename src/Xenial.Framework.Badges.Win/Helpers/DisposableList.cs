using System;
using System.Collections.Generic;
using System.Linq;

namespace Xenial.Framework.Badges.Win.Helpers
{
    internal sealed class DisposableList : List<IDisposable>, IDisposable
    {
        private bool disposedValue;

        public DisposableActionList Actions { get; }

        public DisposableList()
        {
            Actions = new DisposableActionList();
            Add(Actions);
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var disposable in this)
                    {
                        disposable.Dispose();
                    }
                }

                disposedValue = true;
                Clear();
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    internal sealed class DisposableActionList : List<Action>, IDisposable
    {
        private bool disposedValue;

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var action in this)
                    {
                        action();
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
