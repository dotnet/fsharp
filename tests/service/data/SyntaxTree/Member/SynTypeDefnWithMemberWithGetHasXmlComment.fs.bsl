ImplFile
  (ParsedImplFileInput
     ("/root/Member/SynTypeDefnWithMemberWithGetHasXmlComment.fs", false,
      QualifiedNameOfFile SynTypeDefnWithMemberWithGetHasXmlComment, [], [],
      [SynModuleOrNamespace
         ([SynTypeDefnWithMemberWithGetHasXmlComment], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [A],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                    ([x; B], [(4,12--4,13)], [None; None]),
                                  Some get, None,
                                  Pats
                                    [Paren
                                       (Const (Unit, (4,23--4,25)), (4,23--4,25))],
                                  None, (4,20--4,25)), None,
                               Const (Int32 5, (4,28--4,29)), (3,4--4,25),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (4,26--4,27) })), None,
                         (3,4--4,29), { InlineKeyword = None
                                        WithKeyword = (4,15--4,19)
                                        GetKeyword = Some (4,20--4,23)
                                        AndKeyword = None
                                        SetKeyword = None })], (3,4--4,29)), [],
                  None, (2,5--4,29), { LeadingKeyword = Type (2,0--2,4)
                                       EqualsRange = Some (2,7--2,8)
                                       WithKeyword = None })], (2,0--4,29))],
          PreXmlDocEmpty, [], None, (2,0--5,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
