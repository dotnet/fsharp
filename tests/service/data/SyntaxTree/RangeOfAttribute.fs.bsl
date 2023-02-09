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
                                  /root/RangeOfAttribute.fs (2,18--2,19)),
                               Ident foo, /root/RangeOfAttribute.fs (2,14--2,19)),
                            Const
                              (String
                                 ("bar", Regular,
                                  /root/RangeOfAttribute.fs (2,19--2,24)),
                               /root/RangeOfAttribute.fs (2,19--2,24)),
                            /root/RangeOfAttribute.fs (2,14--2,24)),
                         /root/RangeOfAttribute.fs (2,13--2,14),
                         Some /root/RangeOfAttribute.fs (2,24--2,25),
                         /root/RangeOfAttribute.fs (2,13--2,25))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = /root/RangeOfAttribute.fs (2,2--2,25) }]
                 Range = /root/RangeOfAttribute.fs (2,0--2,27) }],
              /root/RangeOfAttribute.fs (2,0--2,27));
           Expr
             (Do
                (Const (Unit, /root/RangeOfAttribute.fs (3,3--3,5)),
                 /root/RangeOfAttribute.fs (3,0--3,5)),
              /root/RangeOfAttribute.fs (3,0--3,5))], PreXmlDocEmpty, [], None,
          /root/RangeOfAttribute.fs (2,0--4,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))