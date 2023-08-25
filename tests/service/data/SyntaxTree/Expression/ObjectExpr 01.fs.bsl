ImplFile
  (ParsedImplFileInput
     ("/root/Expression/ObjectExpr 01.fs", false, QualifiedNameOfFile V, [], [],
      [SynModuleOrNamespace
         ([V], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([[]], SynArgInfo ([], false, None)), None,
                     None),
                  LongIdent
                    (SynLongIdent ([create], [], [None]), None, None,
                     Pats [Paren (Const (Unit, (3,11--3,13)), (3,11--3,13))],
                     None, (3,4--3,13)), None,
                  ObjExpr
                    (LongIdent (SynLongIdent ([Object], [], [None])),
                     Some (Const (Unit, (4,16--4,18)), None), Some (4,19--4,23),
                     [],
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((5,8), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = true
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)]; []],
                                  SynArgInfo ([], false, None)), None, None),
                            LongIdent
                              (SynLongIdent
                                 ([_; ToString], [(5,18--5,19)], [None; None]),
                               None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (5,27--5,29)), (5,27--5,29))],
                               None, (5,17--5,29)), None,
                            Const
                              (String ("", Regular, (5,32--5,34)), (5,32--5,34)),
                            (5,17--5,29), NoneAtInvisible,
                            { LeadingKeyword = Override (5,8--5,16)
                              InlineKeyword = None
                              EqualsRange = Some (5,30--5,31) }), (5,8--5,34))],
                     [SynInterfaceImpl
                        (LongIdent (SynLongIdent ([Interface1], [], [None])),
                         Some (6,27--6,31), [],
                         [Member
                            (SynBinding
                               (None, Normal, false, false, [],
                                PreXmlDoc ((7,10), FSharp.Compiler.Xml.XmlDocCollector),
                                SynValData
                                  (Some
                                     { IsInstance = true
                                       IsDispatchSlot = false
                                       IsOverrideOrExplicitImpl = true
                                       IsFinal = false
                                       GetterOrSetterIsCompilerGenerated = false
                                       MemberKind = Member },
                                   SynValInfo
                                     ([[SynArgInfo ([], false, None)];
                                       [SynArgInfo ([], false, Some s)]],
                                      SynArgInfo ([], false, None)), None, None),
                                LongIdent
                                  (SynLongIdent
                                     ([_; Foo1], [(7,18--7,19)], [None; None]),
                                   None, None,
                                   Pats
                                     [Named
                                        (SynIdent (s, None), false, None,
                                         (7,24--7,25))], None, (7,17--7,25)),
                                None, Ident s, (7,17--7,25), NoneAtInvisible,
                                { LeadingKeyword = Member (7,10--7,16)
                                  InlineKeyword = None
                                  EqualsRange = Some (7,26--7,27) }),
                             (7,10--7,29))], (6,6--7,29));
                      SynInterfaceImpl
                        (LongIdent (SynLongIdent ([Interface2], [], [None])),
                         Some (9,27--9,31), [],
                         [Member
                            (SynBinding
                               (None, Normal, false, false, [],
                                PreXmlDoc ((10,10), FSharp.Compiler.Xml.XmlDocCollector),
                                SynValData
                                  (Some
                                     { IsInstance = true
                                       IsDispatchSlot = false
                                       IsOverrideOrExplicitImpl = true
                                       IsFinal = false
                                       GetterOrSetterIsCompilerGenerated = false
                                       MemberKind = Member },
                                   SynValInfo
                                     ([[SynArgInfo ([], false, None)];
                                       [SynArgInfo ([], false, Some s)]],
                                      SynArgInfo ([], false, None)), None, None),
                                LongIdent
                                  (SynLongIdent
                                     ([_; Foo2], [(10,18--10,19)], [None; None]),
                                   None, None,
                                   Pats
                                     [Named
                                        (SynIdent (s, None), false, None,
                                         (10,24--10,25))], None, (10,17--10,25)),
                                None, Ident s, (10,17--10,25), NoneAtInvisible,
                                { LeadingKeyword = Member (10,10--10,16)
                                  InlineKeyword = None
                                  EqualsRange = Some (10,26--10,27) }),
                             (10,10--10,29))], (9,6--10,29))], (4,6--4,18),
                     (4,4--10,31)), (3,4--3,13), NoneAtLet,
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,14--3,15) })], (3,0--10,31))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--10,31), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
