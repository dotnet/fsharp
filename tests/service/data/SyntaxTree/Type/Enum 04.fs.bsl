ImplFile
  (ParsedImplFileInput
     ("/root/Type/Enum 04.fs", false, QualifiedNameOfFile Module, [],
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
                              ("attrUnionCaseDecl", (4,9--4,9)),
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            (4,6--4,9), { BarRange = Some (4,4--4,5)
                                          EqualsRange = (4,8--4,9) });
                         SynEnumCase
                           ([], SynIdent (B, None),
                            Const (Int32 2, (5,10--5,11)),
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            (5,6--5,11), { BarRange = Some (5,4--5,5)
                                           EqualsRange = (5,8--5,9) })],
                        (4,4--5,11)), (4,4--5,11)), [], None, (3,5--5,11),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--5,11));
           Expr (Const (Unit, (7,0--7,2)), (7,0--7,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(5,4)-(5,5) parse error Unexpected symbol '|' in union case
