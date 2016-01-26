module RunPl

open System

open PlatformHelpers

let TODO f = ignore f

open System.Text.RegularExpressions

let (|Regex|_|) pattern input =
    let m = Regex.Match(input, pattern)
    if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
    else None

let lc (s: string) = s.ToLower()

(** PERL NOTES

`-e $path`
mean file exists at $path ( or a directory/named pipe/symlink )

`my 	( $_sources, $_SCFLAGS ) = @_;`
deconstruct arguments of function ( @_ ) into variables $_sources, $_SCFLAGS

**)


//use strict;
//use IO::Socket;
//use Cwd;
ignore "useless requires"
    
//use constant COMPILER_NAME => 'fsc';
let COMPILER_NAME = "fsc"
//use constant FSI_NAME      => 'fsiAnyCPU';
let FSI_NAME      = "fsiAnyCPU"
//use constant FSI32_NAME    => 'fsi';
let FSI32_NAME    = "fsi"
//use constant CSC_NAME      => 'csc';
let CSC_NAME      = "csc"
//use constant VBC_NAME      => 'vbc';
let VBC_NAME      = "vbc"
    
// # Constant values for test result
//use constant TEST_PASS     => 0;
let TEST_PASS     = 0
//use constant TEST_FAIL     => 1;
let TEST_FAIL     = 1
//use constant TEST_SKIPPED  => 2;
let TEST_SKIPPED  = 2
//use constant TEST_CASCADE  => 3;
let TEST_CASCADE  = 3
//use constant TEST_NORESULT => 4;
let TEST_NORESULT = 4
    
// # Constant values for target type
//use constant TARGET_EXE => 0;
let TARGET_EXE = 0
//use constant TARGET_DLL => 1;
let TARGET_DLL = 1
//use constant TARGET_MOD => 2;
let TARGET_MOD = 2
    
// # Constant values for platform type
//use constant PLATFORM_X86 => 1;
let PLATFORM_X86 = 1
//use constant PLATFORM_IA64 => 2;
let PLATFORM_IA64 = 2
//use constant PLATFORM_AMD64 => 3;
let PLATFORM_AMD64 = 3
//use constant PLATFORM_WIN9X => 4;
let PLATFORM_WIN9X = 4
//use constant PLATFORM_WOW_IA64 => 5;
let PLATFORM_WOW_IA64 = 5
//use constant PLATFORM_WOW_AMD64 => 6;
let PLATFORM_WOW_AMD64 = 6
    
    
    
//# Constant values used internally to determine if the compile/run should succeed or fail
//use constant TEST_SEEK_SUCCESS  => 0;
let TEST_SEEK_SUCCESS  = 0
//use constant TEST_SEEK_WARN     => 1;
let TEST_SEEK_WARN     = 1
//use constant TEST_SEEK_ERROR    => 2;
let TEST_SEEK_ERROR    = 2

//use constant ASSERT_FILE => '_assert.$$$'; # where we store the VSASSERT file
let ASSERT_FILE = "_assert.$$$" // # where we store the VSASSERT file


// ################################################################################
// #
// # SUB ROUTINES
// #
// ################################################################################

// #############################################################
// # RunCommand -- execute a cmd, redirecting stdout, stderr.
// #
// # Redirects STDERR to STDOUT, and then redirects STDOUT to the
// # argument named in $redirect.  It is done this way since
// # invoking system() with i/o redirection under Win9x masks
// # the return code, always yielding a 0.
// #
// # The return value is the actual return value from the test.
// #
//sub RunCommand {
let RunCommand cwd envVars msg (exe, cmdArgs) dumpOutput = attempt {
    let unlink = Commands.rm cwd
    let fileExists = Commands.fileExists cwd
    let getfullpath = Commands.getfullpath cwd
    let ``exec 2>1 1>a`` a = NUnitConf.Command.exec cwd envVars { Output = NUnitConf.RedirectTo.OutputAndError(NUnitConf.RedirectToType.Overwrite(a)); Input = None; }

    // #add Win9x Hack here
    
    //unlink ASSERT_FILE;
    unlink ASSERT_FILE

    //my ($msg,$cmd,$dumpOutput) = @_;
    ignore "arguments"
    
    // #  open SAVEERR, ">&STDERR"; open STDOUT, ">&STDOUT"; 	# save a copy of stderr and redirect to stdout
    
    //print("$msg: [$cmd]\n");
    printfn "%s: [%s]\n" msg (sprintf "%s %s" exe cmdArgs)

    //select STDERR; $| = 1; select STDOUT; $| = 1;		# enable autoflush
    //open(COMMAND,"$cmd 2>&1 |") or RunExit(TEST_FAIL, "Command Process Couldn't Be Created: $! Returned $? \n");
    //@CommandOutput = <COMMAND>;
    //close COMMAND;
    let tempOut = IO.Path.GetTempFileName()
    let result = ``exec 2>1 1>a`` tempOut exe cmdArgs
    let cmdExitCode = match result with CmdResult.ErrorLevel(x) -> x | CmdResult.Success -> 0
    let CommandOutput = tempOut |> IO.File.ReadAllText

    // #  close STDERR; open STDERR, ">&SAVEERR"; #resore stderr
    
    //print @CommandOutput if ($dumpOutput == 1);
    do if dumpOutput then printfn "%s" CommandOutput
    
    // # Test for an assertion failure
    //if (-e ASSERT_FILE) {
    return! 
        if fileExists ASSERT_FILE |> Option.isSome then
            //print("Failing Test: Assertion detected. Dump Follows:\n");
            printfn "Failing Test: Assertion detected. Dump Follows:"

            //open ASSERT, ASSERT_FILE or RunExit(TEST_SKIPPED, "Can't open:" . ASSERT_FILE . " : $!\n");
            //while (<ASSERT>){ print; }
            //close ASSERT;
            try
                ASSERT_FILE
                |> getfullpath
                |> System.IO.File.ReadLines
                |> Seq.iter (printfn "%s")
                NUnitConf.genericError "Command Unexpectedly Failed with ASSERT \n"
            with e ->
                NUnitConf.skip (sprintf "Can't open: %s : %s" ASSERT_FILE (e.Message))
        else
            succeed (cmdExitCode, CommandOutput)
    }

// #############################################################
// # RunCompilerCommand -- runs a compiler command line, either directly or through external host
// #
let RunCompilerCommand cwd envVars useHosted msg (exe, cmd) dumpOutput =
    //my ($useHosted, $msg, $cmd, $dumpOutput) = @_;
    ignore "are arguments"
    
    //if($useHosted){
    if useHosted then
        //my $port = "11000";
        //if($ENV{HOSTED_COMPILER_PORT} ne ""){
        //   $port = $ENV{HOSTED_COMPILER_PORT};
        //}
        //
        //my $attempts = 0;
        //my $remote = undef;
        //until($remote || ($attempts == 10)) {
        //    $remote = IO::Socket::INET->new(
        //                        Proto    => "tcp",
        //                        PeerAddr => "localhost",
        //                        PeerPort => $port,
        //                    ) or sleep(1);
        //    $attempts++;                            
        //}
        //RunExit(TEST_FAIL, "Unable to connect to hosted compiler \n") unless $remote;
        //
        //my $currDir = getcwd();
        //
        //# send current directory and full command line to the compiler host process
        //print $remote "$currDir|||$compiler_command";
        //
        //# first line of respone is the exit code
        //my $ExitCode = 0 + <$remote>;
        //
        //# remainder of response is output of compiler
        //@CommandOutput = <$remote>;
        //
        //# still some issues with reliability of hosted compiler.
        //# if compilation unexpectedly fails, try again with standard compiler
        //if ($ExitCode && ($Type < TEST_SEEK_ERROR)) {
        //  return RunCommand($msg, $cmd);
        //}
        //
        //return $ExitCode;
        TODO "useHosted not supported, let's run the command as is"

        RunCommand cwd envVars msg (exe, cmd) dumpOutput

    //}else{
    else
        //return RunCommand($msg, $cmd);
        RunCommand cwd envVars msg (exe, cmd) dumpOutput
    //}

