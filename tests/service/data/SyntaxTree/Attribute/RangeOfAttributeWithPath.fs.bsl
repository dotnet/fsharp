ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/RangeOfAttributeWithPath.fs", false,
      QualifiedNameOfFile RangeOfAttributeWithPath, [], [],
      [SynModuleOrNamespace
         ([RangeOfAttributeWithPath], false, AnonModule,
          [Attributes
             ([{ Attributes =
                  [{ TypeName =
                      SynLongIdent
                        ([Prefix; MyAttribute], [(2,8--2,9)], [None; None])
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
                                  (2,25--2,26)), Ident foo, (2,21--2,26)),
                            Const
                              (String ("bar", Regular, (2,26--2,31)),
                               (2,26--2,31)), (2,21--2,31)), (2,20--2,21),
                         Some (2,31--2,32), (2,20--2,32))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = (2,2--2,32) }]
                 Range = (2,0--2,34) }], (2,0--2,34));
           Expr (Do (Const (Unit, (3,3--3,5)), (3,0--3,5)), (3,0--3,5))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
