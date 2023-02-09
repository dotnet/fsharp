ImplFile
  (ParsedImplFileInput
     ("/root/NamedParameter.fs", false, QualifiedNameOfFile NamedParameter, [],
      [],
      [SynModuleOrNamespace
         ([NamedParameter], false, AnonModule,
          [Expr
             (App
                (Atomic, false, Ident f,
                 Paren
                   (App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_Equality], [], [Some (OriginalNotation "=")]),
                             None, /root/NamedParameter.fs (1,3--1,4)), Ident x,
                          /root/NamedParameter.fs (1,2--1,4)),
                       Const (Int32 4, /root/NamedParameter.fs (1,4--1,5)),
                       /root/NamedParameter.fs (1,2--1,5)),
                    /root/NamedParameter.fs (1,1--1,2),
                    Some /root/NamedParameter.fs (1,5--1,6),
                    /root/NamedParameter.fs (1,1--1,6)),
                 /root/NamedParameter.fs (1,0--1,6)),
              /root/NamedParameter.fs (1,0--1,6))], PreXmlDocEmpty, [], None,
          /root/NamedParameter.fs (1,0--1,6), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))