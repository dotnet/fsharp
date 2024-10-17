ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/ObjectModelWithTwoMembers.fs", false,
      QualifiedNameOfFile ObjectModelWithTwoMembers, [], [],
      [SynModuleOrNamespace
         ([ObjectModelWithTwoMembers], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (2,6--2,8)), None,
                         PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (2,5--2,6), { AsKeyword = None });
                      LetBindings
                        ([SynBinding
                            (None, Normal, false, true, [],
                             PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named
                               (SynIdent (allowInto, None), false, None,
                                (3,16--3,25)), None,
                             Const (Int32 0, (3,28--3,29)), (3,16--3,25),
                             Yes (3,4--3,29),
                             { LeadingKeyword = Let (3,4--3,7)
                               InlineKeyword = None
                               EqualsRange = Some (3,26--3,27) })], false, false,
                         (3,4--3,29));
                      GetSetMember
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
                                    ([_; AllowIntoPattern], [(4,12--4,13)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const (Unit, (4,38--4,40)), (4,38--4,40))],
                                  None, (4,35--4,40)), None, Ident allowInto,
                               (4,35--4,40), NoneAtInvisible,
                               { LeadingKeyword = Member (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (4,41--4,42) })),
                         Some
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
                                      MemberKind = PropertySet },
                                  SynValInfo
                                    ([[SynArgInfo ([], false, None)];
                                      [SynArgInfo ([], false, Some v)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([_; AllowIntoPattern], [(4,12--4,13)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Named
                                       (SynIdent (v, None), false, None,
                                        (4,61--4,62))], None, (4,57--4,62)),
                               None,
                               LongIdentSet
                                 (SynLongIdent ([allowInto], [], [None]),
                                  Ident v, (4,65--4,79)), (4,57--4,62),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (4,63--4,64) })),
                         (4,4--4,79), { InlineKeyword = None
                                        WithKeyword = (4,30--4,34)
                                        GetKeyword = Some (4,35--4,38)
                                        AndKeyword = Some (4,53--4,56)
                                        SetKeyword = Some (4,57--4,60) })],
                     (3,4--4,79)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (2,6--2,8)), None,
                        PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (2,5--2,6), { AsKeyword = None })), (2,5--4,79),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (2,0--4,79))], PreXmlDocEmpty, [],
          None, (2,0--5,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
