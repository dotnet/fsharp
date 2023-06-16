ImplFile
  (ParsedImplFileInput
     ("/root/ModuleMember/Open 01.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Open
             (ModuleOrNamespace (SynLongIdent ([], [], []), (3,4--3,4)),
              (3,0--3,4));
           Open
             (ModuleOrNamespace (SynLongIdent ([Ns1], [], [None]), (4,5--4,8)),
              (4,0--4,8))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,8), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,5)-(4,0) parse error Incomplete structured construct at or before this point in open declaration. Expected identifier, 'global', 'type' or other token.
