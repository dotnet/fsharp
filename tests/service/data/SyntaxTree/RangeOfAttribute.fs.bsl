ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfAttribute.fs", false, QualifiedNameOfFile RangeOfAttribute,
      [], [],
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
                                  /root/RangeOfAttribute.fs (1,18--1,19)),
                               Ident foo, /root/RangeOfAttribute.fs (1,14--1,19)),
                            Const
                              (String
                                 ("bar", Regular,
                                  /root/RangeOfAttribute.fs (1,19--1,24)),
                               /root/RangeOfAttribute.fs (1,19--1,24)),
                            /root/RangeOfAttribute.fs (1,14--1,24)),
                         /root/RangeOfAttribute.fs (1,13--1,14),
                         Some /root/RangeOfAttribute.fs (1,24--1,25),
                         /root/RangeOfAttribute.fs (1,13--1,25))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = /root/RangeOfAttribute.fs (1,2--1,25) }]
                 Range = /root/RangeOfAttribute.fs (1,0--1,27) }],
              /root/RangeOfAttribute.fs (1,0--1,27));
           Expr
             (Do
                (Const (Unit, /root/RangeOfAttribute.fs (2,3--2,5)),
                 /root/RangeOfAttribute.fs (2,0--2,5)),
              /root/RangeOfAttribute.fs (2,0--2,5))], PreXmlDocEmpty, [], None,
          /root/RangeOfAttribute.fs (1,0--2,5), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))