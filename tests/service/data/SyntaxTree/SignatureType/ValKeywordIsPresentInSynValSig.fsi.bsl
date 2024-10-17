SigFile
  (ParsedSigFileInput
     ("/root/SignatureType/ValKeywordIsPresentInSynValSig.fsi",
      QualifiedNameOfFile Meh, [], [],
      [SynModuleOrNamespaceSig
         ([Meh], false, NamedModule,
          [Val
             (SynValSig
                ([{ Attributes = [{ TypeName = SynLongIdent ([Foo], [], [None])
                                    ArgExpr = Const (Unit, (4,2--4,5))
                                    Target = None
                                    AppliesToGetterAndSetter = false
                                    Range = (4,2--4,5) }]
                    Range = (4,0--4,7) }], SynIdent (a, None),
                 SynValTyparDecls (None, true),
                 LongIdent (SynLongIdent ([int], [], [None])),
                 SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                 Single None, None, (4,0--6,11),
                 { LeadingKeyword = Val (6,0--6,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange = None }), (4,0--6,11))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--6,11), { LeadingKeyword = Module (2,0--2,6) })],
      { ConditionalDirectives = []
        CodeComments = [LineComment (5,0--5,6)] }, set []))
