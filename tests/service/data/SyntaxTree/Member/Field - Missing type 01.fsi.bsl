SigFile
  (ParsedSigFileInput
     ("/root/Member/Field - Missing type 01.fsi", QualifiedNameOfFile Module, [],
      [],
      [SynModuleOrNamespaceSig
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [ValField
                        (SynField
                           ([], false, Some F1, FromParseError (4,10--4,10),
                            false,
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,4--4,10),
                            { LeadingKeyword = Some (Val (4,4--4,7)) }),
                         (4,4--4,10));
                      ValField
                        (SynField
                           ([], false, Some F2,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,4--5,15),
                            { LeadingKeyword = Some (Val (5,4--5,7)) }),
                         (5,4--5,15))], (4,4--5,15)), [], (3,5--5,15),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--5,15))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,15), { LeadingKeyword = Module (1,0--1,6) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,11)-(5,4) parse error Incomplete structured construct at or before this point in field declaration. Expected ':' or other token.
