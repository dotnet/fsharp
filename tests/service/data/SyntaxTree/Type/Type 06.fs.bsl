ImplFile
  (ParsedImplFileInput
     ("/root/Type/Type 06.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  Simple (None (3,5--3,6), (3,5--3,6)), [], None, (3,5--3,6),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,5--3,6)
                    WithKeyword = None })], (3,0--3,6))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,6), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,0)-(4,0) parse error Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (3:1). Try indenting this further.
To continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.
(3,5)-(3,6) parse error Unexpected symbol '=' in type name
(3,5)-(3,6) parse error A type definition requires one or more members or other declarations. If you intend to define an empty class, struct or interface, then use 'type ... = class end', 'interface end' or 'struct end'.
