ImplFile
  (ParsedImplFileInput
     ("/root/Type/Enum 10 - Eof.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [E],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  Simple
                    (Enum
                       ([SynEnumCase
                           ([], SynIdent (A, None),
                            ArbitraryAfterError
                              ("firstUnionCaseDecl", (4,7--4,7)),
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            (4,4--4,7), { BarRange = None
                                          EqualsRange = (4,6--4,7) })],
                        (4,4--4,7)), (4,4--4,7)), [], None, (3,5--4,7),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--4,7))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,7), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,0)-(4,7) parse error Incomplete structured construct at or before this point in type definition
