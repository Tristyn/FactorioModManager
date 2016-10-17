using System.Reactive.Concurrency;
using System.Windows.Forms;
using FactorioModManager.UI.ViewModels;
using ReactiveUI;

namespace FactorioModManager.UI.Views
{
    public partial class DebugView   : Form, IViewFor<DebugViewModel>
    {
        private DebugViewModel _vm = new DebugViewModel();
        private readonly IScheduler _scheduler;

        public DebugView()
        {
            InitializeComponent();
            _scheduler = new ControlScheduler(this);

            
        }



        #region IViewFor<MyViewModel>
        object IViewFor.ViewModel
        {
            get { return _vm; }
            set { _vm = (DebugViewModel)value; }
        }

        DebugViewModel IViewFor<DebugViewModel>.ViewModel
        {
            get { return _vm; }
            set { _vm = value; }
        }
        #endregion
    }
}
