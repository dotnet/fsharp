// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService.Resources {
    using System;
    using System.Reflection;
    using System.Globalization;
    using System.Resources;
    using System.Text;
    using System.Threading;
    using System.ComponentModel;
    using System.Security.Permissions;

   [AttributeUsage(AttributeTargets.All)]
    internal sealed class SRDescriptionAttribute : DescriptionAttribute {

        private bool replaced = false;

        public SRDescriptionAttribute(string description) : base(description) {
        }

        public override string Description {
            get {
                if (!replaced) {
                    replaced = true;
                    DescriptionValue = SR.GetString(base.Description);
                }
                return base.Description;
            }
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class SRCategoryAttribute : CategoryAttribute {

        public SRCategoryAttribute(string category) : base(category) {
        }

        protected override string GetLocalizedString(string value) {
            return SR.GetString(value);
        }
    }
    internal sealed class SR {
    internal const string BraceMatchStatus = "BraceMatchStatus";
    internal const string BufferChanged = "BufferChanged";
    internal const string ComboMembersTip = "ComboMembersTip";
    internal const string ComboTypesTip = "ComboTypesTip";
    internal const string CommentSelection = "CommentSelection";
    internal const string EditIntersects = "EditIntersects";
    internal const string FormatSpan = "FormatSpan";
    internal const string MaxErrorsReached = "MaxErrorsReached";
    internal const string NoExpansionSession = "NoExpansionSession";
    internal const string Truncated = "Truncated";
    internal const string UncommentSelection = "UncommentSelection";
    internal const string UnsupportedFormat = "UnsupportedFormat";
    internal const string TemplateNotPrepared = "TemplateNotPrepared";
    internal const string UnknownBuffer = "UnknownBuffer";
    internal const string UnrecognizedFilterFormat = "UnrecognizedFilterFormat";

        static SR loader = null;
        ResourceManager resources;

        private static Object s_InternalSyncObject;
        private static Object InternalSyncObject {
            get {
                if (s_InternalSyncObject == null) {
                    Object o = new Object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, o, null);
                }
                return s_InternalSyncObject;
            }
        }
        
        internal SR() {
#if FX_ATLEAST_45
            resources = new System.Resources.ResourceManager("FSharp.LanguageService.Base.Microsoft.VisualStudio.Package.LanguageService", this.GetType().Assembly);
#else
            resources = new System.Resources.ResourceManager("Microsoft.VisualStudio.Package.LanguageService", this.GetType().Assembly);
#endif
        }
        
        private static SR GetLoader() {
            if (loader == null) {
                lock (InternalSyncObject) {
                   if (loader == null) {
                       loader = new SR();
                   }
               }
            }
            
            return loader;
        }

        private static CultureInfo Culture {
            get { return null/*use ResourceManager default, CultureInfo.CurrentUICulture*/; }
        }
        
        public static ResourceManager Resources {
            get {
                return GetLoader().resources;
            }
        }
        
        public static string GetString(string name, params object[] args) {
            SR sys = GetLoader();
            if (sys == null)
                return null;
            string res = sys.resources.GetString(name, SR.Culture);

            if (args != null && args.Length > 0) {
                return String.Format(CultureInfo.CurrentCulture, res, args);
            }
            else {
                return res;
            }
        }

        public static string GetString(string name) {
            SR sys = GetLoader();
            if (sys == null)
                return null;
            return sys.resources.GetString(name, SR.Culture);
        }
        
        public static object GetObject(string name) {
            SR sys = GetLoader();
            if (sys == null)
                return null;
            return sys.resources.GetObject(name, SR.Culture);
        }
}
}
