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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xaml;
using System.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.VisualStudioTools.Wpf;

namespace Microsoft.VisualStudioTools.BuildTasks {
    public class ExtractLambdasFromXaml : Task {
        private struct LambdaInfo {
            public string Code { get; set; }
            public int LineNumber { get; set; }
        }

        private static readonly string ToolName = typeof(ExtractLambdasFromXaml).Assembly.ManifestModule.Name;

        [Required]
        public ITaskItem InputFileName { get; set; }

        [Required]
        [Output]
        public ITaskItem OutputFileName { get; set; }

        public string Language { get; set; }

        private CodeDomProvider codeDomProvider;

        private TypeConverter typeAttributesConverter;

        private string className;

        private int classNameLineNumber;

        private string classModifier;

        private int classModifierLineNumber;

        private readonly List<LambdaInfo> lambdas = new List<LambdaInfo>();

        private int importedNamespacesLineNumber = 0;

        private readonly List<string> importedNamespaces = new List<string>
        {
            "System",
            "System.Collections",
            "System.Collections.Generic",
            "System.Linq",
            "System.Text",
            "System.Windows",
            "System.Windows.Controls",
            "System.Windows.Input",
            "System.Windows.Media",
            "System.Windows.Navigation"
        };

        public ExtractLambdasFromXaml() {
            Language = "CSharp";
        }

        public override bool Execute() {
            try {
                try {
                    codeDomProvider = CodeDomProvider.CreateProvider(Language);
                } catch (ConfigurationErrorsException) {
                    LogError(classNameLineNumber, 1, "CodeDom provider for language '" + Language + "' not found.");
                    return false;
                }

                typeAttributesConverter = codeDomProvider.GetConverter(typeof(TypeAttributes));

                if (!ParseInput()) {
                    return false;
                }

                if (lambdas.Count == 0) {
                    OutputFileName = null;
                    return true;
                }

                if (className == null) {
                    LogError(classNameLineNumber, 1501, "x:Class not found on root element.");
                    return false;
                }
                if (!className.Contains(".")) {
                    LogError(classNameLineNumber, 1502, "x:Class does not include namespace name.");
                    return false;
                }

                return GenerateOutput();
            } catch (Exception ex) {
                LogError(null, 0, 0, ex.Message);
                return false;
            }
        }

        private void LogError(string source, int line, int code, string text) {
            string xalCode = string.Format("XAL{0:D4}", code);

            if (source == null) {
                source = ToolName;
            }

            if (BuildEngine != null) {
                Log.LogError(null, xalCode, null, source, line, 0, 0, 0, text);
            } else {
                string pos = (line != 0) ? ("(" + line + ")") : " ";
                Console.Error.WriteLine("{0}{1}: error {2}: {3}", source, pos, xalCode, text);
            }
        }

        private void LogError(int code, string text) {
            LogError(InputFileName.ItemSpec, 0, code, text);
        }

        private void LogError(int line, int code, string text) {
            LogError(InputFileName.ItemSpec, line, code, text);
        }

