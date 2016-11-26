using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace FactorioModManager.UI.Extensions
{
    public static class CompositeDisposableExtensions
    {
        public static IDisposable WhenActivated(
            this IActivatable @this,
            Action<CompositeDisposable> disposables,
            IViewFor view = null)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            return @this
                .WhenActivated(
                    () =>
                    {
                        var d = new CompositeDisposable();
                        disposables(d);
                        return new[] { d };
                    },
                    view);
        }

        public static void WhenActivated(
                this ISupportsActivation @this,
                Action<CompositeDisposable> disposables)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            @this.WhenActivated(
                () =>
                {
                    var d = new CompositeDisposable();
                    disposables(d);
                    return new[] { d };
                });
        }

        public static T AddTo<T>(this T @this, CompositeDisposable compositeDisposable)
            where T : IDisposable
        {
            if (@this == null)
                throw new ArgumentNullException("this");
            if (compositeDisposable == null)
                throw new ArgumentNullException("compositeDisposable");

            compositeDisposable.Add(@this);
            return @this;
        }
    }
}
