ImplFile
  (ParsedImplFileInput
     ("/root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs", false,
      QualifiedNameOfFile
        SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember, [], [],
      [SynModuleOrNamespace
         ([SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember], false,
          AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, true, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([[]], SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([f_StaticMethod], [], [None]), None,
                     Some
                       (SynValTyparDecls
                          (Some
                             (PostfixList
                                ([SynTyparDecl ([], SynTypar (T1, None, false));
                                  SynTyparDecl ([], SynTypar (T2, None, false))],
                                 [WhereTyparSupportsMember
                                    (Paren
                                       (Or
                                          (Var
                                             (SynTypar (T1, None, false),
                                              /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,41--2,44)),
                                           Var
                                             (SynTypar (T2, None, false),
                                              /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,48--2,51)),
                                           /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,41--2,51),
                                           { OrKeyword =
                                              /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,45--2,47) }),
                                        /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,40--2,52)),
                                     Member
                                       (SynValSig
                                          ([], SynIdent (StaticMethod, None),
                                           SynValTyparDecls (None, true),
                                           Fun
                                             (LongIdent
                                                (SynLongIdent
                                                   ([int], [], [None])),
                                              LongIdent
                                                (SynLongIdent
                                                   ([int], [], [None])),
                                              /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,84--2,94),
                                              { ArrowRange =
                                                 /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,88--2,90) }),
                                           SynValInfo
                                             ([[SynArgInfo ([], false, None)]],
                                              SynArgInfo ([], false, None)),
                                           false, false,
                                           PreXmlDoc ((2,56), FSharp.Compiler.Xml.XmlDocCollector),
                                           None, None,
                                           /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,56--2,94),
                                           { LeadingKeyword =
                                              StaticMember
                                                (/root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,56--2,62),
                                                 /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,63--2,69))
                                             InlineKeyword = None
                                             WithKeyword = None
                                             EqualsRange = None }),
                                        { IsInstance = false
                                          IsDispatchSlot = false
                                          IsOverrideOrExplicitImpl = false
                                          IsFinal = false
                                          GetterOrSetterIsCompilerGenerated =
                                           false
                                          MemberKind = Member },
                                        /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,56--2,94),
                                        { GetSetKeywords = None }),
                                     /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,40--2,95))],
                                 /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,25--2,97))),
                           false)),
                     Pats
                       [Paren
                          (Const
                             (Unit,
                              /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,97--2,99)),
                           /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,97--2,99))],
                     None,
                     /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,11--2,99)),
                  Some
                    (SynBindingReturnInfo
                       (LongIdent (SynLongIdent ([int], [], [None])),
                        /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,102--2,105),
                        [],
                        { ColonRange =
                           Some
                             /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,100--2,101) })),
                  Typed
                    (Const
                       (Unit,
                        /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (3,4--3,6)),
                     LongIdent (SynLongIdent ([int], [], [None])),
                     /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (3,4--3,6)),
                  /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,11--2,99),
                  NoneAtLet,
                  { LeadingKeyword =
                     Let
                       /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,0--2,3)
                    InlineKeyword =
                     Some
                       /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,4--2,10)
                    EqualsRange =
                     Some
                       /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,106--2,107) })],
              /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,0--3,6))],
          PreXmlDocEmpty, [], None,
          /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,0--4,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))