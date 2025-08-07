ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InType_Struct.fs", false,
      QualifiedNameOfFile InType_Struct, [],
      [SynModuleOrNamespace
         ([InType_Struct], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName = SynLongIdent ([Struct], [], [None])
                            ArgExpr = Const (Unit, (1,2--1,8))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range = (1,2--1,8) }]
                        Range = (1,0--1,10) }], None, [], [ABC],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,8)),
                  ObjectModel
                    (Unspecified,
                     [Open
                        (ModuleOrNamespace
                           (SynLongIdent ([System], [], [None]), (3,9--3,15)),
                         (3,4--3,15));
                      ValField
                        (SynField
                           ([], false, Some a,
                            LongIdent (SynLongIdent ([Int32], [], [None])),
                            false,
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,4--4,16),
                            { LeadingKeyword = Some (Val (4,4--4,7))
                              MutableKeyword = None }), (4,4--4,16));
                      ValField
                        (SynField
                           ([], false, Some b,
                            LongIdent (SynLongIdent ([Int32], [], [None])),
                            false,
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,4--5,16),
                            { LeadingKeyword = Some (Val (5,4--5,7))
                              MutableKeyword = None }), (5,4--5,16));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Constructor },
                               SynValInfo
                                 ([[SynArgInfo ([], false, Some a)]],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent ([new], [], [None]), None,
                               Some (SynValTyparDecls (None, false)),
                               Pats
                                 [Paren
                                    (Named
                                       (SynIdent (a, None), false, None,
                                        (6,9--6,10)), (6,8--6,11))], None,
                               (6,4--6,7)), None,
                            Open
                              (Type
                                 (LongIdent
                                    (SynLongIdent
                                       ([System; Int32], [(7,24--7,25)],
                                        [None; None])), (7,18--7,30)),
                               (7,8--7,30), (7,8--8,31),
                               Record
                                 (None, None,
                                  [SynExprRecordField
                                     ((SynLongIdent ([a], [], [None]), true),
                                      Some (8,12--8,13), Some (Ident a),
                                      (8,10--8,15),
                                      Some ((8,15--8,16), Some (8,16)));
                                   SynExprRecordField
                                     ((SynLongIdent ([b], [], [None]), true),
                                      Some (8,19--8,20), Some (Ident MinValue),
                                      (8,17--8,29), None)], (8,8--8,31))),
                            (6,4--6,11), NoneAtInvisible,
                            { LeadingKeyword = New (6,4--6,7)
                              InlineKeyword = None
                              EqualsRange = Some (6,12--6,13) }), (6,4--8,31))],
                     (3,4--8,31)), [], None, (1,0--8,31),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (1,0--8,31))], PreXmlDocEmpty, [],
          None, (1,0--9,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
