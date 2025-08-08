ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/OpenTypeOnSameIndent.fs", false,
      QualifiedNameOfFile OpenTypeOnSameIndent, [],
      [SynModuleOrNamespace
         ([OpenTypeOnSameIndent], false, AnonModule,
          [Open
             (ModuleOrNamespace (SynLongIdent ([], [], []), (1,4--1,4)),
              (1,0--1,4));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [System],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,11)),
                  Simple (None (2,5--2,11), (2,5--2,11)), [], None, (2,5--2,11),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = None
                    WithKeyword = None })], (2,0--2,11))], PreXmlDocEmpty, [],
          None, (1,0--2,11), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(1,5)-(2,0) parse error Incomplete structured construct at or before this point in open declaration. Expected identifier, 'global', 'type' or other token.
