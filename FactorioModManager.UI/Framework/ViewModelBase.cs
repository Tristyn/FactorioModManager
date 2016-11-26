using ReactiveUI;

namespace FactorioModManager.UI.Framework
{
    public abstract class ViewModelBase : ReactiveObject, ISupportsActivation
    {
        ViewModelActivator ISupportsActivation.Activator { get; } = new ViewModelActivator();
    }
}
