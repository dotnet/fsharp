ImplFile
  (ParsedImplFileInput
     ("/root/Member/GetSetMemberWithInlineKeyword.fs", false,
      QualifiedNameOfFile GetSetMemberWithInlineKeyword, [], [],
      [SynModuleOrNamespace
         ([GetSetMemberWithInlineKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, true, false, [],
                               PreXmlMerge
  (PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                               SynValData
                                 (Some
                                    { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = PropertyGet },
                                  SynValInfo
                                    ([[SynArgInfo ([], false, None)]; []],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([x; Y], [(3,19--3,20)], [None; None]),
                                  Some get, None,
                                  Pats
                                    [Paren
                                       (Const (Unit, (4,24--4,26)), (4,24--4,26))],
                                  None, (4,20--4,26)), None,
                               Const (Int32 4, (4,29--4,30)), (4,20--4,26),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (3,4--3,10)
                                 InlineKeyword = Some (4,13--4,19)
                                 EqualsRange = Some (4,27--4,28) })),
                         Some
                           (SynBinding
                              (None, Normal, true, false, [],
                               PreXmlMerge
  (PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                               SynValData
                                 (Some
                                    { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = PropertySet },
                                  SynValInfo
                                    ([[SynArgInfo ([], false, None)];
                                      [SynArgInfo ([], false, Some y)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([x; Y], [(3,19--3,20)], [None; None]),
                                  Some set, None,
                                  Pats
                                    [Named
                                       (SynIdent (y, None), false, None,
                                        (5,23--5,24))], None, (5,19--5,24)),
                               None, Const (Unit, (5,27--5,29)), (5,19--5,24),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (3,4--3,10)
                                 InlineKeyword = Some (5,12--5,18)
                                 EqualsRange = Some (5,25--5,26) })),
                         (3,4--5,29), { InlineKeyword = Some (3,11--3,17)
                                        WithKeyword = (4,8--4,12)
                                        GetKeyword = Some (4,20--4,23)
                                        AndKeyword = Some (5,8--5,11)
                                        SetKeyword = Some (5,19--5,22) })],
                     (3,4--5,29)), [], None, (2,5--5,29),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,7--2,8)
                    WithKeyword = None })], (2,0--5,29))], PreXmlDocEmpty, [],
          None, (2,0--6,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
