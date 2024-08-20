SigFile
  (ParsedSigFileInput
     ("/root/ModuleOrNamespaceSig/ModuleRangeShouldStartAtFirstAttribute.fsi",
      QualifiedNameOfFile Bar, [], [],
      [SynModuleOrNamespaceSig
         ([Bar], false, NamedModule,
          [Val
             (SynValSig
                ([], SynIdent (s, None), SynValTyparDecls (None, true),
                 LongIdent (SynLongIdent ([string], [], [None])),
                 SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                 Single None, None, (5,0--5,14),
                 { LeadingKeyword = Val (5,0--5,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange = None }), (5,0--5,14))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
          [{ Attributes = [{ TypeName = SynLongIdent ([Foo], [], [None])
                             ArgExpr = Const (Unit, (2,4--2,7))
                             Target = None
                             AppliesToGetterAndSetter = false
                             Range = (2,4--2,7) }]
             Range = (2,0--2,11) }], None, (2,0--5,14),
          { LeadingKeyword = Module (3,0--3,6) })], { ConditionalDirectives = []
                                                      CodeComments = [] },
      set []))
