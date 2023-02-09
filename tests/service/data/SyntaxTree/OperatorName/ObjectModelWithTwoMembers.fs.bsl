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
                     false, None,
                     /root/OperatorName/ObjectModelWithTwoMembers.fs (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [],
                         SimplePats
                           ([],
                            /root/OperatorName/ObjectModelWithTwoMembers.fs (2,6--2,8)),
                         None,
                         PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                         /root/OperatorName/ObjectModelWithTwoMembers.fs (2,5--2,6),
                         { AsKeyword = None });
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
                                /root/OperatorName/ObjectModelWithTwoMembers.fs (3,16--3,25)),
                             None,
                             Const
                               (Int32 0,
                                /root/OperatorName/ObjectModelWithTwoMembers.fs (3,28--3,29)),
                             /root/OperatorName/ObjectModelWithTwoMembers.fs (3,16--3,25),
                             Yes
                               /root/OperatorName/ObjectModelWithTwoMembers.fs (3,4--3,29),
                             { LeadingKeyword =
                                Let
                                  /root/OperatorName/ObjectModelWithTwoMembers.fs (3,4--3,7)
                               InlineKeyword = None
                               EqualsRange =
                                Some
                                  /root/OperatorName/ObjectModelWithTwoMembers.fs (3,26--3,27) })],
                         false, false,
                         /root/OperatorName/ObjectModelWithTwoMembers.fs (3,4--3,29));
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
                                    ([_; AllowIntoPattern],
                                     [/root/OperatorName/ObjectModelWithTwoMembers.fs (4,12--4,13)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const
                                          (Unit,
                                           /root/OperatorName/ObjectModelWithTwoMembers.fs (4,38--4,40)),
                                        /root/OperatorName/ObjectModelWithTwoMembers.fs (4,38--4,40))],
                                  None,
                                  /root/OperatorName/ObjectModelWithTwoMembers.fs (4,35--4,40)),
                               None, Ident allowInto,
                               /root/OperatorName/ObjectModelWithTwoMembers.fs (4,35--4,40),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/OperatorName/ObjectModelWithTwoMembers.fs (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/OperatorName/ObjectModelWithTwoMembers.fs (4,41--4,42) })),
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
                                    ([_; AllowIntoPattern],
                                     [/root/OperatorName/ObjectModelWithTwoMembers.fs (4,12--4,13)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Named
                                       (SynIdent (v, None), false, None,
                                        /root/OperatorName/ObjectModelWithTwoMembers.fs (4,61--4,62))],
                                  None,
                                  /root/OperatorName/ObjectModelWithTwoMembers.fs (4,57--4,62)),
                               None,
                               LongIdentSet
                                 (SynLongIdent ([allowInto], [], [None]),
                                  Ident v,
                                  /root/OperatorName/ObjectModelWithTwoMembers.fs (4,65--4,79)),
                               /root/OperatorName/ObjectModelWithTwoMembers.fs (4,57--4,62),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/OperatorName/ObjectModelWithTwoMembers.fs (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/OperatorName/ObjectModelWithTwoMembers.fs (4,63--4,64) })),
                         /root/OperatorName/ObjectModelWithTwoMembers.fs (4,4--4,79),
                         { InlineKeyword = None
                           WithKeyword =
                            /root/OperatorName/ObjectModelWithTwoMembers.fs (4,30--4,34)
                           GetKeyword =
                            Some
                              /root/OperatorName/ObjectModelWithTwoMembers.fs (4,35--4,38)
                           AndKeyword =
                            Some
                              /root/OperatorName/ObjectModelWithTwoMembers.fs (4,53--4,56)
                           SetKeyword =
                            Some
                              /root/OperatorName/ObjectModelWithTwoMembers.fs (4,57--4,60) })],
                     /root/OperatorName/ObjectModelWithTwoMembers.fs (3,4--4,79)),
                  [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        SimplePats
                          ([],
                           /root/OperatorName/ObjectModelWithTwoMembers.fs (2,6--2,8)),
                        None,
                        PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                        /root/OperatorName/ObjectModelWithTwoMembers.fs (2,5--2,6),
                        { AsKeyword = None })),
                  /root/OperatorName/ObjectModelWithTwoMembers.fs (2,5--4,79),
                  { LeadingKeyword =
                     Type
                       /root/OperatorName/ObjectModelWithTwoMembers.fs (2,0--2,4)
                    EqualsRange =
                     Some
                       /root/OperatorName/ObjectModelWithTwoMembers.fs (2,9--2,10)
                    WithKeyword = None })],
              /root/OperatorName/ObjectModelWithTwoMembers.fs (2,0--4,79))],
          PreXmlDocEmpty, [], None,
          /root/OperatorName/ObjectModelWithTwoMembers.fs (2,0--5,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
