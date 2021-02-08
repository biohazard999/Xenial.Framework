using System;
using System.Collections.Generic;
using System.Linq;

namespace Xenial.Framework.Badges.Win.Helpers
{
    internal sealed class DisposableList : List<IDisposable>, IDisposable
    {
        private bool disposedValue;

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
}
