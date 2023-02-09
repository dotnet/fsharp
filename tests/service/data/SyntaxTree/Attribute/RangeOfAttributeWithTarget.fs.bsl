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
                                  /root/Attribute/RangeOfAttributeWithTarget.fs (2,28--2,29)),
                               Ident foo,
                               /root/Attribute/RangeOfAttributeWithTarget.fs (2,24--2,29)),
                            Const
                              (String
                                 ("bar", Regular,
                                  /root/Attribute/RangeOfAttributeWithTarget.fs (2,29--2,34)),
                               /root/Attribute/RangeOfAttributeWithTarget.fs (2,29--2,34)),
                            /root/Attribute/RangeOfAttributeWithTarget.fs (2,24--2,34)),
                         /root/Attribute/RangeOfAttributeWithTarget.fs (2,23--2,24),
                         Some
                           /root/Attribute/RangeOfAttributeWithTarget.fs (2,34--2,35),
                         /root/Attribute/RangeOfAttributeWithTarget.fs (2,23--2,35))
                     Target = Some assembly
                     AppliesToGetterAndSetter = false
                     Range =
                      /root/Attribute/RangeOfAttributeWithTarget.fs (2,2--2,35) }]
                 Range =
                  /root/Attribute/RangeOfAttributeWithTarget.fs (2,0--2,37) }],
              /root/Attribute/RangeOfAttributeWithTarget.fs (2,0--2,37));
           Expr
             (Do
                (Const
                   (Unit,
                    /root/Attribute/RangeOfAttributeWithTarget.fs (3,3--3,5)),
                 /root/Attribute/RangeOfAttributeWithTarget.fs (3,0--3,5)),
              /root/Attribute/RangeOfAttributeWithTarget.fs (3,0--3,5))],
          PreXmlDocEmpty, [], None,
          /root/Attribute/RangeOfAttributeWithTarget.fs (2,0--4,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))