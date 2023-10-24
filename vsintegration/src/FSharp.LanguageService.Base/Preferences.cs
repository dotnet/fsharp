// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Designer.Interfaces;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using System.Globalization;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;

namespace Microsoft.VisualStudio.FSharp.LanguageService {
    internal enum IndentingStyle {
        None,
        Block,
        Smart
    }

    /// <summary>
    /// LanguagePreferences encapsulates the standard General and Tab settings for a language service
    /// and provides a way of getting and setting the values.  It is expected that you
    /// will have one global LanguagePreferences created by your package.  The General and Tabs
    /// settings are automatically persisted in .vssettings files by the core editor package.
    /// All you need to do is register your language under AutomationProperties/TextEditor
    /// and specify:
    /// <code>
    ///  YourLanguage = s '%YourLocalizedName%'
    ///  {
    ///     val Name = s 'YourLanguage'
    ///     val Package = s '{F5E7E720-1401-11D1-883B-0000F87579D2}'
    ///     val ProfileSave = d 1
    ///     val ResourcePackage = s '%YourPackage%'
    ///  }
    /// </code>
    /// Therefore this class hides all it's properties from user setting persistence using
    /// DesignerSerializationVisibility.Hidden.  This is so that if you give this object
    /// to the Package.ExportSettings method as the AutomationObject then it will only
    /// write out your new settings which is what you want, otherwise the General and Tab
    /// settings will appear in two places in the .vsssettings file.
    /// </summary>
    [CLSCompliant(false), ComVisible(true), Guid("934a92fd-b63a-49c7-9284-11aec8c1e03f")]
    public class LanguagePreferences : IVsTextManagerEvents2, IDisposable {
        IServiceProvider site;
        Guid langSvc;
        LANGPREFERENCES2 prefs;
        NativeMethods.ConnectionPointCookie connection;
        bool enableCodeSense;
        bool enableMatchBraces;
        bool enableQuickInfo;
        bool enableShowMatchingBrace;
        bool enableMatchBracesAtCaret;
        bool enableFormatSelection;
        bool enableCommenting;
        int maxErrorMessages;
        int codeSenseDelay;
        bool enableAsyncCompletion;
        bool autoOutlining;
        int maxRegionTime;
        _HighlightMatchingBraceFlags braceFlags;
        string name;

        /// <summary>
        /// Gets the language preferences.
        /// </summary>
        internal LanguagePreferences(IServiceProvider site, Guid langSvc, string name) {
            this.site = site;
            this.langSvc = langSvc;
            this.name = name;
        }

        internal LanguagePreferences() { }

