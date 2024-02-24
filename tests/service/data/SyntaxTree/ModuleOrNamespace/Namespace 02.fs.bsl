ImplFile
  (ParsedImplFileInput
     ("/root/ModuleOrNamespace/Namespace 02.fs", false,
      QualifiedNameOfFile Namespace 02, [], [],
      [SynModuleOrNamespace
         ([Ns1], false, DeclaredNamespace,
          [Expr (Const (Unit, (3,0--3,2)), (3,0--3,2))], PreXmlDocEmpty, [],
          None, (1,0--3,2), { LeadingKeyword = Namespace (1,0--1,9) });
       SynModuleOrNamespace
         ([], false, DeclaredNamespace,
          [Expr (Const (Unit, (7,0--7,2)), (7,0--7,2))], PreXmlDocEmpty, [],
          None, (5,0--7,2), { LeadingKeyword = Namespace (5,0--5,9) })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(7,0)-(7,1) parse error Unexpected start of structured construct in implementation file. Expected identifier, 'global' or other token.
