ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfAttributeWithPath.fs", false,
      QualifiedNameOfFile RangeOfAttributeWithPath, [], [],
      [SynModuleOrNamespace
         ([RangeOfAttributeWithPath], false, AnonModule,
          [Attributes
             ([{ Attributes =
                  [{ TypeName =
                      SynLongIdent
                        ([Prefix; MyAttribute],
                         [/root/RangeOfAttributeWithPath.fs (1,8--1,9)],
                         [None; None])
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
                                  /root/RangeOfAttributeWithPath.fs (1,25--1,26)),
                               Ident foo,
                               /root/RangeOfAttributeWithPath.fs (1,21--1,26)),
                            Const
                              (String
                                 ("bar", Regular,
                                  /root/RangeOfAttributeWithPath.fs (1,26--1,31)),
                               /root/RangeOfAttributeWithPath.fs (1,26--1,31)),
                            /root/RangeOfAttributeWithPath.fs (1,21--1,31)),
                         /root/RangeOfAttributeWithPath.fs (1,20--1,21),
                         Some /root/RangeOfAttributeWithPath.fs (1,31--1,32),
                         /root/RangeOfAttributeWithPath.fs (1,20--1,32))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = /root/RangeOfAttributeWithPath.fs (1,2--1,32) }]
                 Range = /root/RangeOfAttributeWithPath.fs (1,0--1,34) }],
              /root/RangeOfAttributeWithPath.fs (1,0--1,34));
           Expr
             (Do
                (Const (Unit, /root/RangeOfAttributeWithPath.fs (2,3--2,5)),
                 /root/RangeOfAttributeWithPath.fs (2,0--2,5)),
              /root/RangeOfAttributeWithPath.fs (2,0--2,5))], PreXmlDocEmpty, [],
          None, /root/RangeOfAttributeWithPath.fs (1,0--2,5),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))