ImplFile
  (ParsedImplFileInput
     ("/root/TypeParameters/Attribute.fs", false, QualifiedNameOfFile Attribute,
      [], [],
      [SynModuleOrNamespace
         ([Attribute], false, AnonModule,
          [Attributes
             ([{ Attributes =
                  [{ TypeName = SynLongIdent ([AGoodAttribute], [], [None])
                     TypeArgs =
                      [Var (SynTypar (A, HeadType, false), (1,17--1,19));
                       App
                         (LongIdent (SynLongIdent ([SomeThing], [], [None])),
                          Some (1,30--1,31),
                          [LongIdent (SynLongIdent ([int], [], [None]))], [],
                          Some (1,34--1,35), false, (1,21--1,35))]
                     ArgExpr = Const (Unit, (1,2--1,36))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = (1,2--1,36) }]
                 Range = (1,0--1,38) }], (1,0--1,38));
           Expr (Do (Const (Unit, (2,2--2,4)), (2,0--2,4)), (2,0--2,4));
           Attributes
             ([{ Attributes =
                  [{ TypeName = SynLongIdent ([AnotherAttr], [], [None])
                     TypeArgs =
                      [Var (SynTypar (a, None, false), (4,14--4,16));
                       LongIdent (SynLongIdent ([string], [], [None]));
                       Intersection
                         (None,
                          [HashConstraint
                             (LongIdent
                                (SynLongIdent ([IDisposible], [], [None])),
                              (4,25--4,37));
                           HashConstraint
                             (App
                                (LongIdent (SynLongIdent ([List], [], [None])),
                                 Some (4,45--4,46),
                                 [LongIdent (SynLongIdent ([int], [], [None]))],
                                 [], Some (4,49--4,50), false, (4,41--4,50)),
                              (4,40--4,50))], (4,25--4,50),
                          { AmpersandRanges = [(4,38--4,39)] })]
                     ArgExpr =
                      Paren
                        (Tuple
                           (false,
                            [App
                               (NonAtomic, false,
                                App
                                  (NonAtomic, true,
                                   LongIdent
                                     (false,
                                      SynLongIdent
                                        ([op_Equality], [],
                                         [Some (OriginalNotation "=")]), None,
                                      (4,61--4,62)), Ident NamedArg1,
                                   (4,52--4,62)),
                                Const
                                  (String ("Foo", Regular, (4,62--4,67)),
                                   (4,62--4,67)), (4,52--4,67));
                             App
                               (NonAtomic, false,
                                App
                                  (NonAtomic, true,
                                   LongIdent
                                     (false,
                                      SynLongIdent
                                        ([op_Equality], [],
                                         [Some (OriginalNotation "=")]), None,
                                      (4,78--4,79)), Ident NamedArg2,
                                   (4,69--4,79)),
                                Const
                                  (String ("Bar", Regular, (4,79--4,84)),
                                   (4,79--4,84)), (4,69--4,84))], [(4,67--4,68)],
                            (4,52--4,84)), (4,51--4,52), Some (4,84--4,85),
                         (4,51--4,85))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = (4,2--4,85) }]
                 Range = (4,0--4,87) }], (4,0--4,87));
           Expr (Do (Const (Unit, (5,2--5,4)), (5,0--5,4)), (5,0--5,4))],
          PreXmlDocEmpty, [], None, (1,0--5,4), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
