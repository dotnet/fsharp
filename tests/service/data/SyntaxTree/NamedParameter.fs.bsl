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
                             None, /root/NamedParameter.fs (2,3--2,4)), Ident x,
                          /root/NamedParameter.fs (2,2--2,4)),
                       Const (Int32 4, /root/NamedParameter.fs (2,4--2,5)),
                       /root/NamedParameter.fs (2,2--2,5)),
                    /root/NamedParameter.fs (2,1--2,2),
                    Some /root/NamedParameter.fs (2,5--2,6),
                    /root/NamedParameter.fs (2,1--2,6)),
                 /root/NamedParameter.fs (2,0--2,6)),
              /root/NamedParameter.fs (2,0--2,6))], PreXmlDocEmpty, [], None,
          /root/NamedParameter.fs (2,0--2,6), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))