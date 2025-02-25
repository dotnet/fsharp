ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/MultipleAttributes05.fs", false,
      QualifiedNameOfFile MultipleAttributes05, [], [],
      [SynModuleOrNamespace
         ([MultipleAttributes05], false, AnonModule,
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
                                  (3,20--3,21)), Ident foo, (3,16--3,21)),
                            Const
                              (String ("bar", Regular, (3,21--3,26)),
                               (3,21--3,26)), (3,16--3,26)), (3,15--3,16),
                         Some (3,26--3,27), (3,15--3,27))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = (3,4--3,27) }]
                 Range = (2,0--3,27) }], (2,0--3,27));
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
