ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/RangeOfAttribute.fs", false,
      QualifiedNameOfFile RangeOfAttribute, [], [],
      [SynModuleOrNamespace
         ([RangeOfAttribute], false, AnonModule,
          [Attributes
             ([{ Attributes =
                  [{ TypeName = SynLongIdent ([MyAttribute], [], [None])
                     TypeArgs =
                      [Var (SynTypar (T, HeadType, false), (2,14--2,16));
                       LongIdent (SynLongIdent ([int], [], [None]))]
                     ArgExpr =
                      Paren
                        (App
                           (NonAtomic, false,
                            App
                              (NonAtomic, true,
                               LongIdent
                                 (false,
                                  SynLongIdent
                                    ([op_Equality], [],
                                     [Some (OriginalNotation "=")]), None,
                                  (2,27--2,28)), Ident foo, (2,23--2,28)),
                            Const
                              (String ("bar", Regular, (2,29--2,34)),
                               (2,29--2,34)), (2,23--2,34)), (2,22--2,23),
                         Some (2,34--2,35), (2,22--2,35))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = (2,2--2,35) }]
                 Range = (2,0--2,37) }], (2,0--2,37));
           Expr (Do (Const (Unit, (3,3--3,5)), (3,0--3,5)), (3,0--3,5))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
