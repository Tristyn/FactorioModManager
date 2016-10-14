using System;
using System.Windows.Forms;
using FactorioModManager.Lib.Models;

namespace FactorioModManager.UI.Dialogs
{
    public partial class InstallationSpecBuilderForm : Form
    {
        public InstallationSpec Result { get; private set; }

        public InstallationSpecBuilderForm()
        {
            InitializeComponent();
            ValidateInternal();
        }

        public void ValidateHandler(object sender, EventArgs e)
        {
            ValidateInternal();
        }

        private void ValidateInternal()
        {
            InstallationSpec spec;
            OkButton.Enabled = TryParseFields(out spec);
            Result = spec;
        }

        private bool TryParseFields(out InstallationSpec spec)
        {
            spec = null;

            try
            {
                var versionStr = string.Format(
                    "{0}.{1}.{2}",
                    VersionMajor.Text, 
                    VersionMinor.Text, 
                    VersionRevision.Text);
                var version = VersionNumber.Parse(versionStr);
                
                CpuArchitecture cpu;
                if (CpuArchitecture.SelectedItem as string == "64 Bit")
                    cpu = Lib.Models.CpuArchitecture.X64;
                else if (CpuArchitecture.SelectedItem as string == "32 Bit")
                    cpu = Lib.Models.CpuArchitecture.X86;
                else return false;

                BuildConfiguration build;
                if (!Enum.TryParse(BuildConfiguration.SelectedItem as string, out build))
                    return false;

                spec = new InstallationSpec(version, cpu, build);
                return true;

            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public void SubmitHandler(object sender, EventArgs e)
        {
            SubmitInternal();
        }

        private void SubmitInternal()
        {
            ValidateInternal();
            if(Result != null)
                Close();
        }
    }
}