// #############################################################
// # GetSrc -- Find the source file to build
// #
//sub GetSrc() {
let GetSrc cwd (envVarSOURCE: string) = attempt {
    //my $cwd = cwd();
    ignore "from arguments"
    
    // # The environment SOURCE var usually defines what to compile
    //$_ = $ENV{SOURCE};
    let mutable s = envVarSOURCE

    //s/\$CWD/$cwd/;
    s <- s.Replace("$CWD", cwd)

    //my $source = $_;
    let source = s

    //return($source) if defined($source);
    return! 
        if not(System.String.IsNullOrWhiteSpace(source)) then
            succeed source
        else
            // # Or if there's only one source file in the directory
            //my @src = glob("*.fs *.fsx *.fsscript");
            //@src <= 1 || RunExit(TEST_SKIPPED, "No SOURCE env var and > 1 source files in the dir: @src \n");
            //return(shift @src);
            TODO "choose a file from directory"
            //failwith "SOURCE var is required, choose a file from directory is not supported"
            NUnitConf.skip "SOURCE var is required, choose a file from directory is not supported"

    }

type ExpectedResults =
     | CmdLine of string
     | ExpectMatch of string * string
     | ExpectNotMatch of string
     | ExeOutputMatch of string list

// #############################################################
// # GetExpectedResults -- 
// #
// # This routine scans the source for magic cookies that show
// # the expected results of the compile.  The format of a cookie
// # line is:
// # //# Expects: [success|warning|error|skip|notin] : [optional text to search for]
// # or
// # //<Expects Status=[success|warning|error|skip|notin]> [optional text to search for]</Expects>
// # or
// # //<Expects Status=[success|warning|error|skip|notin]/>
// #
// # The second colon is not required if there is no text to search for.
// # case is insensitive for success|warning|error.  Note that there is
// # no semantic difference between success and warning. It's strictly
// # for readability in the source.
// #
// # Skip is a special state that has higher priority than even Error.
// # This allows a skip expectation to be added without removing or
// # editing any success, warning or error states.  This will be most
// # useful when developing tests for features NYI, or, perhaps, for
// # features with known bug entries that you don't want showing
// # up on the failure list.  Runall will be given a skip status for
// # this test.
// #
// # Note that multiple 'Expects' lines are legal. The most severe
// # status wins. If there are 23 success tags and one error tag, then
// # error is the assumed condition, and they all might as well have
// # said error.  This is useful for documentation purposes if you have
// # a file that has 10 warnings, and 2 errors and you want it to be
// # clear in the source 'Expects' line.
// #
// # '//# Expects:' is a literal to make it readable in the source.
// #
// # Examples:
// # //# Expects: Error
// #		Compile should fail. No other criteria.
// #
// # //# Expects: Success
// #		Compile should succeed. No other criteria.
// #
// # //# Expects: Warning: warning C4244: '=' : conversion from 'int' to 'char', possible loss of data
// # //# Expects: Warning: warning C4508: 'main' : function should return a value; 'void' return type assumed
// #		This will cause run.pl to expect an executable and expect it to run successfully.
// #		Compilation will only be considered successful if both of the strings after
// #		Warning: are found.  If both strings are not found, the executable is not run.
// #		If the above had been errors instead of warnings, it would not look for
// #		an executable.
// #
// # Getting the OUTPUT
// # A source file also documents its expected output.  It does so
// # in the style of a perl here document.  The startup line takes the
// # form '//[optional white space]<<[optional white space][string]
// # followed by the expected output, exactly as expected.  No variable
// # substitution currently, and newline occur as they will in the output.
// # Then on a blank line by itself: [string] is again placed.
// # Please make sure the closing line has no white space before or
// # after it.  It will be stripped from the front and back of the source.
//sub GetExpectedResults(){
let GetExpectedResults cwd (srcListSepByBlank: string) =
    let getfullpath = Commands.getfullpath cwd

    //my $src = shift @_;
    let mutable src = srcListSepByBlank

    //my $TEST_SEEK_SKIP = 99;
    let TEST_SEEK_SKIP = 99
    //my $_skip = 0;
    let _skip = 0
    //my $level;
    let level = 0
    //my $expect = TEST_SEEK_SUCCESS;
    let expect = TEST_SEEK_SUCCESS
    //my (@expected, @dontmatch);
    TODO "declare but not assigned"

    //my %seekHash = ( "success", TEST_SEEK_SUCCESS,
    //     "warning", TEST_SEEK_WARN,
    //     "error",   TEST_SEEK_ERROR,
    //     "skip",    $TEST_SEEK_SKIP
    //   );
    let seekHash = 
       [ "success", TEST_SEEK_SUCCESS
         "warning", TEST_SEEK_WARN
         "error",   TEST_SEEK_ERROR
         "skip",    TEST_SEEK_SKIP
       ] |> Map.ofList

    //$src =~ s/\s.*//; #grab only the first source file
    let first,_ = src |> PlatformHelpers.splitAtFirst System.Char.IsWhiteSpace
    src <- first //grab only the first source file
    
    let srcPath = src |> getfullpath

    //open SRC, $src or RunExit(TEST_FAIL, "GetExpectedResults::Can't open source file: $src: $!\n");
    let SRC () = srcPath |> System.IO.File.ReadLines

    // parse //<<Output
    let Output =
        SRC ()
        |> Seq.skipWhile ((<>) "//<<Output")
        |> List.ofSeq
        |> function x :: xs -> xs | [] -> []
        |> List.takeWhile ((<>) "//Output")
        |> List.map (fun line -> line.TrimStart('/'))

    //##########################################################
    
    //push @dontmatch, "internal error";
    ignore "useless, it's a failfast"
    
    //let's simplify a bit the loop below, it's xml after //
    let parseXml (s: string) = 
        try
            Choice1Of2 (System.Xml.Linq.XElement.Parse(s))
        with e ->
            Choice2Of2 e

    //ITEM: while(<SRC>) {
    SRC ()
    |> Seq.map (fun line -> line.TrimStart()) //ignore whitespace before //
    |> Seq.choose (fun line -> if line.StartsWith("//") then Some (line.TrimStart('/')) else None) //only comments `//`
    |> Seq.choose (fun line ->
        // # Looking for output tags
        //if (m@//\s*<<\s*(\S+\n)@i) {
        //  my $here = $1;
        //  while(<SRC>){
        //    s@^\s*//@@;
        //    next ITEM if ($here eq $_);
        //    push @expected, $_;
        //  }
        //  # Detect unterminated expected output tags
        //  RunExit(TEST_FAIL, "Unterminated output mark: $here  \n");
        //}
        TODO "for example <<OUTPUT, multiline, not implemented"

        // #####################################################
        // # Rip out the status and search tag (if there is one)
        //next unless m@\s*//[#<\s*]@;
        ignore "useless, Seq.choose iterate the sequence"
            
        // # Test first form
        //if (m@//#\s*Expect\w*\s*:\s*(success|warning|error|skip)\s*:?\s*(.*?)\s*$@i) {
        //  if ($TEST_SEEK_SKIP == $seekHash{$level = lc($1)}) {
        //    $Skip_platforms = uc($2) if $2
        //  } else {
        //    push @match, $2 if $2;
        //  }
        //}
        ignore "first form //# it's unused in fsharpqa tests"

        // # test first form
        //elsif (m@//#\s*Expect\w*\s*:\s*(notin)\s*:?\s*(.*?)\s*$@i) {
        //    push @dontmatch, $2 if $2;
        //}
        ignore "first form //# it's unused in fsharpqa tests"

        //let's parse xml data

        match line |> parseXml with
        | Choice2Of2 e ->

            if line.TrimStart().StartsWith("<CmdLine") then
                Some (Choice2Of2 (sprintf "invalid xml for probable xml '%s' ( file '%s') " line src))
            elif line.TrimStart().StartsWith("<Expect") then
                Some (Choice2Of2 (sprintf "invalid xml for probable xml '%s' ( file '%s') " line src))
            else
                None

        | Choice1Of2 xml ->
            let xn name = System.Xml.Linq.XName.Get(name)
            let tryAttr name (x: System.Xml.Linq.XElement) = 
                x.Attributes(xn name) |> Seq.tryHead |> Option.map (fun a -> a.Value)

            match xml.Name.LocalName with

            // # test for command lines
            // # test full xml form
            //elsif (m@//<CmdLine>\s*(.*?)\s*<(/CmdLine|/)>@i) {
            | "CmdLine" ->
              // if (defined($CmdLine)) # Currently support one command line param
              // {
              //   RunExit(TEST_SKIPPED, "<CmdLine> tag found more than once \n");
              // }
              TODO "add this check after the list is processed"

              let d1 = xml.Value
              //$CmdLine = $1 if defined($1);
              Some (Choice1Of2(CmdLine(d1)))
            //}

            // # test full xml form
            | "Expect"
            | "Expects" ->
            //elsif (m@//\s*<Expect\w*\s*.*?\s*Status\s*=\s*\"?(success|warning|error|skip)\"?\s*.*?\s*.?>\s*(.*?)\s*<(/Expect|/)\w*>@i) {

                match xml |> tryAttr "status" |> Option.map lc with
                | None ->
                    TODO "status has a default?"
                    Some (Choice2Of2(sprintf "status attribute required for <Expect /> ( file '%s', line '%s' )" src line))

                //if ($TEST_SEEK_SKIP == $seekHash{$level = lc($1)}) {
                | Some "skip" ->
                    TODO "<Expect status=skip it's unused in fsharpqa tests"
                    //$Skip_platforms = uc($2) if $2  
                    Some (Choice2Of2(sprintf "unsupported <Expect status=skip /> found ( file '%s', line '%s' )" src line))

                //} else {
                | Some statusAttrValue when ["success"; "warning"; "error" ] |> List.contains statusAttrValue ->

                    let level = lc statusAttrValue
                    let status = seekHash |> Map.tryFind statusAttrValue

                    //my $text = $2;      # save text for later
                    let mutable text = xml.Value

                    //my $id =   $1 if (m@//\s*<Expect\w*\s*.*?\s*id\s*=\s*"?(.*?)"?[\s>]@i);
                    let idAttr = xml |> tryAttr "id"
                    let mutable idValue = idAttr |> function Some s -> s | None -> ""

                    //my $span = $1 if (m@//\s*<Expect\w*\s*.*?\s*span\s*=\s*"?(.*?)"?[\s>]@i);
                    let spanAttr = xml |> tryAttr "span"

                    //$_ = $span; s/\(/\\\(/; s/\)/\\\)/; $span = $_;
                    let span = 
                        spanAttr 
                        |> Option.map (fun s -> s.Replace("(", "\\(").Replace(")", "\\)"))
                        |> function Some s -> s | None -> ""

                    // # Ignore the actual text and only look at ID and SPAN X_SKIPFULLDIAGCHECK is set to 1
                    
                    // # This is typically for LOC or PLOC runs. Unless we are matching for "success", we
                    // # disregard the text, because it is most likely a localized string.
                    //my $res;
                    let mutable res = ""

                    let ENV_X_SKIPFULLDIAGCHECK = 0 //X_SKIPFULLDIAGCHECK it's unused in fsharpqa tests, so it's false

                    //if( ($ENV{X_SKIPFULLDIAGCHECK} == 1) && ($level ne "success")) {
                    if ( (ENV_X_SKIPFULLDIAGCHECK = 1) && (level <> "success")) then
                        //$res = "";
                        res <- ""
                        //$text = "";
                        text <- ""

                    //} else {
                    else
                        //$res = $text;
                        res <- text

                        //$id = $level . " " . $id if(($level eq "warning") || ($level eq "error"));
                        if (level = "warning") || (level = "error") then
                            idValue <- level + " " + idValue
                    //}
                    
                    //$res = $id . ":.+" . $res if($id ne "");
                    if (idValue <> "") then res <- idValue + ":.+" + res

                    //$res = $span . ":.+" . $res if($span ne "");
                    if (span <> "") then res <- span + ":.+" + res
                    
                    //if (($text ne "") || ($id ne "") || ($span ne "")){
                    //    push @match, $res;
                    //}
                    if ( (text <> "") || (idValue <> "") || (span <> "")) then
                        Some (Choice1Of2(ExpectMatch(statusAttrValue,res)))

                    // # test short xml form
                    //elsif (m@//\s*<Expect\w*\s*Status\s*=\s*(success|warning|error|skip)\s*/\s*>@i) { 
                    //  $level = lc($1);
                    //}
                    else
                        TODO "same as full xml form, not needed, it's the empty <Expect/>"
                        Some (Choice1Of2(ExpectMatch(statusAttrValue,res)))

                // # test full xml form
                //elsif (m@//\s*<Expect\w*\s*Status\s*=\s*(notin)\s*>\s*(.*?)\s*<(/Expect|/)\w*>@i) {
                | Some "notin" ->
                    //push @dontmatch, $2 if $2;
                    let d2 = xml.Value
                    Some (Choice1Of2(ExpectNotMatch d2))
                //} else {
                | Some s ->
                    //next;
                    Some (Choice2Of2(sprintf "invalid status attribute '%s' for <Expect />" s))
                //})
            
            //}
            | unsupportedXml -> 
                log "not supported xml '%s' in comment" unsupportedXml
                Some (Choice2Of2(sprintf "not supported xml '%s' in comment" unsupportedXml))
        )
    |> List.ofSeq
    |> (fun l ->
            let failed = l |> List.choose (function Choice2Of2 x -> Some x | _ -> None)
            match failed with
            | [] ->

                let expects = l |> List.choose (function Choice1Of2 x -> Some x | _ -> None)

                // # Actual work!
                //$level = $seekHash{$level};
                //if ($level == $TEST_SEEK_SKIP) {
                //  $_skip = 1;
                //} else {
                //  $expect = $level if ($level > $expect); # max
                //}
                
                let levelMax =
                    expects
                    |> List.choose (function ExpectMatch (level,_) -> Some level | _ -> None)
                    |> List.choose (fun level -> seekHash |> Map.tryFind level)
                    |> List.fold max 0

                let expectExeOutput = 
                    match Output with
                    | [] -> []
                    | l -> [ExeOutputMatch l]

                succeed (levelMax, expects @ expectExeOutput)
            | f :: fs ->
                fs |> List.iter (log "test spec error: %s")
                NUnitConf.genericError f 
        )
    //}
    
    //return($expect, $_skip, \@expected, \@dontmatch);
    //TODO "unused"


