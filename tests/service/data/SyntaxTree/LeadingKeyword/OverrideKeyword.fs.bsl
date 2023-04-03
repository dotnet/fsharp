ImplFile
  (ParsedImplFileInput
     ("/root/LeadingKeyword/OverrideKeyword.fs", false,
      QualifiedNameOfFile OverrideKeyword, [], [],
      [SynModuleOrNamespace
         ([OverrideKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [D],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = true
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)]; []],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent ([E], [], [None]), None, None,
                               Pats [], None, (3,13--3,14)),
                            Some
                              (SynBindingReturnInfo
                                 (LongIdent
                                    (SynLongIdent ([string], [], [None])),
                                  (3,17--3,23), [],
                                  { ColonRange = Some (3,15--3,16) })),
                            Typed
                              (Const
                                 (String ("", Regular, (3,26--3,28)),
                                  (3,26--3,28)),
                               LongIdent (SynLongIdent ([string], [], [None])),
                               (3,26--3,28)), (3,13--3,14), NoneAtInvisible,
                            { LeadingKeyword = Override (3,4--3,12)
                              InlineKeyword = None
                              EqualsRange = Some (3,24--3,25) }), (3,4--3,28))],
                     (3,4--3,28)), [], None, (2,5--3,28),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,7--2,8)
                    WithKeyword = None })], (2,0--3,28))], PreXmlDocEmpty, [],
          None, (2,0--4,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
