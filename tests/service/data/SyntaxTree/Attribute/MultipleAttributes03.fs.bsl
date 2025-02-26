ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/MultipleAttributes03.fs", false,
      QualifiedNameOfFile MultipleAttributes03, [], [],
      [SynModuleOrNamespace
         ([MultipleAttributes03], false, AnonModule,
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
                     Range = (2,2--2,25) };
                   { TypeName = SynLongIdent ([MyAttribute], [], [None])
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
                                  (3,18--3,19)), Ident foo, (3,14--3,19)),
                            Const
                              (String ("bar", Regular, (3,19--3,24)),
                               (3,19--3,24)), (3,14--3,24)), (3,13--3,14),
                         Some (3,24--3,25), (3,13--3,25))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = (3,2--3,25) }]
                 Range = (2,0--3,27) }], (2,0--3,27));
           Expr (Do (Const (Unit, (4,3--4,5)), (4,0--4,5)), (4,0--4,5))],
          PreXmlDocEmpty, [], None, (2,0--5,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
