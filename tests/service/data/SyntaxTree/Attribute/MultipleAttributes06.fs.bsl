ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/MultipleAttributes06.fs", false,
      QualifiedNameOfFile MultipleAttributes06, [], [],
      [SynModuleOrNamespace
         ([MultipleAttributes06], false, AnonModule,
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
                 Range = (2,0--3,25) }], (2,0--3,25));
           Expr
             (App
                (Atomic, false, Ident MyAttribute,
                 Paren
                   (App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_Equality], [], [Some (OriginalNotation "=")]),
                             None, (4,18--4,19)), Ident foo, (4,14--4,19)),
                       Const
                         (String ("bar", Regular, (4,19--4,24)), (4,19--4,24)),
                       (4,14--4,24)), (4,13--4,14), Some (4,24--4,25),
                    (4,13--4,25)), (4,2--4,25)), (4,2--4,25))], PreXmlDocEmpty,
          [], None, (2,0--4,25), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,2)-(4,13) parse error Unexpected identifier in attribute list
(4,25)-(4,27) parse error Unexpected symbol '>]' in implementation file
