SigFile
  (ParsedSigFileInput
     ("/root/SignatureType/SynExceptionSigShouldContainsTheRangeOfTheWithKeyword.fsi",
      QualifiedNameOfFile SynExceptionSigShouldContainsTheRangeOfTheWithKeyword,
      [], [],
      [SynModuleOrNamespaceSig
         ([X], false, DeclaredNamespace,
          [Exception
             (SynExceptionSig
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (Foo, None), Fields [], PreXmlDocEmpty, None,
                       (4,10--4,13), { BarRange = None }), None,
                    PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                    (4,0--4,13)), Some (4,14--4,18),
                 [Member
                    (SynValSig
                       ([], SynIdent (Meh, None), SynValTyparDecls (None, true),
                        Fun
                          (LongIdent (SynLongIdent ([unit], [], [None])),
                           LongIdent (SynLongIdent ([unit], [], [None])),
                           (5,17--5,29), { ArrowRange = (5,22--5,24) }),
                        SynValInfo
                          ([[SynArgInfo ([], false, None)]],
                           SynArgInfo ([], false, None)), false, false,
                        PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                        Single None, None, (5,4--5,29),
                        { LeadingKeyword = Member (5,4--5,10)
                          InlineKeyword = None
                          WithKeyword = None
                          EqualsRange = None }),
                     { IsInstance = true
                       IsDispatchSlot = false
                       IsOverrideOrExplicitImpl = false
                       IsFinal = false
                       GetterOrSetterIsCompilerGenerated = false
                       MemberKind = Member }, (5,4--5,29),
                     { GetSetKeywords = None })], (4,0--5,29)), (4,0--5,29))],
          PreXmlDocEmpty, [], None, (2,0--5,29),
          { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
