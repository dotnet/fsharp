ImplFile
  (ParsedImplFileInput
     ("/root/SynType/SynTypeOrInsideSynTypeConstraintWhereTyparSupportsMember.fs",
      false,
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
                                              (2,41--2,44)),
                                           Var
                                             (SynTypar (T2, None, false),
                                              (2,48--2,51)), (2,41--2,51),
                                           { OrKeyword = (2,45--2,47) }),
                                        (2,40--2,52)),
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
                                              (2,84--2,94),
                                              { ArrowRange = (2,88--2,90) }),
                                           SynValInfo
                                             ([[SynArgInfo ([], false, None)]],
                                              SynArgInfo ([], false, None)),
                                           false, false,
                                           PreXmlDoc ((2,56), FSharp.Compiler.Xml.XmlDocCollector),
                                           None, None, (2,56--2,94),
                                           { LeadingKeyword =
                                              StaticMember
                                                ((2,56--2,62), (2,63--2,69))
                                             InlineKeyword = None
                                             WithKeyword = None
                                             EqualsRange = None }),
                                        { IsInstance = false
                                          IsDispatchSlot = false
                                          IsOverrideOrExplicitImpl = false
                                          IsFinal = false
                                          GetterOrSetterIsCompilerGenerated =
                                           false
                                          MemberKind = Member }, (2,56--2,94),
                                        { GetSetKeywords = None }), (2,40--2,95))],
                                 (2,25--2,97))), false)),
                     Pats [Paren (Const (Unit, (2,97--2,99)), (2,97--2,99))],
                     None, (2,11--2,99)),
                  Some
                    (SynBindingReturnInfo
                       (LongIdent (SynLongIdent ([int], [], [None])),
                        (2,102--2,105), [], { ColonRange = Some (2,100--2,101) })),
                  Typed
                    (Const (Unit, (3,4--3,6)),
                     LongIdent (SynLongIdent ([int], [], [None])), (3,4--3,6)),
                  (2,11--2,99), NoneAtLet, { LeadingKeyword = Let (2,0--2,3)
                                             InlineKeyword = Some (2,4--2,10)
                                             EqualsRange = Some (2,106--2,107) })],
              (2,0--3,6))], PreXmlDocEmpty, [], None, (2,0--4,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
