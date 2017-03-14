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

namespace Microsoft.VisualStudioTools.Project {
    /// <summary>
    /// Implements a publisher which handles publishing the list of files to a destination.
    /// </summary>
    public interface IProjectPublisher {
        /// <summary>
        /// Publishes the files listed in the given project to the provided URI.
        /// 
        /// This function should return when publishing is complete or throw an exception if publishing fails.
        /// </summary>
        /// <param name="project">The project to be published.</param>
        /// <param name="destination">The destination URI for the project.</param>
        void PublishFiles(IPublishProject project, Uri destination);

        /// <summary>
        /// Gets a localized description of the destination type (web site, file share, etc...)
        /// </summary>
        string DestinationDescription {
            get;
        }

        /// <summary>
        /// Gets the schema supported by this publisher - used to select which publisher will
        /// be used based upon the schema of the Uri provided by the user.
        /// </summary>
        string Schema {
            get;
        }
    }
}
