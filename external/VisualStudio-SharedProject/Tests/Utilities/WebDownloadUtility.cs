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
using System.IO;
using System.Net;

namespace TestUtilities {
    public static class WebDownloadUtility {
        public static string GetString(Uri siteUri) {
            string text;
            var req = HttpWebRequest.CreateHttp(siteUri);
            
            using (var resp = req.GetResponse())
            using (StreamReader reader = new StreamReader(resp.GetResponseStream())) {
                text = reader.ReadToEnd();
            }

            return text;
        }
    }
}