// #############################################################
// #
// # GetExpectedTargetInfo
// #
// # Parse the /out /t(arget) options from $SCFLAGS:
// # 1. If /t(arget) is specified then $targetType is set based on the value of the last /t(arget) in 
// # $SCFLAGS. If /t(arget) is not specified then we $targetType is automatically set to TARGET_EXE.
// #
// # 2. If /out is specified then $targetName is set to the value of the last /out in $SCFLAGS else.
// # If /out is not specified, $targetName is determined based on $Sources; this is done by appending 
// # the appropriate extension to the extension stripped source name and testing if the file exists until   
// # we find a match or we expire all possibilities.  
// #
//sub GetExpectedTargetInfo()
let GetExpectedTargetInfo cwd (_sources: string) _SCFLAGS = attempt {
    let fileExists = Commands.fileExists cwd

    //my 	( $_sources, $_SCFLAGS ) = @_;
    ignore "arguments"
    
    //use File::Basename;
    ignore "useless require"

    //my %target_extension_hash = (
    //           exe     => ['.exe', TARGET_EXE],
    //           winexe  => ['.exe', TARGET_EXE],
    //           library => ['.dll', TARGET_DLL],
    //           module  => ['.netmodule', TARGET_MOD]
    //          );
    let target_extension_hash =
        [ "exe",     ( ".exe", TARGET_EXE )
          "winexe",  ( ".exe", TARGET_EXE )
          "library", ( ".dll", TARGET_DLL )
          "module",  ( ".netmodule", TARGET_MOD ) ]
        |> Map.ofList

    //my $target_name;
    let mutable target_name = ""
    //my $target_type = 'exe';
    let mutable target_type = "exe"
    //my $target_extension = $target_extension_hash{$target_type}[0];
    let mutable target_extension = target_extension_hash |> Map.find target_type |> fst;
    
    do match _SCFLAGS with
       //if ($_SCFLAGS =~ /.*(--target:|-a)((\w*)|$)/i) {
       | Regex @".*(--target:|-a)((\w*)|$)" [ d1; d2 ] ->
           // #figure out targetname from SCFLAGS
           //if("$1" eq "-a") {
           if (d1 = "-a") then
               //$target_extension = $target_extension_hash{"library"}[0];
               target_extension <- target_extension_hash |> Map.find "library" |> fst
               //target_type = "library";
               target_type <- "library"
           //} else {
           else
               //$target_extension = $target_extension_hash{lc($2)}[0] if (defined($2));
               if (not(System.String.IsNullOrWhiteSpace(d2))) then
                   target_extension <- target_extension_hash |> Map.find (lc d2) |> fst
               //$target_type = $2 if (defined($2));
               if (not(System.String.IsNullOrWhiteSpace(d2))) then
                   target_type <- d2
           //}
       //}
       | _ -> ()

    do match _SCFLAGS with
       //if ($_SCFLAGS =~ /.*(-out:|-o )(\".*?\"|\S*)/i) {
       | Regex """.*(-out:|-o )(\".*?\"|\S*)""" [ _d1; d2 ] ->
           //#grab what is after out:
           //$target_name = $2;
           target_name <- d2
       //}
       | _ ->
           ()

    //if (defined($target_name)) {
    do if (not(System.String.IsNullOrWhiteSpace(target_name))) then
            //$target_name =~ s/(^\"|$\")//g;     #remove enclosing "s before testing file if exists. '"' is not a valid file name character
            target_name <- target_name.Trim().TrimStart('"').TrimEnd('"') // remove enclosing "s before testing file if exists. '"' is not a valid file name character

            //return undef unless( -e $target_name );
            ignore "useless, it's already checked below"
        //} else { # Figure it out from sources
        else // Figure it out from sources
            //foreach my $source (split(/[\s+]/,$_sources)){
            //    $source = basename( $source );
            //    $source =~ s/(\w+)\.\w*$/$1$target_extension/;
            //    $target_name = $source if (-e $source);
            //    last if ($target_name);
            //}
            _sources.Split([| ' ' |], StringSplitOptions.RemoveEmptyEntries)
            |> Array.tryPick fileExists
            |> Option.map (fun p -> IO.Path.ChangeExtension(p, target_extension))
            |> Option.iter (fun p -> target_name <- p)

        //}
    
    return!
        if String.IsNullOrWhiteSpace(target_name) then
            //return undef unless ( -e $target_name);
            NUnitConf.genericError "target_name required"
        else
            //return ($target_name, $target_extension_hash{$target_type}[1]);
            succeed (target_name, target_extension_hash |> Map.find target_type |> snd )
    }

