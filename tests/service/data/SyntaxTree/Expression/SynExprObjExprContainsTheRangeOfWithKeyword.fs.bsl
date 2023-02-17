ImplFile
  (ParsedImplFileInput
     ("/root/Expression/SynExprObjExprContainsTheRangeOfWithKeyword.fs", false,
      QualifiedNameOfFile SynExprObjExprContainsTheRangeOfWithKeyword, [], [],
      [SynModuleOrNamespace
         ([SynExprObjExprContainsTheRangeOfWithKeyword], false, AnonModule,
          [Expr
             (ObjExpr
                (LongIdent (SynLongIdent ([obj], [], [None])),
                 Some (Const (Unit, (2,9--2,11)), None), Some (2,12--2,16), [],
                 [Member
                    (SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (Some { IsInstance = true
                                  IsDispatchSlot = false
                                  IsOverrideOrExplicitImpl = true
                                  IsFinal = false
                                  GetterOrSetterIsCompilerGenerated = false
                                  MemberKind = Member },
                           SynValInfo
                             ([[SynArgInfo ([], false, None)]; []],
                              SynArgInfo ([], false, None)), None),
                        LongIdent
                          (SynLongIdent
                             ([x; ToString], [(3,12--3,13)], [None; None]), None,
                           None,
                           Pats
                             [Paren (Const (Unit, (3,21--3,23)), (3,21--3,23))],
                           None, (3,11--3,23)), None,
                        Const
                          (String
                             ("INotifyEnumerableInternal", Regular, (3,26--3,53)),
                           (3,26--3,53)), (3,11--3,23), NoneAtInvisible,
                        { LeadingKeyword = Member (3,4--3,10)
                          InlineKeyword = None
                          EqualsRange = Some (3,24--3,25) }), (3,4--3,53))],
                 [SynInterfaceImpl
                    (App
                       (LongIdent
                          (SynLongIdent
                             ([INotifyEnumerableInternal], [], [None])),
                        Some (4,37--4,38),
                        [Var (SynTypar (T, None, false), (4,38--4,40))], [],
                        Some (4,40--4,41), false, (4,12--4,41)), None, [], [],
                     (4,2--5,2));
                  SynInterfaceImpl
                    (App
                       (LongIdent (SynLongIdent ([IEnumerable], [], [None])),
                        Some (5,23--5,24), [Anon (5,24--5,25)], [],
                        Some (5,25--5,26), false, (5,12--5,26)),
                     Some (5,27--5,31), [],
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = true
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)]; []],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent
                                 ([x; GetEnumerator], [(6,12--6,13)],
                                  [None; None]), None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (6,26--6,28)), (6,26--6,28))],
                               None, (6,11--6,28)), None, Null (6,31--6,35),
                            (6,11--6,28), NoneAtInvisible,
                            { LeadingKeyword = Member (6,4--6,10)
                              InlineKeyword = None
                              EqualsRange = Some (6,29--6,30) }), (6,4--6,35))],
                     (5,2--6,37))], (2,2--2,11), (2,0--6,37)), (2,0--6,37))],
          PreXmlDocEmpty, [], None, (2,0--6,37), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
