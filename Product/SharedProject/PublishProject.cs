/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.Project {
    class PublishProject : IPublishProject {
        private readonly CommonProjectNode _node;
        private ReadOnlyCollection<IPublishFile> _files;
        private readonly IVsStatusbar _statusBar;
        private readonly PublishProjectOptions _options;
        private int _progress;

        public PublishProject(CommonProjectNode node, PublishProjectOptions options) {
            _statusBar = (IVsStatusbar)node.Site.GetService(typeof(SVsStatusbar));
            _statusBar.SetText("Starting publish...");
            _node = node;
            _options = options;
        }

        #region IPublishProject Members

        public IList<IPublishFile> Files {
            get {
                if (_files == null) {
                    List<IPublishFile> files = new List<IPublishFile>();
                    foreach (var item in _node.CurrentConfig.Items) {
                        bool? publish = GetPublishSetting(item);

                        // publish if we're a Compile node and we haven't been disabled or if 
                        // we've been specifically enabled.
                        if ((item.ItemType == "Compile" && (publish == null || publish.Value)) ||
                            (publish != null && publish.Value)) {

                            string file = item.GetMetadataValue("FullPath");

                            string destFile = Path.GetFileName(file);
                            if (CommonUtils.IsSubpathOf(_node.ProjectHome, file)) {
                                destFile = CommonUtils.GetRelativeFilePath(_node.ProjectHome, file);
                            } else {
                                destFile = Path.GetFileName(file);
                            }

                            files.Add(new PublishFile(file, destFile));
                        }
                    }

                    foreach (var file in _options.AdditionalFiles) {
                        files.Add(file);
                    }

                    _files = new ReadOnlyCollection<IPublishFile>(files);
                }

                return _files;
            }
        }

        private static bool? GetPublishSetting(Build.Execution.ProjectItemInstance item) {
            bool? publish = null;
            string pubValue = item.GetMetadataValue("Publish");
            bool pubSetting;
            if (!String.IsNullOrWhiteSpace(pubValue) && Boolean.TryParse(pubValue, out pubSetting)) {
                publish = pubSetting;
            }
            return publish;
        }

        public string ProjectDir {
            get {
                return _node.ProjectHome;
            }
        }

        public int Progress {
            get {
                return _progress;
            }
            set {
                _progress = value;
                _statusBar.SetText(String.Format("Publish {0}% done...", _progress));
            }
        }

        #endregion

        internal void Done() {
            _statusBar.SetText("Publish succeeded");
        }

        internal void Failed(string msg) {
            _statusBar.SetText("Publish failed: " + msg);
        }
    }
}
