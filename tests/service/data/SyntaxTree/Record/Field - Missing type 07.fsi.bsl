SigFile
  (ParsedSigFileInput
     ("/root/Record/Field - Missing type 07.fsi", QualifiedNameOfFile Module, [],
      [],
      [SynModuleOrNamespaceSig
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [R],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  Simple
                    (Record
                       (None,
                        [SynField
                           ([], false, Some F1, FromParseError (4,9--4,9), false,
                            PreXmlDoc ((4,6), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,6--4,9), { LeadingKeyword = None });
                         SynField
                           ([{ Attributes =
                                [{ TypeName = SynLongIdent ([A], [], [None])
                                   ArgExpr = Const (Unit, (5,8--5,9))
                                   Target = None
                                   AppliesToGetterAndSetter = false
                                   Range = (5,8--5,9) }]
                               Range = (5,6--5,11) }], false, Some F2,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((5,6), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,6--6,13), { LeadingKeyword = None })],
                        (4,4--6,15)), (4,4--6,15)), [], (3,5--6,15),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--6,15))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,15), { LeadingKeyword = Module (1,0--1,6) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,10)-(5,6) parse error Incomplete structured construct at or before this point in field declaration
