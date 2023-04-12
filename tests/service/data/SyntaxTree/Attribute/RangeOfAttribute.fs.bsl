ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/RangeOfAttribute.fs", false,
      QualifiedNameOfFile RangeOfAttribute, [], [],
      [SynModuleOrNamespace
         ([RangeOfAttribute], false, AnonModule,
          [Attributes
             ([{ Attributes =
                  [{ TypeName = SynLongIdent ([MyAttribute], [], [None])
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
                              (String ("bar", Regular, (2,19--2,24)),
                               (2,19--2,24)), (2,14--2,24)), (2,13--2,14),
                         Some (2,24--2,25), (2,13--2,25))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = (2,2--2,25) }]
                 Range = (2,0--2,27) }], (2,0--2,27));
           Expr (Do (Const (Unit, (3,3--3,5)), (3,0--3,5)), (3,0--3,5))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
