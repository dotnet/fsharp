// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.


using System;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Win32;

namespace Microsoft.VisualStudio.Shell
{
    /// <summary>
    /// This attribute can be used to register information about a project system that supports
    /// the WAP flavor/sub-type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal class WAProvideProjectFactoryAttribute : RegistrationAttribute
    {
        private Type _factoryType;
        private string _displayProjectFileExtensions = null;
        private string _name;
        private string _displayName = null;
        private string _defaultProjectExtension = null;
        private string _possibleProjectExtensions = null;
        private string _projectTemplatesDirectory;
        private int _sortPriority = 100;
        private Guid _folderGuid = Guid.Empty;
        private string _languageVsTemplate;
        private string _templateGroupIDsVsTemplate;
        private string _templateIDsVsTemplate;
        private string _displayProjectTypeVsTemplate;
        private string _projectSubTypeVsTemplate;
        private bool _newProjectRequireNewFolderVsTemplate = false;
        private bool _showOnlySpecifiedTemplatesVsTemplate = false;

        public WAProvideProjectFactoryAttribute(Type factoryType, string name)
        {
            if (factoryType == null)
            {
                throw new ArgumentNullException("factoryType");
            }

            _factoryType = factoryType;
            _name = name;
        }

        public WAProvideProjectFactoryAttribute(Type factoryType, 
                                                string name, 
                                                string languageVsTemplate, 
                                                bool showOnlySpecifiedTemplatesVsTemplate, 
                                                string templateGroupIDsVsTemplate, 
                                                string templateIDsVsTemplate)
        {
            if (factoryType == null)
            {
                throw new ArgumentNullException("factoryType");
            }

            _factoryType = factoryType;
            _name = name;
            _languageVsTemplate = languageVsTemplate;
            _templateGroupIDsVsTemplate = templateGroupIDsVsTemplate;
            _templateIDsVsTemplate = templateIDsVsTemplate;
            _showOnlySpecifiedTemplatesVsTemplate = showOnlySpecifiedTemplatesVsTemplate;
        }

        public string Name
        {
            get { return _name; }
        }

        public string DisplayName
        {
            get { return _displayName; }
        }

        public int SortPriority
        {
            get { return _sortPriority; }
            set { _sortPriority = value; }
        }

        public Type FactoryType
        {
            get
            {
                return _factoryType;
            }
        }

        public string DisplayProjectFileExtensions
        {
            get
            {
                return _displayProjectFileExtensions;
            }
        }

        public string DefaultProjectExtension
        {
            get
            {
                return _defaultProjectExtension;
            }
        }

        public string PossibleProjectExtensions
        {
            get
            {
                return _possibleProjectExtensions;
            }
        }

        public string ProjectTemplatesDirectory
        {
            get
            {
                return _projectTemplatesDirectory;
            }
        }

        public string FolderGuid
        {
            get { return _folderGuid.ToString("B"); }
            set { _folderGuid = new Guid(value); }
        }

        public string LanguageVsTemplate
        {
            get { return _languageVsTemplate; }
            set { _languageVsTemplate = value; }
        }

        public string DisplayProjectTypeVsTemplate
        {
            get { return _displayProjectTypeVsTemplate; }
            set { _displayProjectTypeVsTemplate = value; }
        }

        public string ProjectSubTypeVsTemplate
        {
            get { return _projectSubTypeVsTemplate; }
            set { _projectSubTypeVsTemplate = value; }
        }

        public bool NewProjectRequireNewFolderVsTemplate
        {
            get { return _newProjectRequireNewFolderVsTemplate; }
            set { _newProjectRequireNewFolderVsTemplate = value; }
        }

        public bool ShowOnlySpecifiedTemplatesVsTemplate
        {
            get { return _showOnlySpecifiedTemplatesVsTemplate; }
            set { _showOnlySpecifiedTemplatesVsTemplate = value; }
        }

        public string TemplateGroupIDsVsTemplate
        {
            get { return _templateGroupIDsVsTemplate; }
            set { _templateGroupIDsVsTemplate = value; }
        }

        public string TemplateIDsVsTemplate
        {
            get { return _templateIDsVsTemplate; }
            set { _templateIDsVsTemplate = value; }
        }

        private string ProjectRegKey
        {
            get { return string.Format(CultureInfo.InvariantCulture, "Projects\\{0}", FactoryType.GUID.ToString("B")); }
        }

        private string NewPrjTemplateRegKey(RegistrationContext context)
        {
            return string.Format(CultureInfo.InvariantCulture, "NewProjectTemplates\\TemplateDirs\\{0}\\/1", context.ComponentType.GUID.ToString("B"));
        }

        public override void Register(RegistrationContext context)
        {
            //context.Log.WriteLine(SR.GetString(SR.Reg_NotifyProjectFactory, FactoryType.Name));

            using (Key projectKey = context.CreateKey(ProjectRegKey))
            {
                projectKey.SetValue(string.Empty, Name);
                if (_displayName != null)
                    projectKey.SetValue("DisplayName", _displayName);
                if (_displayProjectFileExtensions != null)
                    projectKey.SetValue("DisplayProjectFileExtensions", _displayProjectFileExtensions);
                projectKey.SetValue("Package", context.ComponentType.GUID.ToString("B"));
                if (_defaultProjectExtension != null)
                    projectKey.SetValue("DefaultProjectExtension", _defaultProjectExtension);
                if (_possibleProjectExtensions != null)
                    projectKey.SetValue("PossibleProjectExtensions", _possibleProjectExtensions);
                if (_projectTemplatesDirectory != null)
                {
                    if (!System.IO.Path.IsPathRooted(_projectTemplatesDirectory))
                    {
                        // If path is not rooted, make it relative to package path
                        _projectTemplatesDirectory = System.IO.Path.Combine(context.ComponentPath, _projectTemplatesDirectory);
                    }
                    projectKey.SetValue("ProjectTemplatesDir", context.EscapePath(_projectTemplatesDirectory));
                }

                // VsTemplate Specific Keys
                //
                if (_languageVsTemplate != null)
                    projectKey.SetValue("Language(VsTemplate)", _languageVsTemplate);

                if (_showOnlySpecifiedTemplatesVsTemplate || _templateGroupIDsVsTemplate != null || _templateIDsVsTemplate != null)
                {
                    int showOnlySpecifiedTemplatesVsTemplate = _showOnlySpecifiedTemplatesVsTemplate ? 1 : 0;
                    projectKey.SetValue("ShowOnlySpecifiedTemplates(VsTemplate)", showOnlySpecifiedTemplatesVsTemplate);
                }

                if (_templateGroupIDsVsTemplate != null)
                    projectKey.SetValue("TemplateGroupIDs(VsTemplate)", _templateGroupIDsVsTemplate);

                if (_templateIDsVsTemplate != null)
                    projectKey.SetValue("TemplateIDs(VsTemplate)", _templateIDsVsTemplate);

                if (_displayProjectTypeVsTemplate != null)
                    projectKey.SetValue("DisplayProjectType(VsTemplate)", _displayProjectTypeVsTemplate);

                if (_projectSubTypeVsTemplate != null)
                    projectKey.SetValue("ProjectSubType(VsTemplate)", _projectSubTypeVsTemplate);

                if (_newProjectRequireNewFolderVsTemplate)
                    projectKey.SetValue("NewProjectRequireNewFolder(VsTemplate)", (int)1);
            }
        }

        public override void Unregister(RegistrationContext context)
        {
            context.RemoveKey(ProjectRegKey);
            context.RemoveKey(NewPrjTemplateRegKey(context));
        }
    }
}
