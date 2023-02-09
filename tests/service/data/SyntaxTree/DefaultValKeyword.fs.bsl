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
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, /root/DefaultValKeyword.fs (2,5--2,8)),
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
                         PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                         None,
                         Const
                           (Int32 1, /root/DefaultValKeyword.fs (3,26--3,27)),
                         /root/DefaultValKeyword.fs (3,4--3,27),
                         { LeadingKeyword =
                            DefaultVal
                              (/root/DefaultValKeyword.fs (3,4--3,11),
                               /root/DefaultValKeyword.fs (3,12--3,15))
                           WithKeyword = None
                           EqualsRange =
                            Some /root/DefaultValKeyword.fs (3,24--3,25)
                           GetSetKeywords = None })],
                     /root/DefaultValKeyword.fs (3,4--3,27)), [], None,
                  /root/DefaultValKeyword.fs (2,5--3,27),
                  { LeadingKeyword = Type /root/DefaultValKeyword.fs (2,0--2,4)
                    EqualsRange = Some /root/DefaultValKeyword.fs (2,9--2,10)
                    WithKeyword = None })],
              /root/DefaultValKeyword.fs (2,0--3,27))], PreXmlDocEmpty, [], None,
          /root/DefaultValKeyword.fs (2,0--4,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))