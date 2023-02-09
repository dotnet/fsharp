ImplFile
  (ParsedImplFileInput
     ("/root/ObjectModelWithTwoMembers.fs", false,
      QualifiedNameOfFile ObjectModelWithTwoMembers, [], [],
      [SynModuleOrNamespace
         ([ObjectModelWithTwoMembers], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, /root/ObjectModelWithTwoMembers.fs (1,5--1,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [],
                         SimplePats
                           ([], /root/ObjectModelWithTwoMembers.fs (1,6--1,8)),
                         None,
                         PreXmlDoc ((1,6), FSharp.Compiler.Xml.XmlDocCollector),
                         /root/ObjectModelWithTwoMembers.fs (1,5--1,6),
                         { AsKeyword = None });
                      LetBindings
                        ([SynBinding
                            (None, Normal, false, true, [],
                             PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named
                               (SynIdent (allowInto, None), false, None,
                                /root/ObjectModelWithTwoMembers.fs (2,16--2,25)),
                             None,
                             Const
                               (Int32 0,
                                /root/ObjectModelWithTwoMembers.fs (2,28--2,29)),
                             /root/ObjectModelWithTwoMembers.fs (2,16--2,25),
                             Yes /root/ObjectModelWithTwoMembers.fs (2,4--2,29),
                             { LeadingKeyword =
                                Let
                                  /root/ObjectModelWithTwoMembers.fs (2,4--2,7)
                               InlineKeyword = None
                               EqualsRange =
                                Some
                                  /root/ObjectModelWithTwoMembers.fs (2,26--2,27) })],
                         false, false,
                         /root/ObjectModelWithTwoMembers.fs (2,4--2,29));
                      GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false, [],
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
                                    ([_; AllowIntoPattern],
                                     [/root/ObjectModelWithTwoMembers.fs (3,12--3,13)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const
                                          (Unit,
                                           /root/ObjectModelWithTwoMembers.fs (3,38--3,40)),
                                        /root/ObjectModelWithTwoMembers.fs (3,38--3,40))],
                                  None,
                                  /root/ObjectModelWithTwoMembers.fs (3,35--3,40)),
                               None, Ident allowInto,
                               /root/ObjectModelWithTwoMembers.fs (3,35--3,40),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/ObjectModelWithTwoMembers.fs (3,4--3,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/ObjectModelWithTwoMembers.fs (3,41--3,42) })),
                         Some
                           (SynBinding
                              (None, Normal, false, false, [],
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
                                      [SynArgInfo ([], false, Some v)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([_; AllowIntoPattern],
                                     [/root/ObjectModelWithTwoMembers.fs (3,12--3,13)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Named
                                       (SynIdent (v, None), false, None,
                                        /root/ObjectModelWithTwoMembers.fs (3,61--3,62))],
                                  None,
                                  /root/ObjectModelWithTwoMembers.fs (3,57--3,62)),
                               None,
                               LongIdentSet
                                 (SynLongIdent ([allowInto], [], [None]),
                                  Ident v,
                                  /root/ObjectModelWithTwoMembers.fs (3,65--3,79)),
                               /root/ObjectModelWithTwoMembers.fs (3,57--3,62),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/ObjectModelWithTwoMembers.fs (3,4--3,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/ObjectModelWithTwoMembers.fs (3,63--3,64) })),
                         /root/ObjectModelWithTwoMembers.fs (3,4--3,79),
                         { InlineKeyword = None
                           WithKeyword =
                            /root/ObjectModelWithTwoMembers.fs (3,30--3,34)
                           GetKeyword =
                            Some /root/ObjectModelWithTwoMembers.fs (3,35--3,38)
                           AndKeyword =
                            Some /root/ObjectModelWithTwoMembers.fs (3,53--3,56)
                           SetKeyword =
                            Some /root/ObjectModelWithTwoMembers.fs (3,57--3,60) })],
                     /root/ObjectModelWithTwoMembers.fs (2,4--3,79)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        SimplePats
                          ([], /root/ObjectModelWithTwoMembers.fs (1,6--1,8)),
                        None,
                        PreXmlDoc ((1,6), FSharp.Compiler.Xml.XmlDocCollector),
                        /root/ObjectModelWithTwoMembers.fs (1,5--1,6),
                        { AsKeyword = None })),
                  /root/ObjectModelWithTwoMembers.fs (1,5--3,79),
                  { LeadingKeyword =
                     Type /root/ObjectModelWithTwoMembers.fs (1,0--1,4)
                    EqualsRange =
                     Some /root/ObjectModelWithTwoMembers.fs (1,9--1,10)
                    WithKeyword = None })],
              /root/ObjectModelWithTwoMembers.fs (1,0--3,79))], PreXmlDocEmpty,
          [], None, /root/ObjectModelWithTwoMembers.fs (1,0--3,79),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))