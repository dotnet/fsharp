SigFile
  (ParsedSigFileInput
     ("/root/ModuleOrNamespaceSig/Namespace - Keyword 01.fsi",
      QualifiedNameOfFile Namespace - Keyword 01, [], [],
      [SynModuleOrNamespaceSig
         ([Foo], false, DeclaredNamespace,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [Bar],
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (3,0--3,10)), false,
              [Val
                 (SynValSig
                    ([], SynIdent (a, None), SynValTyparDecls (None, true),
                     LongIdent (SynLongIdent ([int], [], [None])),
                     SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                     PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                     None, None, (4,4--4,14), { LeadingKeyword = Val (4,4--4,7)
                                                InlineKeyword = None
                                                WithKeyword = None
                                                EqualsRange = None }),
                  (4,4--4,14))], (3,0--4,14),
              { ModuleKeyword = Some (3,0--3,6)
                EqualsRange = Some (3,11--3,12) })], PreXmlDocEmpty, [], None,
          (1,0--4,14), { LeadingKeyword = Namespace (1,0--1,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
