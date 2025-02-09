ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/MultipleAttributes01.fs", false,
      QualifiedNameOfFile MultipleAttributes01, [], [],
      [SynModuleOrNamespace
         ([MultipleAttributes01], false, AnonModule,
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
                                  (2,43--2,44)), Ident foo, (2,39--2,44)),
                            Const
                              (String ("bar", Regular, (2,44--2,49)),
                               (2,44--2,49)), (2,39--2,49)), (2,38--2,39),
                         Some (2,49--2,50), (2,38--2,50))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = (2,27--2,50) }]
                 Range = (2,0--2,52) }], (2,0--2,52));
           Expr (Do (Const (Unit, (3,3--3,5)), (3,0--3,5)), (3,0--3,5))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
