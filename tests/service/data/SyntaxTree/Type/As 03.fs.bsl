ImplFile
  (ParsedImplFileInput
     ("/root/Type/As 03.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel (Class, [], (3,17--3,26)), [], None, (3,5--3,26),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,15--3,16)
                    WithKeyword = None })], (3,0--3,26))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,26), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(3,7)-(3,14) parse error This definition may only be used in a type with a primary constructor. Consider adding arguments to your type definition, e.g. 'type X(args) = ...'.