// #############################################################
// #
// # LogTime -- Log the time it took for a test to execute...
// #
//sub LogTime{
let LogTime logFile src (compileTime: TimeSpan) (runTime: TimeSpan) =
    //my($Src, $CompileTime, $RunTime) = @_;
    ignore "arguments"

    //my($dir) = $main::root;
    //open(TIMELOGFILE, ">>$dir\\timing.log");
    //print TIMELOGFILE "$Src\t$CompileTime\t$RunTime\n";
    //close TIMELOGFILE;
    System.IO.File.AppendAllLines(logFile, [| sprintf "%s\t%A\t%A" src compileTime runTime |])

// #############################################################
// #
// # RunExit -- Exits the script with the specified value.  
// # 
//sub RunExit {
let RunExit envPOSTCMD (exitVal: int) (cmtStr: string) = attempt {
    //my (
    //    $exitVal,		# Our exit value
    //    $cmtStr,		# Comment string to print before exit
    //   ) = @_;
    ignore "are arguments"    

    //my %status_hash = (
    //       0 => "PASS",
    //       1 => "FAIL",
    //       2 => "SKIP"
    //      );
    let status_hash =
        [ 0, "PASS"
          1, "FAIL"
          2, "SKIP" ]
        |> Map.ofList
              
    //print("$cmtStr") if ($cmtStr);
    do if (not(System.String.IsNullOrWhiteSpace(cmtStr))) then
           printf "%s" cmtStr
    
    //my $exit_str;
    let exit_str = ""
    //my $test_result = $exitVal;
    let test_result = exitVal
    
    // # Run POSTCMD if any
    //if (defined($ENV{POSTCMD})) {
    do! if (not(System.String.IsNullOrWhiteSpace(envPOSTCMD))) then
            // # Do the magic to replace known tokens in the
            // # PRECMD/POSTCMD: for now you can write in env.lst
            // # something like:
            // #    SOURCE=foo.fs POSTCMD="\$FSC_PIPE bar.fs"
            // # and it will expanded into $FSC_PIPE before invoking it
            //$_ = $ENV{POSTCMD};
            //s/^\$FSC_PIPE/$FSC_PIPE/;
            //s/^\$FSI_PIPE/$FSI_PIPE/;
            //s/^\$FSI32_PIPE/$FSI32_PIPE/;
            //s/^\$CSC_PIPE/$CSC_PIPE/;
            //s/^\$VBC_PIPE/$VBC_PIPE/;
            TODO "implement replace"
            
            //if (RunCommand("POSTCMD",$_,1)){
            //     $exitVal = TEST_FAIL;
            //     $test_result = TEST_FAIL;
            //     $exit_str .= "Fail to execute the POSTCMD. ";
            //}
            TODO "implement POSTCMD"

            NUnitConf.skip "POSTCMD not implemented"
        else
            Success
    //}
    
    //if (exists($ENV{SKIPTEST})) {
    //    $exit_str = "Test Marked: SKIP using Environment Variable SKIPTEST , Tested as: ";
    //    $exitVal = TEST_SKIPPED;
    //} elsif ($Skip) {
    //    my @platforms;
    //    $Skip_platforms=~s/\s//g;
    //    # skip all platforms if no platforms specified
    //    if ($Skip_platforms eq "") {
    //        $exit_str = "Test Marked: SKIP, Tested as: ";
    //        $exitVal = TEST_SKIPPED;
    //    }
    //    # treat garbage comas as fatal error
    //    elsif (!scalar(@platforms = split(/,/,$Skip_platforms))) {
    //        $exit_str = "Expects Skip Tag Has Errors: \"$Skip_platforms\" Test Was $status_hash{$test_result}, Tested as: ";
    //        $exitVal = TEST_FAIL;
    //        $test_result = TEST_FAIL;
    //    } else {  
    //        my $platform_to_skip;
    //        foreach my $match (@platforms) {
    //            # treat unrecognized platform or garbage as fatal error
    //            unless ($platform_to_skip = $Platform_Hash{$match}) {
    //                $exit_str = "Expects Skip Tag Has Errors: \"$Skip_platforms\" Test Was $status_hash{$test_result}, Tested as: ";
    //                $exitVal = TEST_FAIL;
    //                $test_result = TEST_FAIL;
    //                last;
    //            }
    //            # don't break here even if we match because we might run into garbage later on
    //            elsif ($platform_to_skip == $platform) {
    //                $exit_str = "Test Marked: SKIP, Tested as: ";
    //                $exitVal = TEST_SKIPPED;
    //            }
    //        }
    //    }
    //}
    TODO "implement SKIP? or it's the runner filter?"
    
    //print $exit_str . $status_hash{$test_result} . "\n";
    printfn "%s %s" exit_str (status_hash |> Map.find test_result)
    
    //exit($exitVal);
    return exitVal
    }

