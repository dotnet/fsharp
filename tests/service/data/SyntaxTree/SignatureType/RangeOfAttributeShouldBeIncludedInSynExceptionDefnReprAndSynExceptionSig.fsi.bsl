SigFile
  (ParsedSigFileInput
     ("/root/SignatureType/RangeOfAttributeShouldBeIncludedInSynExceptionDefnReprAndSynExceptionSig.fsi",
      QualifiedNameOfFile FSharp.Compiler.ParseHelpers, [], [],
      [SynModuleOrNamespaceSig
         ([FSharp; Compiler; ParseHelpers], false, NamedModule,
          [Exception
             (SynExceptionSig
                (SynExceptionDefnRepr
                   ([{ Attributes =
                        [{ TypeName = SynLongIdent ([NoEquality], [], [None])
                           ArgExpr = Const (Unit, (5,2--5,12))
                           Target = None
                           AppliesToGetterAndSetter = false
                           Range = (5,2--5,12) };
                         { TypeName = SynLongIdent ([NoComparison], [], [None])
                           ArgExpr = Const (Unit, (5,14--5,26))
                           Target = None
                           AppliesToGetterAndSetter = false
                           Range = (5,14--5,26) }]
                       Range = (5,0--5,28) }],
                    SynUnionCase
                      ([], SynIdent (SyntaxError, None),
                       Fields
                         [SynField
                            ([], false, None,
                             LongIdent (SynLongIdent ([obj], [], [None])), false,
                             PreXmlDoc ((6,25), FSharp.Compiler.Xml.XmlDocCollector),
                             None, (6,25--6,28), { LeadingKeyword = None });
                          SynField
                            ([], false, Some range,
                             LongIdent (SynLongIdent ([range], [], [None])),
                             false,
                             PreXmlDoc ((6,31), FSharp.Compiler.Xml.XmlDocCollector),
                             None, (6,31--6,43), { LeadingKeyword = None })],
                       PreXmlDocEmpty, None, (6,10--6,43), { BarRange = None }),
                    None, PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                    None, (5,0--6,43)), None, [], (5,0--6,43)), (5,0--6,43))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [],
          Some (Internal (2,7--2,15)), (2,0--6,43),
          { LeadingKeyword = Module (2,0--2,6) })],
      { ConditionalDirectives = []
        CodeComments = [LineComment (4,0--4,90)] }, set []))