        private bool ParseInput() {
            XamlXmlReader reader = null;
            try {
                try {
                    reader = new XamlXmlReader(InputFileName.ItemSpec, new XamlXmlReaderSettings { ProvideLineInfo = true });
                } catch (FileNotFoundException ex) {
                    LogError(1001, ex.Message);
                    return false;
                }

                bool classNameExpected = false, classModifierExpected = false;
                bool lambdaBodyExpected = false, importedNamespacesExpected = false;
                int nestingLevel = 0, lambdaNestingLevel = -1;
                while (reader.Read()) {
                    switch (reader.NodeType) {
                        case XamlNodeType.GetObject:
                            ++nestingLevel;
                            break;

                        case XamlNodeType.StartObject:
                            ++nestingLevel;
                            if (nestingLevel == 1) {
                                classNameLineNumber = reader.LineNumber;
                            }
                            if ((reader.Type.Name == "Lambda" || reader.Type.Name == "LambdaExtension") && IsLambdaNamespace(reader.Type.PreferredXamlNamespace)) {
                                lambdaNestingLevel = nestingLevel;
                            }
                            break;

                        case XamlNodeType.EndObject:
                            --nestingLevel;
                            if (nestingLevel < lambdaNestingLevel) {
                                lambdaNestingLevel = -1;
                            }
                            break;

                        case XamlNodeType.StartMember:
                            if (nestingLevel == 1) {
                                if (reader.Member.PreferredXamlNamespace == XamlLanguage.Xaml2006Namespace) {
                                    switch (reader.Member.Name) {
                                        case "Class": {
                                                classNameExpected = true;
                                            } break;

                                        case "ClassModifier": {
                                                classModifierExpected = true;
                                            } break;
                                    }
                                } else if (reader.Member.DeclaringType != null && IsLambdaNamespace(reader.Member.DeclaringType.PreferredXamlNamespace)) {
                                    if (reader.Member.Name == "ImportedNamespaces" && reader.Member.DeclaringType.Name == "LambdaProperties") {
                                        importedNamespacesExpected = true;
                                    }
                                }
                            } else if (nestingLevel == lambdaNestingLevel) {
                                if (reader.Member == XamlLanguage.UnknownContent || reader.Member == XamlLanguage.PositionalParameters || reader.Member.Name == "Lambda") {
                                    lambdaBodyExpected = true;
                                }
                            }
                            break;

                        case XamlNodeType.EndMember:
                            classNameExpected = lambdaBodyExpected = importedNamespacesExpected = false;
                            break;

                        case XamlNodeType.Value:
                            if (classNameExpected) {
                                classNameExpected = false;
                                className = (string)reader.Value;
                                classNameLineNumber = reader.LineNumber;
                            } else if (classModifierExpected) {
                                classModifierExpected = false;
                                classModifier = (string)reader.Value;
                                classModifierLineNumber = reader.LineNumber;
                            } else if (importedNamespacesExpected) {
                                importedNamespaces.Clear();
                                importedNamespaces.AddRange(((string)reader.Value).Split(" \f\n\r\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                                importedNamespacesLineNumber = reader.LineNumber;
                            } else if (lambdaBodyExpected) {
                                lambdas.Add(new LambdaInfo { Code = (string)reader.Value, LineNumber = reader.LineNumber });
                            }
                            break;
                    }
                }
            } catch (IOException ex) {
                LogError(1002, ex.Message);
                return false;
            } catch (XmlException ex) {
                LogError(1003, ex.Message);
                return false;
            } catch (XamlException ex) {
                LogError(1004, ex.Message);
                return false;
            } finally {
                if (reader != null) {
                    reader.Close();
                }
            }

            return true;
        }

        private bool GenerateOutput() {
            string shortClassName, classNamespace;
            int dot = className.LastIndexOf('.');
            if (dot >= 0) {
                classNamespace = className.Substring(0, dot);
                shortClassName = className.Substring(dot + 1);
            } else {
                classNamespace = null;
                shortClassName = className;
            }

            bool isPrivate = false;
            if (classModifier != null) {
                string publicModifier = null, privateModifier = null;
                if (typeAttributesConverter != null || typeAttributesConverter.CanConvertTo(typeof(string))) {
                    try {
                        publicModifier = typeAttributesConverter.ConvertTo(TypeAttributes.Public, typeof(string)) as string;
                        privateModifier = typeAttributesConverter.ConvertTo(TypeAttributes.NotPublic, typeof(string)) as string;
                    } catch (NotSupportedException) {
                    }
                }

                if (string.Equals(classModifier, privateModifier, StringComparison.OrdinalIgnoreCase)) {
                    isPrivate = true;
                } else if (!string.Equals(classModifier, publicModifier, StringComparison.OrdinalIgnoreCase)) {
                    LogError(classModifierLineNumber, 1503, "Language '" + Language + "' does not support x:ClassModifier '" + classModifier + "'.");
                    return false;
                }
            }

            var unit = new CodeCompileUnit();

            var ns = new CodeNamespace(classNamespace);
            unit.Namespaces.Add(ns);
            foreach (string importName in importedNamespaces) {
                var import = new CodeNamespaceImport(importName);
                if (importedNamespacesLineNumber != 0) {
                    import.LinePragma = new CodeLinePragma(InputFileName.ItemSpec, importedNamespacesLineNumber);
                }
                ns.Imports.Add(import);
            }
            var type = new CodeTypeDeclaration {
                Name = shortClassName,
                IsPartial = true,
                BaseTypes = { typeof(ILambdaConverterProvider) }
            };
            ns.Types.Add(type);
            if (isPrivate) {
                type.TypeAttributes &= ~TypeAttributes.Public;
            }

            var method = new CodeMemberMethod {
                Name = "GetConverterForLambda",
                PrivateImplementationType = new CodeTypeReference(typeof(ILambdaConverterProvider)),
                ReturnType = new CodeTypeReference(typeof(LambdaConverter)),
                Parameters =
                {   
                    new CodeParameterDeclarationExpression
                    {
                        Name = "lambda__",
                        Type = new CodeTypeReference(typeof(string))
                    }
                },
                CustomAttributes =
                {
                    new CodeAttributeDeclaration(new CodeTypeReference(typeof(GeneratedCodeAttribute)))
                    {
                        Arguments = 
                        {
                            new CodeAttributeArgument(new CodePrimitiveExpression(ToolName)),
                            new CodeAttributeArgument(new CodePrimitiveExpression(typeof(ExtractLambdasFromXaml).Assembly.GetName().Version.ToString()))
                        }
                    }
                }
            };
            type.Members.Add(method);

            foreach (var lambda in lambdas) {
                var cond = new CodeConditionStatement {
                    Condition = new CodeBinaryOperatorExpression {
                        Operator = CodeBinaryOperatorType.ValueEquality,
                        Left = new CodeArgumentReferenceExpression("lambda__"),
                        Right = new CodePrimitiveExpression(lambda.Code)
                    },
                    TrueStatements =
                    {
                        new CodeMethodReturnStatement
                        {
                            Expression = new CodeMethodInvokeExpression
                            {
                                Method = new CodeMethodReferenceExpression
                                {
                                    TargetObject = new CodeTypeReferenceExpression(typeof(LambdaConverter)),
                                    MethodName = "Create"
                                },
                                Parameters =
                                {
                                    new CodeSnippetExpression(lambda.Code)
                                }
                            },
                            LinePragma = new CodeLinePragma
                            {
                                FileName = InputFileName.ItemSpec,
                                LineNumber = lambda.LineNumber
                            }
                        }
                    }
                };

                method.Statements.Add(cond);
            }

            method.Statements.Add(
                new CodeThrowExceptionStatement {
                    ToThrow = new CodeObjectCreateExpression {
                        CreateType = new CodeTypeReference(typeof(ArgumentOutOfRangeException)),
                        Parameters =
                        {
                            new CodePrimitiveExpression("lambda__")
                        }
                    }
                });

            try {
                using (var writer = File.CreateText(OutputFileName.ItemSpec)) {
                    var options = new CodeGeneratorOptions();
                    codeDomProvider.GenerateCodeFromCompileUnit(unit, writer, options);
                }
            } catch (IOException ex) {
                LogError(2002, ex.Message);
                return false;
            }

            return true;
        }

        private static readonly string LambdaConverterClrNamespace = "clr-namespace:" + typeof(LambdaConverter).Namespace;
        private static readonly string LambdaConverterClrNamespaceWithAssembly = LambdaConverterClrNamespace + ";assembly=";

        private static bool IsLambdaNamespace(string ns) {
            return ns != null && (ns == LambdaConverterClrNamespace || ns.StartsWith(LambdaConverterClrNamespaceWithAssembly));
        }

#if CONSOLE_APP
        internal static int Main(string[] args) {
            var task = new ExtractLambdasFromXaml();

            bool argsError = false;
            foreach (string arg in args) {
                if (arg.StartsWith("/") || arg.StartsWith("-")) {
                    string[] opt = arg.Substring(1).Split(new[] { ':' }, 2);
                    string optName = opt[0].ToLowerInvariant();
                    string optArg = opt.Length == 2 ? opt[1] : null;
                    switch (optName) {
                        case "language": {
                                if (optArg != null) {
                                    task.Language = optArg;
                                } else {
                                    argsError = true;
                                }
                            } break;
                        default: {
                                argsError = true;
                            } break;
                    }
                } else if (task.InputFileName == null) {
                    task.InputFileName = new TaskItem(arg);
                } else if (task.OutputFileName == null) {
                    task.OutputFileName = new TaskItem(arg);
                } else {
                    argsError = true;
                }
            }

            if (argsError || task.InputFileName == null || task.OutputFileName == null) {
                Console.Error.WriteLine("Usage: {0} <input-file> <output-file> [/language:<language>]", ToolName);
                return 1;
            }

            return task.Execute() ? 0 : 1;
        }
#endif
    }
}