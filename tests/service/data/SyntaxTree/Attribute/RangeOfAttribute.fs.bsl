ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/RangeOfAttribute.fs", false,
      QualifiedNameOfFile RangeOfAttribute, [], [],
      [SynModuleOrNamespace
         ([RangeOfAttribute], false, AnonModule,
          [Attributes
             ([{ Attributes =
                  [{ TypeName = SynLongIdent ([MyAttribute], [], [None])
                     TypeParams =
                      Some
                        (PostfixList
                           ([SynTyparDecl
                               ([], SynTypar (T, HeadType, false), [],
                                { AmpersandRanges = [] });
                             SynTyparDecl
                               ([], SynTypar (S, HeadType, false), [],
                                { AmpersandRanges = [] })], [], (2,13--2,21)))
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
                                  (2,26--2,27)), Ident foo, (2,22--2,27)),
                            Const
                              (String ("bar", Regular, (2,28--2,33)),
                               (2,28--2,33)), (2,22--2,33)), (2,21--2,22),
                         Some (2,33--2,34), (2,21--2,34))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = (2,2--2,34) }]
                 Range = (2,0--2,36) }], (2,0--2,36));
           Expr (Do (Const (Unit, (3,3--3,5)), (3,0--3,5)), (3,0--3,5))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
