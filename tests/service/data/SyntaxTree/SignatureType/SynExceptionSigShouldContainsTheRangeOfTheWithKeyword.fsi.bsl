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
                           (5,13--5,25), { ArrowRange = (5,18--5,20) }),
                        SynValInfo
                          ([[SynArgInfo ([], false, None)]],
                           SynArgInfo ([], false, None)), false, false,
                        PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                        None, None, (5,0--5,25),
                        { LeadingKeyword = Member (5,0--5,6)
                          InlineKeyword = None
                          WithKeyword = None
                          EqualsRange = None }),
                     { IsInstance = true
                       IsDispatchSlot = false
                       IsOverrideOrExplicitImpl = false
                       IsFinal = false
                       GetterOrSetterIsCompilerGenerated = false
                       MemberKind = Member }, (5,0--5,25),
                     { GetSetKeywords = None })], (4,0--5,25)), (4,0--5,25))],
          PreXmlDocEmpty, [], None, (2,0--5,25),
          { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
