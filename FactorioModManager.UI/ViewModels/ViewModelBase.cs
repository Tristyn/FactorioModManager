using ReactiveUI;

namespace FactorioModManager.UI.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject, ISupportsActivation
    {
        ViewModelActivator ISupportsActivation.Activator { get; } = new ViewModelActivator();
    }
}
