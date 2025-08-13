ImplFile
  (ParsedImplFileInput
     ("/root/Type/Exception Inside Type 01.fs", false,
      QualifiedNameOfFile Exception Inside Type 01, [],
      [SynModuleOrNamespace
         ([Exception Inside Type 01], false, AnonModule, [], PreXmlDocEmpty, [],
          None, (7,0--7,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,46)] }, set []))

(6,4)-(6,13) parse warning Exceptions must be defined at module level, not inside types.
(6,4)-(6,13) parse error Unexpected keyword 'exception' in member definition
