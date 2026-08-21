ImplFile
  (ParsedImplFileInput
     ("/root/Type/Record - Mutable 03.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [R],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  Simple
                    (Record
                       (None,
                        [SynField
                           ([], false, None, FromParseError (5,15--5,15), true,
                            PreXmlDoc ((5,8), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,8--5,15),
                            { LeadingKeyword = None
                              MutableKeyword = Some (5,8--5,15) });
                         SynField
                           ([], false, Some F2,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((6,8), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,8--6,15), { LeadingKeyword = None
                                                 MutableKeyword = None })],
                        (4,4--7,5)), (4,4--7,5)), [], None, (3,5--7,5),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--7,5));
           Expr (Const (Unit, (9,0--9,2)), (9,0--9,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--9,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(5,16)-(6,8) parse error Incomplete structured construct at or before this point in field declaration. Expected identifier or other token.
