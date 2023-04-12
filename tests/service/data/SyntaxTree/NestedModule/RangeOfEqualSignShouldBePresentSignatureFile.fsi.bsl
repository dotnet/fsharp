SigFile
  (ParsedSigFileInput
     ("/root/NestedModule/RangeOfEqualSignShouldBePresentSignatureFile.fsi",
      QualifiedNameOfFile RangeOfEqualSignShouldBePresentSignatureFile, [], [],
      [SynModuleOrNamespaceSig
         ([Foo], false, DeclaredNamespace,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [X],
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (4,0--4,8)), false,
              [Val
                 (SynValSig
                    ([], SynIdent (bar, None), SynValTyparDecls (None, true),
                     LongIdent (SynLongIdent ([int], [], [None])),
                     SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                     PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                     None, None, (5,4--5,17), { LeadingKeyword = Val (5,4--5,7)
                                                InlineKeyword = None
                                                WithKeyword = None
                                                EqualsRange = None }),
                  (5,4--5,17))], (4,0--5,17), { ModuleKeyword = Some (4,0--4,6)
                                                EqualsRange = Some (4,9--4,10) })],
          PreXmlDocEmpty, [], None, (2,0--5,17),
          { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
