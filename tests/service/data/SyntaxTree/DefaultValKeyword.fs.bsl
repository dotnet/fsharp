ImplFile
  (ParsedImplFileInput
     ("/root/DefaultValKeyword.fs", false, QualifiedNameOfFile DefaultValKeyword,
      [], [],
      [SynModuleOrNamespace
         ([DefaultValKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, /root/DefaultValKeyword.fs (1,5--1,8)),
                  ObjectModel
                    (Unspecified,
                     [AutoProperty
                        ([], false, A,
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
                           (Int32 1, /root/DefaultValKeyword.fs (2,26--2,27)),
                         /root/DefaultValKeyword.fs (2,4--2,27),
                         { LeadingKeyword =
                            DefaultVal
                              (/root/DefaultValKeyword.fs (2,4--2,11),
                               /root/DefaultValKeyword.fs (2,12--2,15))
                           WithKeyword = None
                           EqualsRange =
                            Some /root/DefaultValKeyword.fs (2,24--2,25)
                           GetSetKeywords = None })],
                     /root/DefaultValKeyword.fs (2,4--2,27)), [], None,
                  /root/DefaultValKeyword.fs (1,5--2,27),
                  { LeadingKeyword = Type /root/DefaultValKeyword.fs (1,0--1,4)
                    EqualsRange = Some /root/DefaultValKeyword.fs (1,9--1,10)
                    WithKeyword = None })],
              /root/DefaultValKeyword.fs (1,0--2,27))], PreXmlDocEmpty, [], None,
          /root/DefaultValKeyword.fs (1,0--2,27), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))