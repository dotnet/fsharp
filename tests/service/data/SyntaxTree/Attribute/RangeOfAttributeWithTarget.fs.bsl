ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/RangeOfAttributeWithTarget.fs", false,
      QualifiedNameOfFile RangeOfAttributeWithTarget, [], [],
      [SynModuleOrNamespace
         ([RangeOfAttributeWithTarget], false, AnonModule,
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
                                  (2,28--2,29)), Ident foo, (2,24--2,29)),
                            Const
                              (String ("bar", Regular, (2,29--2,34)),
                               (2,29--2,34)), (2,24--2,34)), (2,23--2,24),
                         Some (2,34--2,35), (2,23--2,35))
                     Target = Some assembly
                     AppliesToGetterAndSetter = false
                     Range = (2,2--2,35) }]
                 Range = (2,0--2,37) }], (2,0--2,37));
           Expr (Do (Const (Unit, (3,3--3,5)), (3,0--3,5)), (3,0--3,5))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
