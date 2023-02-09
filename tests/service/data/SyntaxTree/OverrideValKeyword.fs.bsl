ImplFile
  (ParsedImplFileInput
     ("/root/OverrideValKeyword.fs", false,
      QualifiedNameOfFile OverrideValKeyword, [], [],
      [SynModuleOrNamespace
         ([OverrideValKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, /root/OverrideValKeyword.fs (1,5--1,6)),
                  ObjectModel
                    (Unspecified,
                     [AutoProperty
                        ([], false, Y,
                         Some (LongIdent (SynLongIdent ([int], [], [None]))),
                         Member, { IsInstance = true
                                   IsDispatchSlot = false
                                   IsOverrideOrExplicitImpl = true
                                   IsFinal = false
                                   GetterOrSetterIsCompilerGenerated = false
                                   MemberKind = Member },
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = true
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertySet },
                         PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                         None,
                         Const
                           (Int32 1, /root/OverrideValKeyword.fs (2,27--2,28)),
                         /root/OverrideValKeyword.fs (2,4--2,28),
                         { LeadingKeyword =
                            OverrideVal
                              (/root/OverrideValKeyword.fs (2,4--2,12),
                               /root/OverrideValKeyword.fs (2,13--2,16))
                           WithKeyword = None
                           EqualsRange =
                            Some /root/OverrideValKeyword.fs (2,25--2,26)
                           GetSetKeywords = None })],
                     /root/OverrideValKeyword.fs (2,4--2,28)), [], None,
                  /root/OverrideValKeyword.fs (1,5--2,28),
                  { LeadingKeyword = Type /root/OverrideValKeyword.fs (1,0--1,4)
                    EqualsRange = Some /root/OverrideValKeyword.fs (1,7--1,8)
                    WithKeyword = None })],
              /root/OverrideValKeyword.fs (1,0--2,28))], PreXmlDocEmpty, [],
          None, /root/OverrideValKeyword.fs (1,0--2,28),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))