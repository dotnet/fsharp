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

using System.Collections.Generic;

namespace Microsoft.VisualStudioTools.Project {
    public sealed class PublishProjectOptions {
        private readonly IPublishFile[] _additionalFiles;
        private readonly string _destination;
        public static readonly PublishProjectOptions Default = new PublishProjectOptions(new IPublishFile[0]);

        public PublishProjectOptions(IPublishFile[] additionalFiles = null, string destinationUrl = null) {
            _additionalFiles = additionalFiles ?? Default._additionalFiles;
            _destination = destinationUrl;
        }

        public IList<IPublishFile> AdditionalFiles {
            get {
                return _additionalFiles;
            }
        }

        /// <summary>
        /// Gets an URL which overrides the project publish settings or returns null if no override is specified.
        /// </summary>
        public string DestinationUrl {
            get {
                return _destination;
            }
        }
    }
}
