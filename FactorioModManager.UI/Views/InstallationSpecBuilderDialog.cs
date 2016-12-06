using System;
using System.Windows.Forms;
using FactorioModManager.Lib.Models;

namespace FactorioModManager.UI.Views
{
    class InstallationSpecBuilderDialog
    {
        public InstallationSpec SpecResult { get; private set; }

        readonly InstallationSpecBuilderView _form = new InstallationSpecBuilderView();

        /// <exception cref="InvalidOperationException">The form being shown is already visible.-or- The form being shown is disabled.-or- The form being shown is not a top-level window.-or- The form being shown as a dialog box is already a modal form.-or-The current process is not running in user interactive mode (for more information, see <see cref="P:System.Windows.Forms.SystemInformation.UserInteractive" />).</exception>
        public DialogResult ShowDialog()
        {
            _form.ShowDialog();

            SpecResult = _form.Result;
            return _form.Result == null
                ? DialogResult.Cancel
                : DialogResult.OK;
        }
    }
}
