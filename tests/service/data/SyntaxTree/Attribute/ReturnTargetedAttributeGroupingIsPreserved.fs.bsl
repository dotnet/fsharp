ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/ReturnTargetedAttributeGroupingIsPreserved.fs", false,
      QualifiedNameOfFile M, [],
      [SynModuleOrNamespace
         ([M], false, NamedModule,
          [Open
             (ModuleOrNamespace
                (SynLongIdent ([System], [], [None]), (3,5--3,11)), (3,0--3,11));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName =
                             SynLongIdent ([AttributeUsage], [], [None])
                            ArgExpr =
                             Paren
                               (Tuple
                                  (false,
                                   [LongIdent
                                      (false,
                                       SynLongIdent
                                         ([AttributeTargets; ReturnValue],
                                          [(5,33--5,34)], [None; None]), None,
                                       (5,17--5,45));
                                    App
                                      (NonAtomic, false,
                                       App
                                         (NonAtomic, true,
                                          LongIdent
                                            (false,
                                             SynLongIdent
                                               ([op_Equality], [],
                                                [Some (OriginalNotation "=")]),
                                             None, (5,61--5,62)),
                                          Ident AllowMultiple, (5,47--5,62)),
                                       Const (Bool true, (5,63--5,67)),
                                       (5,47--5,67))], [(5,45--5,46)],
                                   (5,17--5,67)), (5,16--5,17),
                                Some (5,67--5,68), (5,16--5,68))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range = (5,2--5,68) }]
                        Range = (5,0--5,70) }], None, [],
                     Some (LongIdent (SynLongIdent ([AAttribute], [], [None]))),
                     PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (6,5--6,15)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (6,15--6,17)), None,
                         PreXmlDoc ((6,15), FSharp.Compiler.Xml.XmlDocCollector),
                         (6,5--6,15), { AsKeyword = None });
                      ImplicitInherit
                        (LongIdent (SynLongIdent ([Attribute], [], [None])),
                         Const (Unit, (7,21--7,23)), None, (7,4--7,23),
                         { InheritKeyword = (7,4--7,11) })], (7,4--7,23)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (6,15--6,17)), None,
                        PreXmlDoc ((6,15), FSharp.Compiler.Xml.XmlDocCollector),
                        (6,5--6,15), { AsKeyword = None })), (5,0--7,23),
                  { LeadingKeyword = Type (6,0--6,4)
                    EqualsRange = Some (6,18--6,19)
                    WithKeyword = None })], (5,0--7,23));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false,
                  [{ Attributes = [{ TypeName = SynLongIdent ([A], [], [None])
                                     ArgExpr = Const (Unit, (9,10--9,11))
                                     Target = Some return
                                     AppliesToGetterAndSetter = false
                                     Range = (9,2--9,11) }]
                     Range = (9,0--9,13) };
                   { Attributes = [{ TypeName = SynLongIdent ([A], [], [None])
                                     ArgExpr = Const (Unit, (9,23--9,24))
                                     Target = Some return
                                     AppliesToGetterAndSetter = false
                                     Range = (9,15--9,24) }]
                     Range = (9,13--9,26) }],
                  PreXmlDoc ((9,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([[]], SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([f], [], [None]), None, None,
                     Pats [Paren (Const (Unit, (10,6--10,8)), (10,6--10,8))],
                     None, (10,4--10,8)), None, Const (Unit, (10,11--10,13)),
                  (9,0--10,8), NoneAtLet, { LeadingKeyword = Let (10,0--10,3)
                                            InlineKeyword = None
                                            EqualsRange = Some (10,9--10,10) })],
              (9,0--10,13), { InKeyword = None });
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false,
                  [{ Attributes =
                      [{ TypeName = SynLongIdent ([A], [], [None])
                         ArgExpr = Const (Unit, (12,10--12,11))
                         Target = Some return
                         AppliesToGetterAndSetter = false
                         Range = (12,2--12,11) };
                       { TypeName = SynLongIdent ([A], [], [None])
                         ArgExpr = Const (Unit, (12,21--12,22))
                         Target = Some return
                         AppliesToGetterAndSetter = false
                         Range = (12,13--12,22) }]
                     Range = (12,0--12,24) }],
                  PreXmlDoc ((12,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([[]], SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([g], [], [None]), None, None,
                     Pats [Paren (Const (Unit, (13,6--13,8)), (13,6--13,8))],
                     None, (13,4--13,8)), None, Const (Unit, (13,11--13,13)),
                  (12,0--13,8), NoneAtLet, { LeadingKeyword = Let (13,0--13,3)
                                             InlineKeyword = None
                                             EqualsRange = Some (13,9--13,10) })],
              (12,0--13,13), { InKeyword = None })],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--13,13), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
