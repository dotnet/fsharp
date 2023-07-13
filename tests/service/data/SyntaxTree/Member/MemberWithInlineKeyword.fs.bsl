ImplFile
  (ParsedImplFileInput
     ("/root/Member/MemberWithInlineKeyword.fs", false,
      QualifiedNameOfFile MemberWithInlineKeyword, [], [],
      [SynModuleOrNamespace
         ([MemberWithInlineKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, true, false, [],
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)]; []],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent
                                 ([x; Y], [(3,19--3,20)], [None; None]), None,
                               None,
                               Pats
                                 [Paren
                                    (Const (Unit, (3,22--3,24)), (3,22--3,24))],
                               None, (3,18--3,24)), None,
                            Const (Unit, (3,27--3,29)), (3,18--3,24),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (3,4--3,10)
                              InlineKeyword = Some (3,11--3,17)
                              EqualsRange = Some (3,25--3,26) }), (3,4--3,29))],
                     (3,4--3,29)), [], None, (2,5--3,29),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,7--2,8)
                    WithKeyword = None })], (2,0--3,29))], PreXmlDocEmpty, [],
          None, (2,0--4,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
