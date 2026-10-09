ImplFile
  (ParsedImplFileInput
     ("/root/SynType/SynTypeAppNestedMultilineClosingGreaterAligned.fs", false,
      QualifiedNameOfFile SynTypeAppNestedMultilineClosingGreaterAligned, [],
      [SynModuleOrNamespace
         ([SynTypeAppNestedMultilineClosingGreaterAligned], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (1,5--1,6)),
                  ObjectModel
                    (Unspecified,
                     [AbstractSlot
                        (SynValSig
                           ([], SynIdent (M, None),
                            SynValTyparDecls (None, true),
                            App
                              (LongIdent (SynLongIdent ([A], [], [None])),
                               Some (3,9--3,10),
                               [App
                                  (LongIdent (SynLongIdent ([B], [], [None])),
                                   Some (4,13--4,14),
                                   [LongIdent (SynLongIdent ([int], [], [None]))],
                                   [], Some (6,12--6,13), false, (4,12--6,13))],
                               [], Some (7,8--7,9), false, (3,8--7,9)),
                            SynValInfo ([], SynArgInfo ([], false, None)), false,
                            false,
                            PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, None, (2,4--7,9),
                            { LeadingKeyword = Abstract (2,4--2,12)
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertyGet }, (2,4--7,9),
                         { GetSetKeywords = None })], (2,4--7,9)), [], None,
                  (1,5--7,9), { LeadingKeyword = Type (1,0--1,4)
                                EqualsRange = Some (1,7--1,8)
                                WithKeyword = None })], (1,0--7,9))],
          PreXmlDocEmpty, [], None, (1,0--8,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      WarnDirectives = []
                      CodeComments = [] }, set []))
