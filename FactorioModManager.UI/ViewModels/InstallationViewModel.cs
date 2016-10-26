﻿using System;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Forms;
using FactorioModManager.Lib;
using FactorioModManager.Lib.Archive;
using FactorioModManager.Lib.Models;
using FactorioModManager.Lib.Web;
using FactorioModManager.UI.Framework;
using ReactiveUI;

namespace FactorioModManager.UI.ViewModels
{
    public class InstallationViewModel : ViewModelBase
    {
        private Installation _model;

        private readonly ObservableAsPropertyHelper<string> _status;
        private string _installFileArchiveFilePath;

        /// <exception cref="ArgumentNullException"><paramref name="model"/> is <see langword="null" />.</exception>
        public InstallationViewModel(Installation model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            Model = model;

            this.WhenAnyObservable(viewModel => viewModel.Model.Status)
                .Select(status => status.ToString())
                .ToProperty(this, viewModel => viewModel.Status, out _status);

            RefreshStatus = ReactiveCommand.CreateAsyncTask(o => Model.RefreshStatus());

            InstallFileArchive = ReactiveCommand.CreateAsyncTask(o => InstallArchiveImpl());
        }

        public Task<Stream> OpenArchiveFileImpl()
        {
            throw new NotImplementedException();
        }

        public async Task InstallArchiveImpl()
        {
            /*
            var archiveExtension = GameArchiveSpec.GetArchiveExtension(os);

            var zipDialog = new OpenFileDialog
            {
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                DefaultExt = archiveExtension,
                SupportMultiDottedExtensions = true,
                Filter = string.Format("Archive Files (*{0}) | *{0}", archiveExtension)
            };
            
            if (zipDialog.ShowDialog() != DialogResult.OK)
                return;
                */
            // Do this: http://stackoverflow.com/a/25630554
            // Pass FileName or stream or something as an argument to the second command using Observable.Invoke
            if (InstallFileArchiveFilePath == null)
                return;

            var os = OperatingSystemEx.CurrentOS;
            var archiveSpec = new GameArchiveSpec(Model.Spec, os);

            try
            {
                using (var archive = new GameArchive(InstallFileArchiveFilePath, archiveSpec))
                {
                    await Model.InstallFromArchive(archive);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Unauthorized access");
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("The specified path is invalid (for example, it is on an unmapped drive).");
            }
            catch (IOException)
            {
                MessageBox.Show("The directory is a file.-or-The network name is not known.");
            }
            catch (SecurityException)
            {
                MessageBox.Show("Security exception");
            }
        }

        private Installation Model
        {
            get { return _model; }
            set { this.RaiseAndSetIfChanged(ref _model, value); }
        }

        public string Status => string.Format("Status: {0}", _status.Value);

        public InstallationSpec Spec => _model.Spec;
        
        public IReactiveCommand RefreshStatus { get; }

        public string InstallFileArchiveFilePath
        {
            get { return _installFileArchiveFilePath; }
            set { this.RaiseAndSetIfChanged(ref _installFileArchiveFilePath, value); }
        }

        public IReactiveCommand InstallFileArchive { get; }
    }
}
