using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CsQuery;
using FactorioModManager.Lib.Models;
using OperatingSystem = FactorioModManager.Lib.Models.OperatingSystem;
using Version = FactorioModManager.Lib.Models.Version;

namespace FactorioModManager.Lib.Web
{
    public class GameArchiveFeed : IEnumerable<GameArchiveSpec>
    {
        private readonly IEnumerable<GameArchiveSpec> _downloads;

        public GameArchiveFeed(IEnumerable<GameArchiveSpec> downloads)
        {
            _downloads = downloads;
        }

        /// <summary>
        /// Build the feed using html from https://www.factorio.com/download or a similar page.
        /// </summary>
        public static GameArchiveFeed FromFactorioDownloadsPageHtml(string html)
        {
            var downloads = new List<GameArchiveSpec>();

            var dom = CQ.CreateDocument(html);

            var containers = dom["div.container"];

            // Ignore the page header, the container for downloads should be immediately after.
            var container = containers.First(domObj => !domObj.HasClass("header"));

            var versionNodes = container.ChildElements.Where(domObj => domObj.NodeName == "H3");

            foreach (var releaseHeader in versionNodes)
            {
                // Each header node is an H3 with the inner
                // text format "0.0.0 ([alpha | demo | headless])"
                var headerText = releaseHeader.InnerText;

                var splitHeader = headerText
                    .Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(str => str.Trim())
                    .ToList();

                Debug.Assert(splitHeader.Count == 2);

                var versionInts = splitHeader[0].Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                Debug.Assert(versionInts.Length == 3);

                var version = Version.Parse(splitHeader[0]);

                BuildConfiguration type;
                if (headerText.Contains("alpha"))
                    type = BuildConfiguration.Client;
                else if (headerText.Contains("headless"))
                    type = BuildConfiguration.Server;
                else type = BuildConfiguration.Demo;

                // A <P> tag, ignore it
                var releaseDescription = releaseHeader.NextElementSibling;
                Debug.Assert(releaseDescription.NodeName == "P");
                var downloadsList = releaseDescription.NextElementSibling;
                Debug.Assert(downloadsList.NodeName == "UL");

                // From each list item, grab the text contained in the anchor tag
                var downloadLinkTexts = downloadsList.ChildElements
                    .Select(listItem => listItem.ChildNodes.Single(liChilds => liChilds.NodeName == "A").InnerText);

                // Parse the anchor node text for OS and cpu architechure 
                foreach (var linkText in downloadLinkTexts)
                {
                    // link text format:
                    // [MS Windows | macOS | Generic Linux tar package] ([32 | 64] [ignorable text])

                    OperatingSystem os;
                    if (linkText.StartsWith("MS Windows"))
                        os = OperatingSystem.Windows;
                    else if (linkText.StartsWith("macOS"))
                        os = OperatingSystem.Mac;
                    else // if (linkText.StartsWith("Generic Linux"))
                        os = OperatingSystem.Linux;

                    CpuArchitecture cpu;
                    if (linkText.Contains("32"))
                        cpu = CpuArchitecture.X86;
                    else cpu = CpuArchitecture.X86_64;

                    downloads.Add(new GameArchiveSpec(version, cpu, type, os));
                }
            }

            return new GameArchiveFeed(downloads);
        }

        public IEnumerator<GameArchiveSpec> GetEnumerator()
        {
            return _downloads.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _downloads.GetEnumerator();
        }
    }
}