        protected string LanguageName {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// This property is not public for a reason. If it were public it would
        /// get called during LoadSettingsFromStorage which will break it.  
        /// Instead use GetSite().
        /// </summary>
        protected IServiceProvider Site {
            get { return this.site; }
            set { this.site = value; }
        }

        public IServiceProvider GetSite() {
            return this.site;
        }

        // Our base language service preferences (from Babel originally)
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableCodeSense {
            get { return this.enableCodeSense; }
            set { this.enableCodeSense = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableMatchBraces {
            get {
                return this.enableMatchBraces;
            }
            set { this.enableMatchBraces = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableQuickInfo {
            get { return this.enableQuickInfo; }
            set { this.enableQuickInfo = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableShowMatchingBrace {
            get { return this.enableShowMatchingBrace; }
            set { this.enableShowMatchingBrace = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableMatchBracesAtCaret {
            get { return this.enableMatchBracesAtCaret; }
            set { this.enableMatchBracesAtCaret = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MaxErrorMessages {
            get { return this.maxErrorMessages; }
            set { this.maxErrorMessages = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CodeSenseDelay {
            get { return this.codeSenseDelay; }
            set { this.codeSenseDelay = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableAsyncCompletion {
            get { return this.enableAsyncCompletion; }
            set { this.enableAsyncCompletion = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableFormatSelection {
            get { return this.enableFormatSelection; }
            set { this.enableFormatSelection = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableCommenting {
            get { return this.enableCommenting; }
            set { this.enableCommenting = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MaxRegionTime {
            get { return this.maxRegionTime; }
            set { this.maxRegionTime = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public _HighlightMatchingBraceFlags HighlightMatchingBraceFlags {
            get { return this.braceFlags; }
            set { this.braceFlags = value; }
        }

        public virtual void Init() {
            ILocalRegistry3 localRegistry = site.GetService(typeof(SLocalRegistry)) as ILocalRegistry3;
            string root = null;
            if (localRegistry != null) {
                NativeMethods.ThrowOnFailure(localRegistry.GetLocalRegistryRoot(out root));
            }
            if (root != null) {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(root, false)) {
                    if (key != null) {
                        InitMachinePreferences(key, name);
                    }
                }
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(root, false)) {
                    if (key != null) {
                        InitUserPreferences(key, name);
                    }
                }
            }
            Connect();
            localRegistry = null;
        }

        public virtual void InitUserPreferences(RegistryKey key, string name) {
            this.GetLanguagePreferences();
        }
        public int GetIntegerValue(RegistryKey key, string name, int def) {
            object o = key.GetValue(name);
            if (o is int) return (int)o;
            if (o is string) return int.Parse((string)o, CultureInfo.InvariantCulture);
            return def;
        }
        public bool GetBooleanValue(RegistryKey key, string name, bool def) {
            object o = key.GetValue(name);
            if (o is int) return ((int)o != 0);
            if (o is string) return bool.Parse((string)o);
            return def;
        }

        public virtual void InitMachinePreferences(RegistryKey key, string name) {
            using (RegistryKey keyLanguage = key.OpenSubKey("languages\\language services\\" + name, false)) {
                if (keyLanguage != null) {
                    this.EnableCodeSense = GetBooleanValue(keyLanguage, "CodeSense", true);
                    this.EnableMatchBraces = GetBooleanValue(keyLanguage, "MatchBraces", true);
                    this.EnableQuickInfo = GetBooleanValue(keyLanguage, "QuickInfo", true);
                    this.EnableShowMatchingBrace = GetBooleanValue(keyLanguage, "ShowMatchingBrace", true);
                    this.EnableMatchBracesAtCaret = GetBooleanValue(keyLanguage, "MatchBracesAtCaret", true);
                    this.MaxErrorMessages = GetIntegerValue(keyLanguage, "MaxErrorMessages", 10);
                    this.CodeSenseDelay = GetIntegerValue(keyLanguage, "CodeSenseDelay", 1000);
                    this.EnableAsyncCompletion = GetBooleanValue(keyLanguage, "EnableAsyncCompletion", true);
                    this.EnableFormatSelection = GetBooleanValue(keyLanguage, "EnableFormatSelection", false);
                    this.EnableCommenting = GetBooleanValue(keyLanguage, "EnableCommenting", true);
                    this.AutoOutlining = GetBooleanValue(keyLanguage, "AutoOutlining", true);
                    this.MaxRegionTime = GetIntegerValue(keyLanguage, "MaxRegionTime", 2000); // 2 seconds
                    this.braceFlags = (_HighlightMatchingBraceFlags)GetIntegerValue(keyLanguage, "HighlightMatchingBraceFlags", (int)_HighlightMatchingBraceFlags.HMB_USERECTANGLEBRACES);
                }
            }
        }

        public virtual void Dispose() {
            Disconnect();
            site = null;
        }

        // General tab
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AutoListMembers {
            get { return prefs.fAutoListMembers != 0; }
            set { prefs.fAutoListMembers = (uint)(value ? 1 : 0); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HideAdvancedMembers {
            get { return prefs.fHideAdvancedAutoListMembers != 0; }
            set { prefs.fHideAdvancedAutoListMembers = (uint)(value ? 1 : 0); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ParameterInformation {
            get { return prefs.fAutoListParams != 0; }
            set { prefs.fAutoListParams = (uint)(value ? 1 : 0); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VirtualSpace {
            get { return prefs.fVirtualSpace != 0; }
            set { prefs.fVirtualSpace = (uint)(value ? 1 : 0); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool WordWrap {
            get { return prefs.fWordWrap != 0; }
            set { prefs.fWordWrap = (uint)(value ? 1 : 0); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool WordWrapGlyphs {
            get { return (int)prefs.fWordWrapGlyphs != 0; }
            set { prefs.fWordWrapGlyphs = (uint)(value ? 1 : 0); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CutCopyBlankLines {
            get { return (int)prefs.fCutCopyBlanks != 0; }
            set { prefs.fCutCopyBlanks = (uint)(value ? 1 : 0); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool LineNumbers {
            get { return prefs.fLineNumbers != 0; }
            set { prefs.fLineNumbers = (uint)(value ? 1 : 0); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableLeftClickForURLs {
            get { return prefs.fHotURLs != 0; }
            set { prefs.fHotURLs = (uint)(value ? 1 : 0); }
        }

        // Tabs tab
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal IndentingStyle IndentStyle {
            get { return (IndentingStyle)prefs.IndentStyle; }
            set { prefs.IndentStyle = (vsIndentStyle)value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TabSize {
            get { return (int)prefs.uTabSize; }
            set { prefs.uTabSize = (uint)value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int IndentSize {
            get { return (int)prefs.uIndentSize; }
            set { prefs.uIndentSize = (uint)value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InsertTabs {
            get { return prefs.fInsertTabs != 0; }
            set { prefs.fInsertTabs = (uint)(value ? 1 : 0); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowNavigationBar {
            get { return (int)prefs.fDropdownBar != 0; }
            set { prefs.fDropdownBar = (uint)(value ? 1 : 0); }
        }

        public bool AutoOutlining {
            get { return this.autoOutlining; }
            set { this.autoOutlining = value; }
        }

        public virtual void GetLanguagePreferences() {
            IVsTextManager textMgr = site.GetService(typeof(SVsTextManager)) as IVsTextManager;
            if (textMgr != null) {
                this.prefs.guidLang = langSvc;
                IVsTextManager2 textMgr2 = site.GetService(typeof(SVsTextManager)) as IVsTextManager2;
                if (textMgr2 != null) {
                    LANGPREFERENCES2[] langPrefs2 = new LANGPREFERENCES2[1];
                    langPrefs2[0] = this.prefs;
                    if (NativeMethods.Succeeded(textMgr2.GetUserPreferences2(null, null, langPrefs2, null))) {
                        this.prefs = langPrefs2[0];
                    } else {
                        Debug.Assert(false, "textMgr2.GetUserPreferences2");
                    }
                }
            }
        }

        public virtual void Apply() {
            IVsTextManager2 textMgr2 = site.GetService(typeof(SVsTextManager)) as IVsTextManager2;
            if (textMgr2 != null) {
                this.prefs.guidLang = langSvc;
                LANGPREFERENCES2[] langPrefs2 = new LANGPREFERENCES2[1];
                langPrefs2[0] = this.prefs;
                if (!NativeMethods.Succeeded(textMgr2.SetUserPreferences2(null, null, langPrefs2, null))) {
                    Debug.Assert(false, "textMgr2.SetUserPreferences2");
                }
            }
        }

        private void Connect() {
            if (this.connection == null && this.site != null) {
                IVsTextManager2 textMgr2 = this.site.GetService(typeof(SVsTextManager)) as IVsTextManager2;
                if (textMgr2 != null) {
                    this.connection = new NativeMethods.ConnectionPointCookie(textMgr2, (IVsTextManagerEvents2)this, typeof(IVsTextManagerEvents2));
                }
            }
        }

        private void Disconnect() {
            if (this.connection != null) {
                this.connection.Dispose();
                this.connection = null;
            }
        }

        public virtual int OnRegisterMarkerType(int iMarkerType) {
            return NativeMethods.S_OK;
        }
        public virtual int OnRegisterView(IVsTextView view) {
            return NativeMethods.S_OK;
        }
        public virtual int OnUnregisterView(IVsTextView view) {
            return NativeMethods.S_OK;
        }
        public virtual int OnReplaceAllInFilesBegin() {
            return NativeMethods.S_OK;
        }
        public virtual int OnReplaceAllInFilesEnd() {
            return NativeMethods.S_OK;
        }

        public virtual int OnUserPreferencesChanged2(VIEWPREFERENCES2[] viewPrefs, FRAMEPREFERENCES2[] framePrefs, LANGPREFERENCES2[] langPrefs, FONTCOLORPREFERENCES2[] fontColorPrefs) {
            if (langPrefs != null && langPrefs.Length > 0 && langPrefs[0].guidLang == this.langSvc) {
                this.prefs = langPrefs[0];
            }
            return NativeMethods.S_OK;
        }
    }
}
