using System;

namespace FactorioModManager.Lib.Models
{
    public class DisposalFunc : IDisposable
    {
        private readonly Action _func;

        public DisposalFunc()
        {

        }

        public DisposalFunc(Action func)
        {
            _func = func;
        }

        public void Dispose()
        {
            _func?.Invoke();
        }
    }
}
