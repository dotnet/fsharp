ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/OpenTypeOnSameIndent2.fs", false,
      QualifiedNameOfFile OpenTypeOnSameIndent2, [],
      [SynModuleOrNamespace
         ([OpenTypeOnSameIndent2], false, AnonModule,
          [Expr
             (Paren
                (Open
                   (ModuleOrNamespace (SynLongIdent ([], [], []), (1,5--1,5)),
                    (1,1--1,5), (1,1--2,12), Ident System), (1,0--1,1),
                 Some (2,12--2,13), (1,0--2,13)), (1,0--2,13))], PreXmlDocEmpty,
          [], None, (1,0--2,13), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(1,6)-(2,1) parse error Incomplete structured construct at or before this point in open declaration. Expected identifier, 'global', 'type' or other token.
