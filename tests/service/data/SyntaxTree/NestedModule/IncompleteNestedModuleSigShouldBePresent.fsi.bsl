SigFile
  (ParsedSigFileInput
     ("/root/NestedModule/IncompleteNestedModuleSigShouldBePresent.fsi",
      QualifiedNameOfFile A.B, [], [],
      [SynModuleOrNamespaceSig
         ([A; B], false, NamedModule,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [C],
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (3,0--3,8)), false, [], (3,0--3,8),
              { ModuleKeyword = Some (3,0--3,6)
                EqualsRange = None });
           Val
             (SynValSig
                ([], SynIdent (a, None), SynValTyparDecls (None, true),
                 LongIdent (SynLongIdent ([unit], [], [None])),
                 SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                 Single None, None, (5,0--5,11),
                 { LeadingKeyword = Val (5,0--5,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange = None }), (5,0--5,11))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,11), { LeadingKeyword = Module (1,0--1,6) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,9)-(5,0) parse error Incomplete structured construct at or before this point in signature file. Expected ':', '=' or other token.
