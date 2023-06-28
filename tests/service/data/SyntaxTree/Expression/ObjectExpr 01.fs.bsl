ImplFile
  (ParsedImplFileInput
     ("/root/Expression/ObjectExpr 01.fs", false,
      QualifiedNameOfFile ObjectExpr 01, [], [],
      [SynModuleOrNamespace
         ([ObjectExpr 01], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([[]], SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([create], [], [None]), None, None,
                     Pats [Paren (Const (Unit, (1,11--1,13)), (1,11--1,13))],
                     None, (1,4--1,13)), None,
                  ObjExpr
                    (LongIdent (SynLongIdent ([Object], [], [None])),
                     Some (Const (Unit, (2,16--2,18)), None), Some (2,19--2,23),
                     [],
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((3,8), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([_; ToString], [(3,18--3,19)], [None; None]),
                               None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (3,27--3,29)), (3,27--3,29))],
                               None, (3,17--3,29)), None,
                            Const
                              (String ("", Regular, (3,32--3,34)), (3,32--3,34)),
                            (3,17--3,29), NoneAtInvisible,
                            { LeadingKeyword = Override (3,8--3,16)
                              InlineKeyword = None
                              EqualsRange = Some (3,30--3,31) }), (3,8--3,34))],
                     [SynInterfaceImpl
                        (LongIdent (SynLongIdent ([Interface1], [], [None])),
                         Some (4,27--4,31), [],
                         [Member
                            (SynBinding
                               (None, Normal, false, false, [],
                                PreXmlDoc ((5,10), FSharp.Compiler.Xml.XmlDocCollector),
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
                                      SynArgInfo ([], false, None)), None),
                                LongIdent
                                  (SynLongIdent
                                     ([_; Foo1], [(5,18--5,19)], [None; None]),
                                   None, None,
                                   Pats
                                     [Named
                                        (SynIdent (s, None), false, None,
                                         (5,24--5,25))], None, (5,17--5,25)),
                                None, Ident s, (5,17--5,25), NoneAtInvisible,
                                { LeadingKeyword = Member (5,10--5,16)
                                  InlineKeyword = None
                                  EqualsRange = Some (5,26--5,27) }),
                             (5,10--5,29))], (4,6--5,29));
                      SynInterfaceImpl
                        (LongIdent (SynLongIdent ([Interface2], [], [None])),
                         Some (7,27--7,31), [],
                         [Member
                            (SynBinding
                               (None, Normal, false, false, [],
                                PreXmlDoc ((8,10), FSharp.Compiler.Xml.XmlDocCollector),
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
                                      SynArgInfo ([], false, None)), None),
                                LongIdent
                                  (SynLongIdent
                                     ([_; Foo2], [(8,18--8,19)], [None; None]),
                                   None, None,
                                   Pats
                                     [Named
                                        (SynIdent (s, None), false, None,
                                         (8,24--8,25))], None, (8,17--8,25)),
                                None, Ident s, (8,17--8,25), NoneAtInvisible,
                                { LeadingKeyword = Member (8,10--8,16)
                                  InlineKeyword = None
                                  EqualsRange = Some (8,26--8,27) }),
                             (8,10--8,29))], (7,6--8,29))], (2,6--2,18),
                     (2,4--8,31)), (1,4--1,13), NoneAtLet,
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,14--1,15) })], (1,0--8,31))],
          PreXmlDocEmpty, [], None, (1,0--9,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'ObjectExpr 01' based on the file name 'ObjectExpr 01.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
