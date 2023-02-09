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
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
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
                                              /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,41--1,44)),
                                           Var
                                             (SynTypar (T2, None, false),
                                              /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,48--1,51)),
                                           /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,41--1,51),
                                           { OrKeyword =
                                              /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,45--1,47) }),
                                        /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,40--1,52)),
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
                                              /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,84--1,94),
                                              { ArrowRange =
                                                 /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,88--1,90) }),
                                           SynValInfo
                                             ([[SynArgInfo ([], false, None)]],
                                              SynArgInfo ([], false, None)),
                                           false, false,
                                           PreXmlDoc ((1,56), FSharp.Compiler.Xml.XmlDocCollector),
                                           None, None,
                                           /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,56--1,94),
                                           { LeadingKeyword =
                                              StaticMember
                                                (/root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,56--1,62),
                                                 /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,63--1,69))
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
                                        /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,56--1,94),
                                        { GetSetKeywords = None }),
                                     /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,40--1,95))],
                                 /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,25--1,97))),
                           false)),
                     Pats
                       [Paren
                          (Const
                             (Unit,
                              /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,97--1,99)),
                           /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,97--1,99))],
                     None,
                     /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,11--1,99)),
                  Some
                    (SynBindingReturnInfo
                       (LongIdent (SynLongIdent ([int], [], [None])),
                        /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,102--1,105),
                        [],
                        { ColonRange =
                           Some
                             /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,100--1,101) })),
                  Typed
                    (Const
                       (Unit,
                        /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,4--2,6)),
                     LongIdent (SynLongIdent ([int], [], [None])),
                     /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (2,4--2,6)),
                  /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,11--1,99),
                  NoneAtLet,
                  { LeadingKeyword =
                     Let
                       /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,0--1,3)
                    InlineKeyword =
                     Some
                       /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,4--1,10)
                    EqualsRange =
                     Some
                       /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,106--1,107) })],
              /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,0--2,6))],
          PreXmlDocEmpty, [], None,
          /root/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs (1,0--2,6),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))