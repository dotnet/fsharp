ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/ReturnTypeAttributeOnBareTuple.fs", false,
      QualifiedNameOfFile M, [],
      [SynModuleOrNamespace
         ([M], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[]],
                                  SynArgInfo
                                    ([{ Attributes =
                                         [{ TypeName =
                                             SynLongIdent ([A], [], [None])
                                            ArgExpr = Const (Unit, (5,10--5,11))
                                            Target = None
                                            AppliesToGetterAndSetter = false
                                            Range = (5,10--5,11) }]
                                        Range = (5,8--5,13) }], false, None)),
                               None),
                            LongIdent
                              (SynLongIdent ([GetPair], [], [None]), None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (4,25--4,27)), (4,25--4,27))],
                               None, (4,18--4,27)),
                            Some
                              (SynBindingReturnInfo
                                 (Tuple
                                    (false,
                                     [Type
                                        (SignatureParameter
                                           ([{ Attributes =
                                                [{ TypeName =
                                                    SynLongIdent
                                                      ([A], [], [None])
                                                   ArgExpr =
                                                    Const (Unit, (5,10--5,11))
                                                   Target = None
                                                   AppliesToGetterAndSetter =
                                                    false
                                                   Range = (5,10--5,11) }]
                                               Range = (5,8--5,13) }], false,
                                            None,
                                            LongIdent
                                              (SynLongIdent
                                                 ([string], [], [None])),
                                            (5,8--6,14))); Star (6,15--6,16);
                                      Type
                                        (LongIdent
                                           (SynLongIdent ([string], [], [None])))],
                                     (5,8--6,23)), (5,8--6,23),
                                  [{ Attributes =
                                      [{ TypeName =
                                          SynLongIdent ([A], [], [None])
                                         ArgExpr = Const (Unit, (5,10--5,11))
                                         Target = None
                                         AppliesToGetterAndSetter = false
                                         Range = (5,10--5,11) }]
                                     Range = (5,8--5,13) }],
                                  { ColonRange = Some (4,28--4,29) })),
                            Typed
                              (Tuple
                                 (false,
                                  [Const
                                     (String ("", Regular, (7,12--7,14)),
                                      (7,12--7,14));
                                   Const
                                     (String ("", Regular, (7,16--7,18)),
                                      (7,16--7,18))], [(7,14--7,15)],
                                  (7,12--7,18)),
                               Tuple
                                 (false,
                                  [Type
                                     (SignatureParameter
                                        ([{ Attributes =
                                             [{ TypeName =
                                                 SynLongIdent ([A], [], [None])
                                                ArgExpr =
                                                 Const (Unit, (5,10--5,11))
                                                Target = None
                                                AppliesToGetterAndSetter = false
                                                Range = (5,10--5,11) }]
                                            Range = (5,8--5,13) }], false, None,
                                         LongIdent
                                           (SynLongIdent ([string], [], [None])),
                                         (5,8--6,14))); Star (6,15--6,16);
                                   Type
                                     (LongIdent
                                        (SynLongIdent ([string], [], [None])))],
                                  (5,8--6,23)), (7,12--7,18)), (4,18--4,27),
                            NoneAtInvisible,
                            { LeadingKeyword =
                               StaticMember ((4,4--4,10), (4,11--4,17))
                              InlineKeyword = None
                              EqualsRange = Some (6,24--6,25) }), (4,4--7,18))],
                     (4,4--7,18)), [], None, (3,5--7,18),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--7,18))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,18), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
