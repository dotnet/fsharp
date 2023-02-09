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
                        ([Prefix; MyAttribute],
                         [/root/Attribute/RangeOfAttributeWithPath.fs (2,8--2,9)],
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
                                  /root/Attribute/RangeOfAttributeWithPath.fs (2,25--2,26)),
                               Ident foo,
                               /root/Attribute/RangeOfAttributeWithPath.fs (2,21--2,26)),
                            Const
                              (String
                                 ("bar", Regular,
                                  /root/Attribute/RangeOfAttributeWithPath.fs (2,26--2,31)),
                               /root/Attribute/RangeOfAttributeWithPath.fs (2,26--2,31)),
                            /root/Attribute/RangeOfAttributeWithPath.fs (2,21--2,31)),
                         /root/Attribute/RangeOfAttributeWithPath.fs (2,20--2,21),
                         Some
                           /root/Attribute/RangeOfAttributeWithPath.fs (2,31--2,32),
                         /root/Attribute/RangeOfAttributeWithPath.fs (2,20--2,32))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range =
                      /root/Attribute/RangeOfAttributeWithPath.fs (2,2--2,32) }]
                 Range = /root/Attribute/RangeOfAttributeWithPath.fs (2,0--2,34) }],
              /root/Attribute/RangeOfAttributeWithPath.fs (2,0--2,34));
           Expr
             (Do
                (Const
                   (Unit, /root/Attribute/RangeOfAttributeWithPath.fs (3,3--3,5)),
                 /root/Attribute/RangeOfAttributeWithPath.fs (3,0--3,5)),
              /root/Attribute/RangeOfAttributeWithPath.fs (3,0--3,5))],
          PreXmlDocEmpty, [], None,
          /root/Attribute/RangeOfAttributeWithPath.fs (2,0--4,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))