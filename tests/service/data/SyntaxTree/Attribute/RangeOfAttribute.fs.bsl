ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/RangeOfAttribute.fs", false,
      QualifiedNameOfFile RangeOfAttribute, [], [],
      [SynModuleOrNamespace
         ([RangeOfAttribute], false, AnonModule,
          [Attributes
             ([{ Attributes =
                  [{ TypeName = SynLongIdent ([MyAttribute], [], [None])
                     TypeArgs = []
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
                                  (2,18--2,19)), Ident foo, (2,14--2,19)),
                            Const
                              (String ("bar", Regular, (2,20--2,25)),
                               (2,20--2,25)), (2,14--2,25)), (2,13--2,14),
                         Some (2,25--2,26), (2,13--2,26))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = (2,2--2,26) }]
                 Range = (2,0--2,28) }], (2,0--2,28));
           Expr (Do (Const (Unit, (3,3--3,5)), (3,0--3,5)), (3,0--3,5))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
