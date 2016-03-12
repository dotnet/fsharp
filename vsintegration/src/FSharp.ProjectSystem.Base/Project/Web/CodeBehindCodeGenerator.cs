// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Designer.Interfaces;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Design;
using Microsoft.VisualStudio.Shell.Design.Serialization;

using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IVsCodeBehindCodeGenerator = Microsoft.VisualStudio.Web.Application.IVsCodeBehindCodeGenerator;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem.Web
{
	[ComVisible(true)]
	internal class CodeBehindCodeGenerator : IVsCodeBehindCodeGenerator
	{
		private ServiceProvider _serviceProvider;
		private IVsHierarchy _hierarchy;
		private CodeGeneratorOptions _codeGeneratorOptions;

		// Generate state
		private VsHierarchyItem _itemCode;
		private VsHierarchyItem _itemDesigner;
		private CodeDomProvider _codeDomProvider;
		private CodeCompileUnit _ccu;
		private CodeTypeDeclaration _ctd;
		private bool _create;
		private bool _isPartialClassDisabled;
		private FieldDataDictionary _codeFields;
		private FieldDataDictionary _designerFields;
		private string _className_Full;
		private string _className_Namespace;
		private string _className_Name;

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Constructor 
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public CodeBehindCodeGenerator()
		{
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Finalizer
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		~CodeBehindCodeGenerator()
		{
			Debug.Fail("CodeBehindCodeGenerator was not disposed.");
			Dispose();
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Initializes the generator state.
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		void IVsCodeBehindCodeGenerator.Initialize(IVsHierarchy hierarchy)
		{
			IOleServiceProvider serviceProvider = null;
			_hierarchy = hierarchy;
			_hierarchy.GetSite(out serviceProvider);
			_serviceProvider = new ServiceProvider(serviceProvider);
			_codeGeneratorOptions = new CodeGeneratorOptions();
			_codeGeneratorOptions.BlankLinesBetweenMembers = false;
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Cleans up state.
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		void IVsCodeBehindCodeGenerator.Close()
		{
			Dispose();
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Cleans up member state.
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public virtual void Dispose()
		{
			if (_serviceProvider != null)
			{
				_serviceProvider.Dispose();
				_serviceProvider = null;
			}

			_hierarchy = null;
			_codeGeneratorOptions = null;

			DisposeGenerateState();

			GC.SuppressFinalize(this);
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Cleans up Generate member state.
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ virtual void DisposeGenerateState()
		{
			try
			{
				_itemCode = null;
				_itemDesigner = null;
				_ccu = null;
				_ctd = null;
				_create = false;
				_codeFields = null;
				_designerFields = null;
				_className_Full = null;
				_className_Namespace = null;
				_className_Name = null;

				if (_codeDomProvider != null)
				{
					_codeDomProvider.Dispose();
					_codeDomProvider = null;
				}
			}
			catch
			{
			}
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 The full class name currently generating for
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ string ClassName_Full
		{
			get
			{
				return _className_Full;
			}
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 The class namespace currently generating for
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ string ClassName_Namespace
		{
			get
			{
				return _className_Namespace;
			}
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 The class name currently generating for
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ string ClassName_Name
		{
			get
			{
				return _className_Name;
			}
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 The code behind item
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ VsHierarchyItem ItemCode
		{
			get
			{
				return _itemCode;
			}
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Languages that do NOT support partial class in a .designer file should disable partial classes
		///	 using this property.
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ bool IsPartialClassDisabled
		{
			get
			{
				return _isPartialClassDisabled;
			}
			set
			{
				_isPartialClassDisabled = value;
			}
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Create CodeDomProvider for the language of the file
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ virtual CodeDomProvider CreateCodeDomProvider(uint itemid)
		{
			Microsoft.VisualStudio.OLE.Interop.IServiceProvider oleServiceProvider;
			ErrorHandler.ThrowOnFailure(((IVsProject)this._hierarchy).GetItemContext(itemid, out oleServiceProvider));
			ServiceProvider serviceProvider = new ServiceProvider(oleServiceProvider);
			IVSMDCodeDomProvider provider = serviceProvider.GetService(typeof(SVSMDCodeDomProvider)) as IVSMDCodeDomProvider;
			CodeDomProvider codeDomProvider = null;
			if (provider != null)
			{
				codeDomProvider = provider.CodeDomProvider as CodeDomProvider;
			}

			Debug.Assert(codeDomProvider!=null, "Failed to create CodeDomProvider");
			return codeDomProvider;
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Returns public field names in dictionary
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ FieldDataDictionary GetFieldNames(string[] publicFields, bool caseSensitive)
		{
			FieldDataDictionary fields = null;

			if (publicFields != null)
			{
				foreach (string name in publicFields)
				{
					if (!string.IsNullOrEmpty(name))
					{
						FieldData field = new FieldData(name);

						if (field != null)
						{
							if (fields == null)
							{
								fields = new FieldDataDictionary(caseSensitive);
							}

							try
							{
								if (!fields.ContainsKey(field.Name))
								{
									fields.Add(field.Name, field);
								}
							}
							catch
							{
							}
						}
					}
				}
			}

			return fields;
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Returns field names in the specified class using code model.
		///	 If publicOnly is true only public fields are returned.
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ FieldDataDictionary GetFieldNames(VsHierarchyItem itemCode, string className, bool caseSensitive, bool onlyBaseClasses, int maxDepth)
		{
			FieldDataDictionary fields = null;

			if (itemCode != null)
			{
				CodeClass codeClass = FindClass(itemCode, className);
				if (codeClass != null)
				{
					GetFieldNames(codeClass, caseSensitive, onlyBaseClasses, 0, maxDepth, ref fields);
				}
			}

			return fields;
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Returns field names in the specified class using code model.
		///	 If publicOnly is true only public fields are returned.
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ void GetFieldNames(CodeClass codeClass, bool caseSensitive, bool onlyBaseClasses, int depth, int maxDepth, ref FieldDataDictionary fields)
		{
			if (codeClass != null && depth <= maxDepth)
			{
				if (!(onlyBaseClasses && depth == 0))
				{
					foreach (CodeElement codeElement in codeClass.Members)
					{
						//vsCMElement kind = codeElement.Kind; //vsCMElementVariable
						CodeVariable codeVariable = codeElement as CodeVariable;
						if (codeVariable != null)
						{
							FieldData field = new FieldData(codeClass, codeVariable, depth);

							if (field != null && !string.IsNullOrEmpty(field.Name))
							{
								if (fields == null)
								{
									fields = new FieldDataDictionary(caseSensitive);
								}

								try
								{
									if (!fields.ContainsKey(field.Name))
									{
										fields.Add(field.Name, field);
									}
								}
								catch
								{
								}
							}
						}
					}
				}

				foreach (CodeElement baseCodeElement in codeClass.Bases)
				{
					CodeClass baseCodeClass = baseCodeElement as CodeClass;
					if (baseCodeClass != null)
					{
						CodeElements partCodeElements = null;
						CodeClass2 baseCodeClass2 = baseCodeClass as CodeClass2;
						if (baseCodeClass2 != null)
						{
							vsCMClassKind classKind = baseCodeClass2.ClassKind;
							if ((classKind | vsCMClassKind.vsCMClassKindPartialClass) == vsCMClassKind.vsCMClassKindPartialClass)
							{
								try
								{
									partCodeElements = baseCodeClass2.Parts;
								}
								catch
								{
								}
							}
						}

						if (partCodeElements != null && partCodeElements.Count > 1)
						{
							foreach (CodeElement partCodeElement in partCodeElements)
							{
								CodeClass partCodeClass = partCodeElement as CodeClass;
								if (partCodeClass != null)
								{
									GetFieldNames(partCodeClass, caseSensitive, onlyBaseClasses, depth + 1, maxDepth, ref fields);
								}
							}
						}
						else
						{
							GetFieldNames(baseCodeClass, caseSensitive, onlyBaseClasses, depth + 1, maxDepth, ref fields);
						}
					}
				}
			}
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Locates the code model CodeClass 
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ virtual CodeClass FindClass(VsHierarchyItem item, string className)
		{
			if (item != null)
			{
				try
				{
					ProjectItem projectItem = item.ProjectItem();
					if (projectItem != null)
					{
						FileCodeModel fileCodeModel = projectItem.FileCodeModel;
						if (fileCodeModel != null)
						{
							return FindClass(fileCodeModel.CodeElements, className);
						}
					}
				}
				catch
				{
				}
			}
			return null;
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Recursively searches the CodeElements for the specified class
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ virtual CodeClass FindClass(CodeElements codeElements, string className)
		{
			if (codeElements != null && !string.IsNullOrEmpty(className))
			{
				foreach (CodeElement codeElement in codeElements)
				{
					vsCMElement kind = codeElement.Kind;
					if (kind == vsCMElement.vsCMElementClass)
					{
						CodeClass codeClass = codeElement as CodeClass;
						if (codeClass != null && string.Compare(codeClass.FullName, className, StringComparison.Ordinal) == 0)
						{
							return codeClass;
						}
					}
					else if (kind == vsCMElement.vsCMElementNamespace)
					{
						EnvDTE.CodeNamespace codeNamespace = codeElement as EnvDTE.CodeNamespace;
						if (codeNamespace != null)
						{
							CodeClass codeClass = FindClass(codeNamespace.Children, className);
							if (codeClass != null)
							{
								return codeClass;
							}
						}
					}
				}
			}
			return null;
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Gets a VshierarchyItem for the designer file if possible.
		///	 Will create new file if specified and not existing.
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ virtual VsHierarchyItem GetDesignerItem(VsHierarchyItem itemCode, bool create)
		{
			VsHierarchyItem itemDesigner = null;

			if (itemCode != null)
			{
				// Calculate codebehind and designer file paths 
				string codeBehindFile = itemCode.FullPath();
				string designerFile = null;
				if (_isPartialClassDisabled)
				{
					designerFile = codeBehindFile;
				}
				else if (!string.IsNullOrEmpty(codeBehindFile))
				{
					designerFile = codeBehindFile.Insert(codeBehindFile.LastIndexOf("."), ".designer");
				}

				// Try to locate existing designer file
				if (!string.IsNullOrEmpty(designerFile))
				{
					itemDesigner = VsHierarchyItem.CreateFromMoniker(designerFile, _hierarchy);
					if (itemDesigner != null)
					{
						return itemDesigner;
					}
				}

				// Create empty designer file if requested
				if (create && !string.IsNullOrEmpty(designerFile))
				{
					ProjectItem projectItemCode = itemCode.ProjectItem();
					if (projectItemCode != null)
					{
						ProjectItems projectItems = projectItemCode.Collection;
						if (projectItems != null)
						{
							try
							{
								using (StreamWriter sw = File.CreateText(designerFile))
								{
									sw.WriteLine(" ");
								}

								projectItems.AddFromFileCopy(designerFile);
							}
							catch
							{
							}

							itemDesigner = VsHierarchyItem.CreateFromMoniker(designerFile, _hierarchy);
							if (itemDesigner != null)
							{
								return itemDesigner;
							}
						}
					}
				}
			}

			return itemDesigner;
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Checks CodeDomProvider for case sensitivity
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ bool IsCaseSensitive(CodeDomProvider codeDomProvider)
		{
			return !((codeDomProvider.LanguageOptions & LanguageOptions.CaseInsensitive) == LanguageOptions.CaseInsensitive);
		}


		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		bool IVsCodeBehindCodeGenerator.IsGenerateAllowed(string document, string codeBehindFile, bool create)
		{
			VsHierarchyItem itemCode = VsHierarchyItem.CreateFromMoniker(codeBehindFile, _hierarchy);
			VsHierarchyItem itemDesigner = GetDesignerItem(itemCode, false);

			if ((itemDesigner != null && itemDesigner.IsBuildActionCompile())
				|| (itemDesigner == null && create))
			{
				return true;
			}

			return false;
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		void IVsCodeBehindCodeGenerator.BeginGenerate(string document, string codeBehindFile, string className_Full, bool create)
		{
			DisposeGenerateState();

			_itemCode = VsHierarchyItem.CreateFromMoniker(codeBehindFile, _hierarchy);
			_itemDesigner = GetDesignerItem(_itemCode, false);
			_create = create;
			_className_Full = className_Full;

			if (_itemCode != null)
			{
				// Get the CodeDomProvider for the language (MergedCodeDomProvider C#/VB)
				_codeDomProvider = CreateCodeDomProvider(_itemCode.VsItemID);

				if (_codeDomProvider != null)
				{
					// Get the field names so we can preserve location and access
					bool caseSensitive = IsCaseSensitive(_codeDomProvider);

					_codeFields = GetFieldNames(_itemCode, _className_Full, caseSensitive, false, 30);
					_designerFields = GetFieldNames(_itemDesigner, _className_Full, caseSensitive, false, 0);

					// Generate the class
					string designerContents = _itemDesigner.GetDocumentText();
					TextReader reader = new StringReader(designerContents);
					_ccu = _codeDomProvider.Parse(reader);
					GenerateClass();
				}
			}
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		private void GenerateClass()
		{
			// Break full name into namespace and name
			if (!string.IsNullOrEmpty(_className_Full))
			{
				int lastdot = _className_Full.LastIndexOf('.');
				if (lastdot >= 0)
				{
					_className_Name = _className_Full.Substring(lastdot + 1);
					_className_Namespace = _className_Full.Substring(0, lastdot);
				}
				else
				{
					_className_Name = _className_Full;
					_className_Namespace = null;
				}
			}


			// Create namespace and add to compile unit
			System.CodeDom.CodeNamespace codeNS = null;
			string ns = GetClassNamespace();
			if (!string.IsNullOrEmpty(ns))
			{
				codeNS = new System.CodeDom.CodeNamespace(ns);
			}
			else
			{
				codeNS = new System.CodeDom.CodeNamespace();
			}
			if (_ccu.Namespaces != null && _ccu.Namespaces.Count>0 && String.CompareOrdinal(_ccu.Namespaces[0].Name, ns) == 0)
				codeNS = _ccu.Namespaces[0];
			else
				_ccu.Namespaces.Add(codeNS);

			// Create class definition and add to namespace
			_ctd = new CodeTypeDeclaration(_className_Name);
			_ctd.IsPartial = !IsPartialClassDisabled;
			if (codeNS.Types != null && codeNS.Types.Count > 0 && String.CompareOrdinal(codeNS.Types[0].Name, _className_Name) == 0)
				_ctd = codeNS.Types[0];
			else
				codeNS.Types.Add(_ctd);
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		void IVsCodeBehindCodeGenerator.EnsureStronglyTypedProperty(string propertyName, string propertyTypeName)
		{
			CodeMemberProperty prop = new CodeMemberProperty();
			prop.Attributes &= ~MemberAttributes.AccessMask;
			prop.Attributes &= ~MemberAttributes.ScopeMask;
			prop.Attributes |= MemberAttributes.Final | MemberAttributes.New | MemberAttributes.Public;
			prop.Name = propertyName;
			prop.Type = new CodeTypeReference(propertyTypeName);

			CodePropertyReferenceExpression propRef = new CodePropertyReferenceExpression(
				new CodeBaseReferenceExpression(), propertyName);

			prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeCastExpression(propertyTypeName, propRef)));
			_ctd.Members.Add(prop);
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		void IVsCodeBehindCodeGenerator.EnsureControlDeclaration(string name, string typeName)
		{
			if (ShouldDeclareField(name, typeName))
			{
				typeName = GetDeclarationTypeName(name, typeName);

				CodeMemberField field = new CodeMemberField(typeName, name);

				// Check if someone made the declaration public in the designer file
				bool isPublic = false;
				if (_designerFields != null && _designerFields.ContainsKey(name))
				{
					FieldData fieldData = _designerFields[name];
					if (fieldData != null)
					{
						isPublic = fieldData.IsPublic;
					}
				}

				// Set access to public or protected
				field.Attributes &= ~MemberAttributes.AccessMask;
				if (isPublic)
				{
					field.Attributes |= MemberAttributes.Public;
				}
				else
				{
					field.Attributes |= MemberAttributes.Family;
				}

				SetAdditionalFieldData(field);

				_ctd.Members.Add(field);
			}
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		void IVsCodeBehindCodeGenerator.Generate()
		{
			DocData ddDesigner = null;
			DocDataTextWriter designerWriter = null;

			try
			{
				if (_itemCode != null && _codeDomProvider != null)
				{

					// Generate the code
					StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
					_codeDomProvider.GenerateCodeFromCompileUnit(_ccu, stringWriter, _codeGeneratorOptions);
					string generatedCode = stringWriter.ToString();

					// Create designer file if requested
					if (_itemDesigner == null && _create)
					{
						_itemDesigner = GetDesignerItem(_itemCode, true);
					}

					// See if generated code changed
					string designerContents = _itemDesigner.GetDocumentText();
					if (!BufferEquals(designerContents, generatedCode))  // Would be nice to just compare lengths but the buffer gets formatted after insertion
					{
						ddDesigner = new LockedDocData(_serviceProvider, _itemDesigner.FullPath());

						// Try to check out designer file (this throws)
						ddDesigner.CheckoutFile(_serviceProvider);

						// Write out the new code
						designerWriter = new DocDataTextWriter(ddDesigner);
						designerWriter.Write(generatedCode);
						designerWriter.Flush();
						designerWriter.Close();
					}
				}
			}
			finally
			{
				if (designerWriter != null)
				{
					designerWriter.Dispose();
				}
				if (ddDesigner != null)
				{
					ddDesigner.Dispose();
				}
			}
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Virtual method to allow language specific determination
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ virtual bool ShouldDeclareField(string name, string typeName)
		{
			// Don't add field if already defined in codebehind or exposed from base class
			bool declareField = true;
			if (_codeFields != null && _codeFields.ContainsKey(name))
			{
				FieldData fieldData = _codeFields[name];
				if (fieldData != null)
				{
					if (fieldData.Depth == 0)
					{
						// For immediate class we don't re-declare regardless
						// of access modifiers.  If the field is not visible to the run-time
						// it will not be set and code against it will fail.
						declareField = false;
					}
					else if (fieldData.IsProtected || fieldData.IsPublic)
					{
						// For bases we do not re-declare if already accessible
						// (internal, private are not accesible to page assembly)
						declareField = false;
					}
				}
			}

			return declareField;
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Virtual method to allow language specific adjustments
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ virtual string GetDeclarationTypeName(string name, string typeName)
		{
			return typeName;
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Virtual method to allow language specific adjustment of root classname namespace
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ virtual string GetClassNamespace()
		{
			return _className_Namespace;
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Virtual method to allow language specific adjustment of field
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ virtual void SetAdditionalFieldData(CodeMemberField field)
		{
			return;
		}

		///-------------------------------------------------------------------------------------------------------------
		/// <summary>
		///	 Compare buffers ignoring whitespace
		/// </summary>
		///-------------------------------------------------------------------------------------------------------------
		public /*protected, but public for FSharp.Project.dll*/ bool BufferEquals(string str1, string str2)
		{
			int i1 = 0;
			int i2 = 0;
			int len1 = str1.Length;
			int len2 = str2.Length;

			for (; ; )
			{
				while (i1 < len1 && char.IsWhiteSpace(str1[i1]))
					i1++;

				while (i2 < len2 && char.IsWhiteSpace(str2[i2]))
					i2++;

				if (i1 >= len1)
				{
					if (i2 >= len2)
					{
						return true; // ended with whitespace
					}
					return false;	// str1 ended before str2
				}
				else if (i2 >= len2)
				{
					return false;	// str2 ended before str1
				}
				else if (str1[i1] != str2[i2])
				{
					return false;	// different chars
				}

				i1++; i2++;		  // advance
			}
		}
	}
}

