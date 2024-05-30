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
                     ArgExpr = Const (Unit, (1,2--1,16))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = (1,2--1,16) }]
                 Range = (1,0--1,37) }], (1,0--1,37));
           Expr (Do (Const (Unit, (2,2--2,4)), (2,0--2,4)), (2,0--2,4));
           Attributes
             ([{ Attributes =
                  [{ TypeName = SynLongIdent ([AnotherAttr], [], [None])
                     TypeArgs =
                      [Var (SynTypar (a, None, false), (4,14--4,16));
                       LongIdent (SynLongIdent ([string], [], [None]));
                       Intersection
                         (Some (SynTypar (T, HeadType, false)),
                          [App
                             (LongIdent (SynLongIdent ([List], [], [None])),
                              Some (4,33--4,34),
                              [LongIdent (SynLongIdent ([int], [], [None]))], [],
                              Some (4,37--4,38), false, (4,29--4,38))],
                          (4,24--4,38), { AmpersandRanges = [(4,27--4,28)] })]
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
                                      (4,49--4,50)), Ident NamedArg1,
                                   (4,40--4,50)),
                                Const
                                  (String ("Foo", Regular, (4,50--4,55)),
                                   (4,50--4,55)), (4,40--4,55));
                             App
                               (NonAtomic, false,
                                App
                                  (NonAtomic, true,
                                   LongIdent
                                     (false,
                                      SynLongIdent
                                        ([op_Equality], [],
                                         [Some (OriginalNotation "=")]), None,
                                      (4,66--4,67)), Ident NamedArg2,
                                   (4,57--4,67)),
                                Const
                                  (String ("Bar", Regular, (4,67--4,72)),
                                   (4,67--4,72)), (4,57--4,72))], [(4,55--4,56)],
                            (4,40--4,72)), (4,39--4,40), Some (4,72--4,73),
                         (4,39--4,73))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = (4,2--4,73) }]
                 Range = (4,0--4,75) }], (4,0--4,75))], PreXmlDocEmpty, [], None,
          (1,0--4,75), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(1,36)-(1,37) parse error Unexpected symbol '>' in attribute list
(4,29)-(4,38) parse error Constraint intersection syntax may only be used with flexible types, e.g. '#IDisposable & #ISomeInterface'.
(4,0)-(4,75) parse error Cannot find code target for this attribute, possibly because the code after the attribute is incomplete.
