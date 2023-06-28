ImplFile
  (ParsedImplFileInput
     ("/root/Member/Field 04.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [ValField
                        (SynField
                           ([], false, Some , FromParseError (1,13--1,13), false,
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,8--134217728,0),
                            { LeadingKeyword = Some (Val (4,4--4,7)) }),
                         (1,13--4,7));
                      ValField
                        (SynField
                           ([], false, Some F2,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,8--5,15),
                            { LeadingKeyword = Some (Val (5,4--5,7)) }),
                         (5,4--5,15))], (1,13--5,15)), [], None, (1,13--5,15),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (1,13--5,15));
           Expr (Const (Unit, (7,0--7,2)), (7,0--7,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,8)-(5,4) parse error Incomplete structured construct at or before this point in type definition. Expected identifier or other token.
