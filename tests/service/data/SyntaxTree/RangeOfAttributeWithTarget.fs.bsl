ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfAttributeWithTarget.fs", false,
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
                                  /root/RangeOfAttributeWithTarget.fs (1,28--1,29)),
                               Ident foo,
                               /root/RangeOfAttributeWithTarget.fs (1,24--1,29)),
                            Const
                              (String
                                 ("bar", Regular,
                                  /root/RangeOfAttributeWithTarget.fs (1,29--1,34)),
                               /root/RangeOfAttributeWithTarget.fs (1,29--1,34)),
                            /root/RangeOfAttributeWithTarget.fs (1,24--1,34)),
                         /root/RangeOfAttributeWithTarget.fs (1,23--1,24),
                         Some /root/RangeOfAttributeWithTarget.fs (1,34--1,35),
                         /root/RangeOfAttributeWithTarget.fs (1,23--1,35))
                     Target = Some assembly
                     AppliesToGetterAndSetter = false
                     Range = /root/RangeOfAttributeWithTarget.fs (1,2--1,35) }]
                 Range = /root/RangeOfAttributeWithTarget.fs (1,0--1,37) }],
              /root/RangeOfAttributeWithTarget.fs (1,0--1,37));
           Expr
             (Do
                (Const (Unit, /root/RangeOfAttributeWithTarget.fs (2,3--2,5)),
                 /root/RangeOfAttributeWithTarget.fs (2,0--2,5)),
              /root/RangeOfAttributeWithTarget.fs (2,0--2,5))], PreXmlDocEmpty,
          [], None, /root/RangeOfAttributeWithTarget.fs (1,0--2,5),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))