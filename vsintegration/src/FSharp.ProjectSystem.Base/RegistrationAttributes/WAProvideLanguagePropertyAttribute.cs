// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.


using System;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Win32;

namespace Microsoft.VisualStudio.Shell
{
    /// <summary>
    /// This class can be used for registering a Web Application Property for a project
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal sealed class WAProvideLanguagePropertyAttribute : RegistrationAttribute
    {
        private Type _languageTemplateFactoryType;
        private string _propertyName;
        private string _propertyValueString;
        private int _propertyValueInt;


        public WAProvideLanguagePropertyAttribute(Type languageTemplateFactoryType, string propertyName, string propertyValue)
        {
            _languageTemplateFactoryType = languageTemplateFactoryType;
            _propertyName = propertyName;
            _propertyValueString = propertyValue;
            _propertyValueInt = 0;
        }

        public WAProvideLanguagePropertyAttribute(Type languageTemplateFactoryType, string propertyName, int propertyValue)
        {
            _languageTemplateFactoryType = languageTemplateFactoryType;
            _propertyName = propertyName;
            _propertyValueString = null;
            _propertyValueInt = propertyValue;
        }

        public WAProvideLanguagePropertyAttribute(Type languageTemplateFactoryType, string propertyName, bool propertyValue)
        {
            _languageTemplateFactoryType = languageTemplateFactoryType;
            _propertyName = propertyName;
            _propertyValueString = null;
            _propertyValueInt = propertyValue ? 1 : 0;
        }

        public WAProvideLanguagePropertyAttribute(Type languageTemplateFactoryType, string propertyName, Type propertyValue)
        {
            _languageTemplateFactoryType = languageTemplateFactoryType;
            _propertyName = propertyName;
            _propertyValueString = propertyValue.GUID.ToString("B");
            _propertyValueInt = 0;
        }

        public Type LanguageTemplateFactoryType
        {
            get
            {
                return _languageTemplateFactoryType;
            }
        }

        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
        }

        public string PropertyValueString
        {
            get
            {
                return _propertyValueString;
            }
        }

        public int PropertyValueInt
        {
            get
            {
                return _propertyValueInt;
            }
        }

        private string LanguagePropertyKey
        {
            get { return string.Format(CultureInfo.InvariantCulture, "Projects\\{0}\\WebApplicationProperties", LanguageTemplateFactoryType.GUID.ToString("B")); }
        }


        public override void Register(RegistrationContext context)
        {
            using (Key propertyKey = context.CreateKey(LanguagePropertyKey))
            {
                if (PropertyValueString != null)
                {
                    propertyKey.SetValue(PropertyName, PropertyValueString);
                }
                else
                {
                    propertyKey.SetValue(PropertyName, PropertyValueInt);
                }
            }
        }

        public override void Unregister(RegistrationContext context)
        {
            context.RemoveKey(LanguagePropertyKey);
        }
    }
}
