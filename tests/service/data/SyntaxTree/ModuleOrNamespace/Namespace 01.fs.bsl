ImplFile
  (ParsedImplFileInput
     ("/root/ModuleOrNamespace/Namespace 01.fs", false,
      QualifiedNameOfFile Namespace 01, [],
      [SynModuleOrNamespace
         ([Ns1], false, DeclaredNamespace,
          [Expr (Const (Unit, (3,0--3,2)), (3,0--3,2))], PreXmlDocEmpty, [],
          None, (1,0--3,2), { LeadingKeyword = Namespace (1,0--1,9) });
       SynModuleOrNamespace
         ([Ns2; Nested], false, DeclaredNamespace,
          [Expr (Const (Unit, (7,0--7,2)), (7,0--7,2))], PreXmlDocEmpty, [],
          None, (5,0--7,2), { LeadingKeyword = Namespace (5,0--5,9) })],
      (true, true), { ConditionalDirectives = []
                      WarnDirectives = []
                      CodeComments = [] }, set []))
