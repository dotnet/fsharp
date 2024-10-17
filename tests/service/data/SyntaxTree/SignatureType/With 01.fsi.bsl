SigFile
  (ParsedSigFileInput
     ("/root/SignatureType/With 01.fsi", QualifiedNameOfFile With 01, [], [],
      [SynModuleOrNamespaceSig
         ([X], false, DeclaredNamespace,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,8)),
                  Simple (None (3,5--4,28), (3,5--4,28)),
                  [Member
                     (SynValSig
                        ([], SynIdent (Meh, None), SynValTyparDecls (None, true),
                         Fun
                           (LongIdent (SynLongIdent ([unit], [], [None])),
                            LongIdent (SynLongIdent ([unit], [], [None])),
                            (4,16--4,28), { ArrowRange = (4,21--4,23) }),
                         SynValInfo
                           ([[SynArgInfo ([], false, None)]],
                            SynArgInfo ([], false, None)), false, false,
                         PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                         Single None, None, (4,4--4,28),
                         { LeadingKeyword = Member (4,4--4,10)
                           InlineKeyword = None
                           WithKeyword = None
                           EqualsRange = None }),
                      { IsInstance = true
                        IsDispatchSlot = false
                        IsOverrideOrExplicitImpl = false
                        IsFinal = false
                        GetterOrSetterIsCompilerGenerated = false
                        MemberKind = Member }, (4,4--4,28),
                      { GetSetKeywords = None })], (3,5--4,28),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = None
                    WithKeyword = Some (3,9--3,13) })], (3,0--4,28))],
          PreXmlDocEmpty, [], None, (1,0--4,28),
          { LeadingKeyword = Namespace (1,0--1,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