// #############################################################
// #
// # GetCurrentPlatform
// #
//sub GetCurrentPlatform(){
let GetCurrentPlatform () =
    // # Get current platform and fail if we don't support it
      
    //my %proc_hash = (
    //     _ => 'WIN9X',
    //     X86_ => 'X86',
    //     AMD64_ => 'AMD64',
    //     IA64_ => 'IA64',
    //     X86_AMD64 => 'WOW_AMD64',
    //     X86_IA64 => 'WOW_IA64',
    //    );
    //
    //my $platform_string = uc($ENV{PROCESSOR_ARCHITECTURE})."_".uc($ENV{PROCESSOR_ARCHITEW6432});
    //my $res = $Platform_Hash{$proc_hash{$platform_string}};
    //
    //unless (defined($res)) {
    //  my $error_string = "PROCESSOR_ARCHITECTURE:" . $ENV{PROCESSOR_ARCHITECTURE} . " with PROCESSOR_ARCHITEW6432:" . $ENV{PROCESSOR_ARCHITEW6432};
    //  RunExit(TEST_FAIL, "GetCurrentPlatform::Fatal Error: Run.pl does not support the current $error_string \n");
    //}
    //return $res;
    ignore "useless, it's calculated from another function"      


let runpl cwd initialEnvVars = attempt {

    let mutable envVars = initialEnvVars

    let env key = envVars |> Map.tryFind key
    let envOrDefault key def = env key |> Option.fold (fun s t -> t) def
    let envOrFail key = env key |> function Some x -> x | None -> failwithf "environment variable '%s' required " key
    let envSet key value = envVars <- envVars |> Map.add key value

    
    let unlink = Commands.rm cwd
    let fileExists = Commands.fileExists cwd
    let getfullpath = Commands.getfullpath cwd
    
    //shadow some function, to have same argument as perl script
    let RunCommand = RunCommand cwd envVars
    let RunCompilerCommand = RunCompilerCommand cwd envVars
    let GetExpectedTargetInfo = GetExpectedTargetInfo cwd
    let GetExpectedResults = GetExpectedResults cwd

    let LogTime = 
        //my($dir) = $main::root;
        //open(TIMELOGFILE, ">>$dir\\timing.log");
        let logFile = __SOURCE_DIRECTORY__ ++ "timing.log"
        LogTime logFile

    // # run.pl
    
    //my %Platform_Hash = (
    //         WIN9X => PLATFORM_WIN9X,
    //         X86 => PLATFORM_X86,
    //         AMD64 => PLATFORM_AMD64, 
    //         IA64 => PLATFORM_IA64,
    //         WOW_AMD64 => PLATFORM_WOW_AMD64,
    //         WOW_IA64 => PLATFORM_WOW_IA64,     
    //        );    
    //
    //my $platform = &GetCurrentPlatform();
    let platform = PLATFORM_X86
    
    //unlink ASSERT_FILE if ( -e ASSERT_FILE );
    fileExists ASSERT_FILE |> Option.iter unlink
    //$ENV{VSASSERT//} = ASSERT_FILE;
    envSet "VSASSERT" ASSERT_FILE

    // #global variable for command output
    //my @CommandOutput=();
    TODO "CommandOutput it's not anymore a global variable, it's returned from Command"
    
    // # Is this a compile-only run?
    //my $compileOnlyRun = 0;
    //$compileOnlyRun = 1 if (exists($ENV{COMPILE_ONLY}));
    let compileOnlyRun =
        match env "COMPILE_ONLY" with
        | None -> false
        | Some _ -> true
    
    // # Process EXCLUDEIF items
    do! match env "EXCLUDEIF" with
        | None -> Success
        //if (defined($ENV{EXCLUDEIF})){
        | Some _ ->
            //  foreach my $EXCLUDE_ITEM ( split(/;/,$ENV{EXCLUDEIF}) ) {
            //    if ($ENV{TARGET} eq $EXCLUDE_ITEM) {
            //      RunExit(TEST_SKIPPED, "Test excluded for target $ENV{TARGET}\n")
            //    }
            //  }
            TODO "EXCLUDEIF not supported, not used in fsharpqa tests"
            NUnitConf.skip "EXCLUDEIF not supported"
        //}
    
    //# See if we are doing strong name verification
    //my $VerifyStrongName = 0;
    //$VerifyStrongName = 1 if ($ENV{VERIFYSTRONGNAME} =~ /TRUE/i);
    let VerifyStrongName =
        match env "VERIFYSTRONGNAME" |> Option.map (fun s -> s.ToUpper()) with
        | Some "TRUE" -> true
        | None | Some _ -> false
    
    // # Check for any compiler flags
    //my $SCFLAGS = $ENV{SCFLAGS};
    let SCFLAGS = env "SCFLAGS"
    
    // # Check for any compiler 'tail' flags
    //my $TAILFLAGS = $ENV{TAILFLAGS};
    let TAILFLAGS = env "TAILFLAGS"
    
    // # Check for any global compiler flags
    //my $ISCFLAGS = $ENV{ISCFLAGS};
    //unless( defined($ISCFLAGS) ){
    //  $ISCFLAGS = " ";
    //}
    let ISCFLAGS = envOrDefault "ISCFLAGS" " "
    
    // # Filter out flags that don't make sense in FSI (e.g. --standalone)
    // # We will add more in the future, if needed.
    //my $IFSIFLAGS = $ENV{IFSIFLAGS};
    //unless( defined($IFSIFLAGS) ){
    //  $IFSIFLAGS = $ISCFLAGS;
    //}
    let mutable IFSIFLAGS =
        match env "IFSIFLAGS" with
        | Some s -> s 
        | None -> ISCFLAGS

    //$_ = $IFSIFLAGS;
    //s/[ ]+--standalone[ ]+/ /; s/[ ]+--standalone$//; s/^--standalone[ ]+//;
    //$IFSIFLAGS = $_;
    IFSIFLAGS <- IFSIFLAGS.Replace("--standalone", " ")
    
    // #Take care of timing
    //my $TimeTests = 0;
    //$TimeTests = 1 if (exists($ENV{TimeTests}));
    let TimeTests = env "TimeTests" |> Option.isSome
    
    //# Running on Vista (or later)?
    //my $isVistaOrLater = 0;
    //$_ = `ver`;
    //$isVistaOrLater = 1 if(/([0-9]+)\.[0-9]+\.[0-9]/ && ($1>=6));
    let isVistaOrLater = 1 //it's always vista or later

    // # Is this a Vista-only test?
    //my $VISTA_ONLY = $ENV{VISTA_ONLY};
    //if($VISTA_ONLY && !$isVistaOrLater)
    do! match env "VISTA_ONLY" with
        | None -> Success
        | Some _ ->
            //{
            //   RunExit(TEST_SKIPPED, "Test skipped: This test only run on Vista (or later)\n");
            //}
            ignore "unused, it's always vista or later, better remove it?"
            NUnitConf.skip "VISTA_ONLY not supported"
    
    // # Are we using a 'special' compiler? By default, we simply invoke "fsc" expecting it to be in the PATH
    // # This new env variable would allow enable the following scenarios:
    // # - specify a private compiler
    // # - apply a stopit kind of logic (to prevent runaway tests to hose a run)
    // # - possibly app compat / bin compat scenarios
    // # By default, we revert to the old behavior (i.e. COMPILER_NAME)
    //my $FSC_PIPE=$ENV{FSC_PIPE};
    //unless( defined($FSC_PIPE) ){
    //  $FSC_PIPE = COMPILER_NAME;
    //  $ENV{FSC_PIPE}=COMPILER_NAME;
    //}
    let FSC_PIPE = envOrDefault "FSC_PIPE" COMPILER_NAME
    envSet "FSC_PIPE" FSC_PIPE
    
    //my $FSI_PIPE=$ENV{FSI_PIPE};
    //unless( defined($FSI_PIPE) ){
    //  $FSI_PIPE = FSI_NAME;
    //  $ENV{FSI_PIPE}=FSI_NAME;
    //}
    let FSI_PIPE = envOrDefault "FSI_PIPE" FSI_NAME
    envSet "FSI_PIPE" FSI_PIPE
    
    //my $FSI32_PIPE=$ENV{FSI32_PIPE};
    //unless( defined($FSI32_PIPE) ){
    //  $FSI32_PIPE = FSI32_NAME;
    //  $ENV{FSI32_PIPE}=FSI32_NAME;
    //}
    let FSI32_PIPE = envOrDefault "FSI32_PIPE" FSI32_NAME
    envSet "FSI32_PIPE" FSI32_PIPE
    
    //my $CSC_PIPE=$ENV{CSC_PIPE};
    //unless( defined($CSC_PIPE) ){
    //  $CSC_PIPE = CSC_NAME;
    //  $ENV{CSC_PIPE}=CSC_NAME;
    //}
    let CSC_PIPE = envOrDefault "CSC_PIPE" CSC_NAME
    envSet "CSC_PIPE" CSC_PIPE
    
    //my $VBC_PIPE=$ENV{VBC_PIPE};
    //unless( defined($VBC_PIPE) ){
    //  $VBC_PIPE = VBC_NAME;
    //  $ENV{VBC_PIPE}=VBC_NAME;
    //}
    let VBC_PIPE = envOrDefault "VBC_PIPE" VBC_NAME
    envSet "VBC_PIPE" VBC_PIPE

    let skipIfContainsRedirection varName (exe, cmdArgs: string) = attempt {
        if cmdArgs.Contains(">") then
            return! NUnitConf.skip (sprintf "output/error redirection is not implemented. Var %s => '%s %s'" varName exe cmdArgs)
        }
        
    
    //#
    //# Run pre-command if any
    //#
    //if (exists($ENV{PRECMD})) {
    do! match env "PRECMD" with
        | None -> Success
        | Some envPRECMD -> attempt {
            let replace (a: string) b (s: string) = s.Replace(a, b)
            // # Do the magic to replace known tokens in the
            // # PRECMD/POSTCMD: for now you can write in env.lst
            // # something like:
            // #    SOURCE=foo.fs PRECMD="\$FSC_PIPE bar.fs"
            // # and it will expanded into $FSC_PIPE before invoking it
            //$_ = $ENV{PRECMD};
            let pre =
                envPRECMD
                //s/^\$FSC_PIPE/$FSC_PIPE/;
                |> replace "$FSC_PIPE" FSC_PIPE 
                //s/^\$FSI_PIPE/$FSI_PIPE/;
                |> replace "$FSI_PIPE" FSI_PIPE
                //s/^\$FSI32_PIPE/$FSI32_PIPE/;
                |> replace "$FSI32_PIPE" FSI32_PIPE
                //s/\$ISCFLAGS/$ISCFLAGS/;
                |> replace "$ISCFLAGS" ISCFLAGS
                //s/^\$CSC_PIPE/$CSC_PIPE/;
                |> replace "$CSC_PIPE" CSC_PIPE
                //s/^\$VBC_PIPE/$VBC_PIPE/;
                |> replace "$VBC_PIPE" VBC_PIPE

            let exe, cmdArgs = pre |> splitAtFirst Char.IsWhiteSpace
            let cmdArgsString = cmdArgs |> function Some s -> s | None -> ""

            do! skipIfContainsRedirection "PRECMD" (exe, cmdArgsString)

            let! e,o = RunCommand "PRECMD" (exe, cmdArgsString) true
            //RunExit(TEST_FAIL, "Fail to execute the PRECMD" . @CommandOutput . "\n")  if RunCommand("PRECMD",$_ ,1); 
            if e <> 0
            then return! NUnitConf.genericError (sprintf "Fail to execute the PRECMD %s" o)
            }
        //}
    
    //# Normal testing begins 
    //my $Sources = &GetSrc();
    let! sources = GetSrc cwd (envOrDefault "SOURCE" "")
    
    //my ( $Skip_platforms, @match, $CmdLine, @NotExpectedOutput);
    TODO "support $Skip_platforms, @match, $CmdLine, @NotExpectedOutput"

    //my ( $Type, $Skip, $Output, $Dontmatch ) = &GetExpectedResults($Sources);
    ignore "let's simplify a bit"

    let! mType, dd = GetExpectedResults sources
    let Type = mType
    
    //#################################################################################
    //# Compiling..........
    //#
    
    //# if env variable is set, use external hosted compiler for FSC tests
    //my $useHostedCompiler = ($ENV{HOSTED_COMPILER} eq "1");
    let mutable useHostedCompiler = env "HOSTED_COMPILER" |> function Some "1" -> true | _ -> false

    //my $compiler_command = "Unknown";
    let! compiler_command = attempt {
        let concatEmpty = Array.ofList >> Array.choose id >> String.concat " "
        //if ($ENV{FSIMODE} eq "") {
        match env "FSIMODE" with
        | None | Some "" ->
            //$compiler_command = "$FSC_PIPE $ISCFLAGS $SCFLAGS $Sources $TAILFLAGS";
            return FSC_PIPE, [ Some ISCFLAGS; SCFLAGS; Some sources; TAILFLAGS ] |> concatEmpty
        //} else {
        | Some _ ->
            // # don't use hosted compiler for FSI tests
            //$useHostedCompiler = 0;
            useHostedCompiler <- false
            match env "FSIMODE" with
            //if($ENV{FSIMODE} eq "PIPE") {
            | Some "PIPE" ->
                //$compiler_command = "$FSI_PIPE<$Sources $IFSIFLAGS $SCFLAGS $TAILFLAGS";
                return! NUnitConf.skip (sprintf "FSIMODE=PIPE it's not implemented")
            //} elsif($ENV{FSIMODE} eq "EXEC") {
            | Some "EXEC" ->
                //$compiler_command = "$FSI_PIPE --exec $IFSIFLAGS $SCFLAGS $Sources $TAILFLAGS";
                return FSI_PIPE, [ Some "--exec"; Some IFSIFLAGS; SCFLAGS; Some sources; TAILFLAGS ] |> concatEmpty 
            //} elsif($ENV{FSIMODE} eq "FEED") {
            | Some "FEED" ->
                //$compiler_command = "$FSI_PIPE $IFSIFLAGS $SCFLAGS $Sources $TAILFLAGS";
                return FSI_PIPE, [ Some IFSIFLAGS; SCFLAGS; Some sources; TAILFLAGS ] |> concatEmpty
            //} else { # default to FEED
            | _ ->
                //$compiler_command = "$FSI_PIPE $IFSIFLAGS $SCFLAGS $Sources $TAILFLAGS";
                return FSI_PIPE, [ Some IFSIFLAGS; SCFLAGS; Some sources; TAILFLAGS ] |> concatEmpty
            //}
        //}
        }
    
    //my($CompilerStartTime) = time();
    let CompilerStartTime = System.Diagnostics.Stopwatch.StartNew()
    //my $ExitCode = RunCompilerCommand($useHostedCompiler, "Compiling", "$compiler_command");
    let! exitCode,commandOutput = RunCompilerCommand useHostedCompiler "Compiling" compiler_command false
    //my($CompileTime) = $CompilerStartTime - time();
    let CompileTime = CompilerStartTime.Elapsed
    
    printfn "Checking Expected results (%i): " (dd |> List.length)
    dd |> List.iter (printfn "- %A")

    do! if dd |> List.exists (function ExpectedResults.CmdLine _ -> true | _ -> false) then
            NUnitConf.skip (sprintf "<CmdLine /> not implemented")
        else    
            Success

    //foreach (@CommandOutput) {
    //  my $n_remaining_to_match = scalar(@match);
    //  my $matched = 0;
    //  for (my $i = 0; $i < $n_remaining_to_match; $i++) {
    //    if (m/$match[$i]/) {
    //      splice(@match, $i, 1);
    //      print("[matched] ");
    //      $matched = 1;
    //      last;
    //    }
    //  }
    //  unless($matched){
    //    foreach my $notin (@{$Dontmatch}){
    //      # print ",$notin,\n";
    //      push(@NotExpectedOutput,$_) if (/$notin/);
    //    }
    //  }
    //  print;
    //}
    //
    //# Expected match lines were never matched
    //if (scalar(@match) || scalar(@NotExpectedOutput)){		# something went wrong
    //  print("\n*** The following necessary lines were never matched:\n");
    //  foreach my $line (@match) {
    //    print("***\t$line\n");
    //  }
    //
    //  print("\n\n*** The following necessary lines were incorrectly matched:\n");
    //  foreach my $line (@NotExpectedOutput){
    //    print("***\t$line\n");
    //  }
    //  print("\n");
    //  RunExit(TEST_FAIL, "Unexpected Compiler Output \n");
    //}

    let checkMatchOutput regexPattern = 
        let outputNormalized = commandOutput.Replace("\r\n","\n") //regex $ multiline doesnt match \r\n, only \n
        (Regex(regexPattern, RegexOptions.Multiline)).Match(outputNormalized).Success

    let matchLines = 
        dd 
        |> List.choose (function ExpectedResults.ExpectMatch(_, s) -> Some s | _ -> None)
        |> List.filter (not << String.IsNullOrWhiteSpace)

    do! match matchLines |> List.filter (not << checkMatchOutput) with
        | [] -> 
            printfn "Expect match: [passed]"
            Success
        | notMatched ->
            printfn "Expect match: [failed]"
            notMatched |> List.iter (printfn "Expected match '%s' not found")
            printfn "Output:"
            printfn "%s" commandOutput
            NUnitConf.genericError (sprintf "expect match failed: %A" notMatched)


    let notMatchLines = 
        dd 
        |> List.choose (function ExpectedResults.ExpectNotMatch s -> Some s | _ -> None)
        |> List.filter (not << String.IsNullOrWhiteSpace)

    do! match notMatchLines |> List.filter (checkMatchOutput) with
        | [] -> 
            printfn "Expect not match: [passed]"
            Success
        | matched ->
            printfn "Expect not match: [failed]"
            matched |> List.iter (printfn "Expected not match '%s' but found")
            printfn "Output:"
            printfn "%s" commandOutput
            NUnitConf.genericError (sprintf "expect not match failed: %A" matched)
    
    //my ($targetName, $targetType) = &GetExpectedTargetInfo($Sources, $SCFLAGS);
    let! targetName, targetType = GetExpectedTargetInfo sources (SCFLAGS |> function Some s -> s | None -> "")
    
    //if ($ExitCode && ($Type < TEST_SEEK_ERROR)) {
    do! if ((exitCode <> 0) && (Type < TEST_SEEK_ERROR)) then
            //RunExit(TEST_FAIL, "Compile Unexpectedly Failed: $ExitCode \n");
            NUnitConf.genericError (sprintf "Compile Unexpectedly Failed: %i" exitCode)
        else
            Success
    //}
    
    //if (($ExitCode == 0) && ($Type == TEST_SEEK_ERROR)) {
    do! if ((exitCode = 0) && (Type = TEST_SEEK_ERROR)) then
            // # If this happens, your failure messages in the source
            // # aren't rich enough since the first test checking to
            // # see if scalar(@match) was non-zero should have triggered.
            //RunExit(TEST_FAIL, "Compile Succeeded, Designed To Fail. \n");
            NUnitConf.genericError (sprintf "Compile Succeeded, Designed To Fail.")
        else
            Success
    //}

    
    //if ($ExitCode) {
    do! if (exitCode <> 0) then
            //RunExit(TEST_SKIPPED, "Internal Logic Error(1)") if ($Type != TEST_SEEK_ERROR);
            if (Type <> TEST_SEEK_ERROR) then
                //RunExit(TEST_SKIPPED, "Internal Logic Error(1)") 
                NUnitConf.skip "Internal Logic Error(1)"
            else
                //RunExit(TEST_PASS);		# Designed to fail, and it did
                ignore "make it pass, it's going to be a big if"
                NUnitConf.genericError "Not implemented: Designed to fail, and it did"
        else
            Success
    //}
    
    //RunExit(TEST_SKIPPED, "Internal Logic Error(2)") if ($Type == TEST_SEEK_ERROR);
    do! if (Type = TEST_SEEK_ERROR) then
            NUnitConf.skip "Internal Logic Error(2)"
        else
            Success

    //RunExit(TEST_SKIPPED, "Internal Logic Error(3)") if ($ExitCode);
    do! if (exitCode <> 0) then
            NUnitConf.skip "Internal Logic Error(3)"
        else
            Success
    
    //if($ENV{REDUCED_RUNTIME} ne "1"){
    do! match env "REDUCED_RUNTIME" with
        | Some "1" ->
            Success
        | _ ->
            //if((defined $targetName) && (defined $targetType)) {
            // # check/set PEVerify
            // my $PEVERIFY = $ENV{PEVERIFY}; 
            // unless(defined($PEVERIFY)) {
            //   # Only use peverify if it is in the path
            //   foreach $_ (split /;/, $ENV{PATH}) {
            //     $PEVERIFY = "peverify.exe" if(-e "$_\\peverify.exe");
            //   }
            //   $ENV{PEVERIFY} = $PEVERIFY;
            // }
            //
            // # Use $ENV{PEVER} if it is defined
            // my $PEVER_ARG = $ENV{PEVER};
            //
            //if (!defined($PEVERIFY)) {
            //  print "PEVerify ($PEVERIFY) not defined/found, skipping...\n";
            //} elsif ($PEVER_ARG =~ /\/Exp_Fail/i) {
            //   # do not run if Exp_Fail
            //   print "PEVerify not run because test is marked as an expected failure...\n";
            // } elsif($targetType <= TARGET_DLL) {
            //   RunExit(TEST_FAIL, "PeVerify Failed the test\n") if (RunCommand("Peverify","$PEVERIFY $targetName $ENV{PEVER}",1));
            // }
            //}
            TODO "REDUCED_RUNTIME <> 1 not implemented"
            NUnitConf.skip "REDUCED_RUNTIME not implemented"
    //}
    
    // ################################################################################
    // #
    // # Running the EXE
    // #
    // # Now we scan the output of the EXE if we must
    let checkRunningExe expectedExeOutput () = attempt {
        
        //my $status = TEST_PASS;
        let status = TEST_PASS
        //my $param = "";
        let mutable param = ""
        //RunExit(TEST_FAIL, "Failed to Find Any Target: $targetName \n") unless ( -e $targetName );
        do! match fileExists targetName with
            | None ->
                NUnitConf.genericError (sprintf "Failed to Find Any Target: %s \n" targetName)
            | Some _ ->
                Success

        //$param = $CmdLine if defined($CmdLine);
        TODO "$CmdLine is declare, but it's not initialized before"
        
        //@CommandOutput = ();
        ignore "unused, the CommandOutput now is a return value"
        
        //my($StartTime) = time();
        let StartTime = System.Diagnostics.Stopwatch.StartNew();
        
        //# For /3Gb runs, we need to mark exe with /LARGEADDRESSAWARE
        let markLargAddress exeName = attempt {
            //RunCommand("Marking exe with /LARGEADDRESSAWARE...","editbin.exe /LARGEADDRESSAWARE $targetName");
            let! e,o = RunCommand "Marking exe with /LARGEADDRESSAWARE..." ("editbin.exe", (sprintf "/LARGEADDRESSAWARE %s" exeName)) false
            if e <> 0 then 
                return! NUnitConf.errorLevel e (sprintf "Failed mark exe with /LARGEADDRESSAWARE: %s" o)
            }

        //if(defined($ENV{LARGEADDRESSAWARE})) {
        do! if env "LARGEADDRESSAWARE" |> Option.isSome then
                markLargAddress targetName
            else 
                Success
        //}

        //my $sim = "";
        ignore "unused variable"
        
        let! exePath = 
            //if (defined($ENV{SIMULATOR_PIPE})) {
            if env "SIMULATOR_PIPE" |> Option.isSome then
                //# replace known tokens
                //$_ = $ENV{SIMULATOR_PIPE};
                //s/^\$FSC_PIPE/$FSC_PIPE/;
                //s/^\$FSI_PIPE/$FSI_PIPE/;
                //s/^\$FSI32_PIPE/$FSI32_PIPE/;
                //s/\$ISCFLAGS/$ISCFLAGS/;
                //s/^\$CSC_PIPE/$CSC_PIPE/;
                //s/^\$VBC_PIPE/$VBC_PIPE/;
                //s/\$PLATFORM/$ENV{PLATFORM}/;
                TODO "replace variables"
            
                //$sim = $_;
                //$ExitCode = RunCommand("Running","$sim $targetName $param");
                TODO "SIMULATOR_PIPE not supported, it's not used in fsharpqa tests"
            
                NUnitConf.skip "var SIMULATOR_PIPE not supported"
            //}
            else 
                let exePath = targetName |> getfullpath
                succeed exePath

        do! skipIfContainsRedirection "SOURCE" (exePath, param)

        //$ExitCode = RunCommand("Running","$sim $targetName $param");
        //NOTE there the $sim is blank
        let! exitCode, commandOutput = RunCommand "Running" (exePath, param) false
          
        
        //my($DeltaTime) = time() - $StartTime;
        let DeltaTime = StartTime.Elapsed
        
        //LogTime($Sources, $CompileTime, $DeltaTime) if ($TimeTests);
        do if TimeTests then
              LogTime sources CompileTime DeltaTime
        
    
        //my $check_output = scalar(@{$Output});

        //my ($LinesMatched) = 0;
        //my ($LinesToMatch) = $check_output;
        //
        //#parse the output
        //foreach (@CommandOutput) {
        //  if ($check_output) {
        //    my $line = shift @{$Output};
        //    chop $line eq "\n" || RunExit(TEST_SKIPPED, "Internal error in perl script, expecting newline in \$line \n");
        //    chop $_ eq "\n" || RunExit(TEST_SKIPPED, "Internal error in perl script, expecting newline in \$_ \n");
        //
        //    if (((length($_) == 0) && (length($line) == 0)) ||
        //         (($_ =~ /$line/) && (length($line) != 0))) {
        //       # The good
        //       print("[matched] $_\n");
        //       $LinesMatched++;  
        //           } else {
        //       # The bad
        //       print("  Error: Expected: [$line]\n");
        //       print("  Error: Received: [$_]\n\n");
        //       $status = TEST_FAIL;
        //    }
        //
        //    $check_output = scalar(@{$Output});
        //  } else {
        //    # redirect outputs from the exe to runpl.log
        //    print;
        //  }
        //}
        //print("\n");
        do! match expectedExeOutput with
            | None ->
                Success
            | Some [] ->
                Success
            | Some (x :: xs) ->
                let possible =
                    commandOutput.Split([| System.Environment.NewLine |], StringSplitOptions.RemoveEmptyEntries)
                    |> Array.skipWhile ((<>) x)
                    |> Array.truncate (x::xs |> List.length)
                    |> List.ofArray

                if (x :: xs) = possible then
                    printfn "Output match: [passed]"
                    Success
                else
                    printfn "Output match: [failed]"
                    printfn "Output:"
                    printfn "%s" commandOutput
                    NUnitConf.genericError "exe output doesnt match"
                    
        
        //RunExit(TEST_FAIL, "Generated Test EXE Failed \n") if ($ExitCode);
        do! if (exitCode <> 0) then
                NUnitConf.genericError "Generated Test EXE Failed"
            else
                Success

        //RunExit(TEST_FAIL, "Test EXE had bad output \n") if ($status != TEST_PASS);
        do! if (status <> TEST_PASS) then
                NUnitConf.genericError "Test EXE had bad output"
            else
                Success

        //RunExit(TEST_FAIL, "Test EXE had bad output \n") if ($LinesMatched != $LinesToMatch);
        TODO "match ouput"
    //}
        }

    let checkVerifyStrongName () = attempt {
        let verifyStrongName () = attempt {
            //RunExit(TEST_FAIL, "Assembly failed verification:\n") if RunCommand("VerifyStroingName","sn -q -vf $targetName",1);
            let! e,_ = RunCommand "VerifyStroingName" ("sn", "-q -vf $targetName") true
            if e <> 0 then
                return! NUnitConf.genericError "Assembly failed verification:\n"
        }

        //if ($VerifyStrongName && $targetType <= TARGET_MOD) {
        if VerifyStrongName && (targetType <= TARGET_MOD) then
            return! verifyStrongName()
        }
    //}

    let expectedExeOutput =
        dd |> List.tryPick (function ExeOutputMatch(l) -> Some l | _ -> None)

    // # If this is a compile only run, call post command and exit
    //if ($compileOnlyRun) {
    return!
        if compileOnlyRun then
            Success
        else attempt {
            //if ($targetType == TARGET_EXE) {
            do! if targetType = TARGET_EXE then
                    checkRunningExe expectedExeOutput ()
                else
                    Success

            return! checkVerifyStrongName ()
            }
    //}
    
    //exit (1); #safe stop
    //safe stop
    
    }
