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
                         [/root/RangeOfAttributeWithPath.fs (2,8--2,9)],
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
                                  /root/RangeOfAttributeWithPath.fs (2,25--2,26)),
                               Ident foo,
                               /root/RangeOfAttributeWithPath.fs (2,21--2,26)),
                            Const
                              (String
                                 ("bar", Regular,
                                  /root/RangeOfAttributeWithPath.fs (2,26--2,31)),
                               /root/RangeOfAttributeWithPath.fs (2,26--2,31)),
                            /root/RangeOfAttributeWithPath.fs (2,21--2,31)),
                         /root/RangeOfAttributeWithPath.fs (2,20--2,21),
                         Some /root/RangeOfAttributeWithPath.fs (2,31--2,32),
                         /root/RangeOfAttributeWithPath.fs (2,20--2,32))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = /root/RangeOfAttributeWithPath.fs (2,2--2,32) }]
                 Range = /root/RangeOfAttributeWithPath.fs (2,0--2,34) }],
              /root/RangeOfAttributeWithPath.fs (2,0--2,34));
           Expr
             (Do
                (Const (Unit, /root/RangeOfAttributeWithPath.fs (3,3--3,5)),
                 /root/RangeOfAttributeWithPath.fs (3,0--3,5)),
              /root/RangeOfAttributeWithPath.fs (3,0--3,5))], PreXmlDocEmpty, [],
          None, /root/RangeOfAttributeWithPath.fs (2,0--4,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))