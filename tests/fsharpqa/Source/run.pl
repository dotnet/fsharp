# run.pl

use strict;
use IO::Socket;
use Cwd;

use constant COMPILER_NAME => 'fsc';
use constant FSI_NAME      => 'fsiAnyCPU';
use constant FSI32_NAME    => 'fsi';
use constant CSC_NAME      => 'csc';
use constant VBC_NAME      => 'vbc';

# Constant values for test result
use constant TEST_PASS     => 0;
use constant TEST_FAIL     => 1;
use constant TEST_SKIPPED  => 2;
use constant TEST_CASCADE  => 3;
use constant TEST_NORESULT => 4;

# Constant values for target type
use constant TARGET_EXE => 0;
use constant TARGET_DLL => 1;
use constant TARGET_MOD => 2;

my $VerifyStrongName = 0;

# Constant values for platform type
use constant PLATFORM_X86 => 1;
use constant PLATFORM_IA64 => 2;
use constant PLATFORM_AMD64 => 3;
use constant PLATFORM_WIN9X => 4;
use constant PLATFORM_WOW_IA64 => 5;
use constant PLATFORM_WOW_AMD64 => 6;



# Constant values used internally to determine if the compile/run should succeed or fail
use constant TEST_SEEK_SUCCESS  => 0;
use constant TEST_SEEK_WARN     => 1;
use constant TEST_SEEK_ERROR    => 2;


my %Platform_Hash = (
         WIN9X => PLATFORM_WIN9X,
         X86 => PLATFORM_X86,
         AMD64 => PLATFORM_AMD64, 
         IA64 => PLATFORM_IA64,
         WOW_AMD64 => PLATFORM_WOW_AMD64,
         WOW_IA64 => PLATFORM_WOW_IA64,     
        );    

my $platform = &GetCurrentPlatform();

use constant ASSERT_FILE => '_assert.$$$'; # where we store the VSASSERT file
unlink ASSERT_FILE if ( -e ASSERT_FILE );
$ENV{VSASSERT} = ASSERT_FILE;

#global variable for command output
my @CommandOutput=();

# Is this a compile-only run?
my $compileOnlyRun = 0;
$compileOnlyRun = 1 if (exists($ENV{COMPILE_ONLY}));

# Process EXCLUDEIF items
if (defined($ENV{EXCLUDEIF})){
  foreach my $EXCLUDE_ITEM ( split(/;/,$ENV{EXCLUDEIF}) ) {
    if ($ENV{TARGET} eq $EXCLUDE_ITEM) {
      RunExit(TEST_SKIPPED, "Test excluded for target $ENV{TARGET}\n")
    }
  }
}

# See if we are doing strong name verification
$VerifyStrongName = 1 if ($ENV{VERIFYSTRONGNAME} =~ /TRUE/i);

# Check for any compiler flags
my $CWD = cwd();
$_ = $ENV{SCFLAGS};
s/\$CWD/$CWD/g;
my $SCFLAGS = $_;

# Check for any compiler 'tail' flags
my $TAILFLAGS = $ENV{TAILFLAGS};

# Check for any global compiler flags
my $ISCFLAGS = $ENV{ISCFLAGS};
unless( defined($ISCFLAGS) ){
  $ISCFLAGS = " ";
}

# Filter out flags that don't make sense in FSI (e.g. --standalone)
# We will add more in the future, if needed.
my $IFSIFLAGS = $ENV{IFSIFLAGS};
unless( defined($IFSIFLAGS) ){
  $IFSIFLAGS = $ISCFLAGS;
}
$_ = $IFSIFLAGS;
s/[ ]+--standalone[ ]+/ /; s/[ ]+--standalone$//; s/^--standalone[ ]+//;
$IFSIFLAGS = $_;

#Take care of timing
my $TimeTests = 0;
$TimeTests = 1 if (exists($ENV{TimeTests}));

# Running on Vista (or later)?
my $isVistaOrLater = 0;
$_ = `ver`;
$isVistaOrLater = 1 if(/([0-9]+)\.[0-9]+\.[0-9]/ && ($1>=6));

# Is this a Vista-only test?
my $VISTA_ONLY = $ENV{VISTA_ONLY};

if($VISTA_ONLY && !$isVistaOrLater)
{
   RunExit(TEST_SKIPPED, "Test skipped: This test only run on Vista (or later)\n");
}

# Are we using a 'special' compiler? By default, we simply invoke "fsc" expecting it to be in the PATH
# This new env variable would allow enable the following scenarios:
# - specify a private compiler
# - apply a stopit kind of logic (to prevent runaway tests to hose a run)
# - possibly app compat / bin compat scenarios
# By default, we revert to the old behavior (i.e. COMPILER_NAME)
my $FSC_PIPE=$ENV{FSC_PIPE};
unless( defined($FSC_PIPE) ){
  $FSC_PIPE = COMPILER_NAME;
  $ENV{FSC_PIPE}=COMPILER_NAME;
}

my $FSI_PIPE=$ENV{FSI_PIPE};
unless( defined($FSI_PIPE) ){
  $FSI_PIPE = FSI_NAME;
  $ENV{FSI_PIPE}=FSI_NAME;
}

my $FSI32_PIPE=$ENV{FSI32_PIPE};
unless( defined($FSI32_PIPE) ){
  $FSI32_PIPE = FSI32_NAME;
  $ENV{FSI32_PIPE}=FSI32_NAME;
}

my $CSC_PIPE=$ENV{CSC_PIPE};
unless( defined($CSC_PIPE) ){
  $CSC_PIPE = CSC_NAME;
  $ENV{CSC_PIPE}=CSC_NAME;
}

my $VBC_PIPE=$ENV{VBC_PIPE};
unless( defined($VBC_PIPE) ){
  $VBC_PIPE = VBC_NAME;
  $ENV{VBC_PIPE}=VBC_NAME;
}

#
# Run pre-command if any
#
if (exists($ENV{PRECMD})) {
  # Do the magic to replace known tokens in the
  # PRECMD/POSTCMD: for now you can write in env.lst
  # something like:
  #    SOURCE=foo.fs PRECMD="\$FSC_PIPE bar.fs"
  # and it will expanded into $FSC_PIPE before invoking it
  $_ = $ENV{PRECMD};
  s/\$FSC_PIPE/$FSC_PIPE/g;
  s/\$FSI_PIPE/$FSI_PIPE/g;
  s/\$FSI32_PIPE/$FSI32_PIPE/g;
  s/\$ISCFLAGS/$ISCFLAGS/g;
  s/\$CSC_PIPE/$CSC_PIPE/g;
  s/\$VBC_PIPE/$VBC_PIPE/g;
  RunExit(TEST_FAIL, "Fail to execute the PRECMD:\n" . join("\n", @CommandOutput) . "\n")  if RunCommand("PRECMD",$_ ,1);
}

# Normal testing begins 
my $Sources = &GetSrc();

my ( $Skip_platforms, @match, $CmdLine, @NotExpectedOutput);
my ( $Type, $Skip, $Output, $Dontmatch ) = &GetExpectedResults($Sources);

#################################################################################
# Compiling..........
#
my $compiler_command = "Unknown";

# if env variable is set, use external hosted compiler for FSC tests
my $useHostedCompiler = ($ENV{HOSTED_COMPILER} eq "1");
if ($ENV{FSIMODE} eq "") {
  $compiler_command = "$FSC_PIPE $ISCFLAGS $SCFLAGS $Sources $TAILFLAGS";
} else {
  # don't use hosted compiler for FSI tests
  $useHostedCompiler = 0;
  if($ENV{FSIMODE} eq "PIPE") {
      $compiler_command = "$FSI_PIPE<$Sources $IFSIFLAGS $SCFLAGS $TAILFLAGS";
  } elsif($ENV{FSIMODE} eq "EXEC") {
      $compiler_command = "$FSI_PIPE --exec $IFSIFLAGS $SCFLAGS $Sources $TAILFLAGS";
  } elsif($ENV{FSIMODE} eq "FEED") {
      $compiler_command = "$FSI_PIPE $IFSIFLAGS $SCFLAGS $Sources $TAILFLAGS";
  } else { # default to FEED
      $compiler_command = "$FSI_PIPE $IFSIFLAGS $SCFLAGS $Sources $TAILFLAGS";
  }
}


my($CompilerStartTime) = time();
my $ExitCode = RunCompilerCommand($useHostedCompiler, "Compiling", "$compiler_command");
my($CompileTime) = $CompilerStartTime - time();

foreach (@CommandOutput) {
  my $n_remaining_to_match = scalar(@match);
  my $matched = 0;
  for (my $i = 0; $i < $n_remaining_to_match; $i++) {
    if (m/$match[$i]/) {
      splice(@match, $i, 1);
      print("[matched] ");
      $matched = 1;
      last;
    }
  }
  unless($matched){
    foreach my $notin (@{$Dontmatch}){
      # print ",$notin,\n";
      push(@NotExpectedOutput,$_) if (/$notin/);
    }
  }
  print;
}


# Expected match lines were never matched
if (scalar(@match) || scalar(@NotExpectedOutput)){		# something went wrong
  print("\n*** The following necessary lines were never matched:\n");
  foreach my $line (@match) {
    print("***\t$line\n");
  }

  print("\n\n*** The following necessary lines were incorrectly matched:\n");
  foreach my $line (@NotExpectedOutput){
    print("***\t$line\n");
  }
  print("\n");
  RunExit(TEST_FAIL, "Unexpected Compiler Output \n");
}

my ($targetName, $targetType) = &GetExpectedTargetInfo($Sources, $SCFLAGS);

if ($ExitCode && ($Type < TEST_SEEK_ERROR)) {
  RunExit(TEST_FAIL, "Compile Unexpectedly Failed: $ExitCode \n");
}

if (($ExitCode == 0) && ($Type == TEST_SEEK_ERROR)) {
  # If this happens, your failure messages in the source
  # aren't rich enough since the first test checking to
  # see if scalar(@match) was non-zero should have triggered.
  RunExit(TEST_FAIL, "Compile Succeeded, Designed To Fail. \n");
}

if ($ExitCode) {
  RunExit(TEST_SKIPPED, "Internal Logic Error(1)") if ($Type != TEST_SEEK_ERROR);
  RunExit(TEST_PASS);		# Designed to fail, and it did
}

RunExit(TEST_SKIPPED, "Internal Logic Error(2)") if ($Type == TEST_SEEK_ERROR);
RunExit(TEST_SKIPPED, "Internal Logic Error(3)") if ($ExitCode);

if($ENV{REDUCED_RUNTIME} ne "1"){
    if((defined $targetName) && (defined $targetType)) {
     # check/set PEVerify
     my $PEVERIFY = $ENV{PEVERIFY}; 
     unless(defined($PEVERIFY)) {
       my $scriptPath = dirname(__FILE__);
       my @configurations = ("Debug", "Release");
       foreach my $config (@configurations) {
         $PEVERIFY = "$scriptPath\\..\\..\\..\\artifacts\\bin\\PEVerify\\$config\\net472\\PEVerify.exe";
         if (-e $PEVERIFY) {
           $ENV{PEVERIFY} = $PEVERIFY;
           last;
         }
       }
     }

     # Use $ENV{PEVER} if it is defined
     my $PEVER_ARG = $ENV{PEVER};

    if (!defined($PEVERIFY)) {
      print "PEVerify ($PEVERIFY) not defined/found, skipping...\n";
    } elsif ($PEVER_ARG =~ /\/Exp_Fail/i) {
       # do not run if Exp_Fail
       print "PEVerify not run because test is marked as an expected failure...\n";
     } elsif($targetType <= TARGET_DLL) {
       RunExit(TEST_FAIL, "PeVerify Failed the test\n") if (RunCommand("Peverify","$PEVERIFY $targetName $ENV{PEVER}",1));
     }
    }
}

# If this is a compile only run, call post command and exit
if ($compileOnlyRun)
{
  RunExit(TEST_PASS);
}

################################################################################
#
# Running the EXE
#
# Now we scan the output of the EXE if we must
if ($targetType == TARGET_EXE) {

  my $check_output = scalar(@{$Output});
  my $status = TEST_PASS;
  my $param = "";
  RunExit(TEST_FAIL, "Failed to Find Any Target: $targetName \n") unless ( -e $targetName );
  $param = $CmdLine if defined($CmdLine);


  @CommandOutput = ();
  my($StartTime) = time();

  # For /3Gb runs, we need to mark exe with /LARGEADDRESSAWARE
  if(defined($ENV{LARGEADDRESSAWARE})) {
      RunCommand("Marking exe with /LARGEADDRESSAWARE...","editbin.exe /LARGEADDRESSAWARE $targetName");
  }

  my $sim = "";
  if (defined($ENV{SIMULATOR_PIPE})) {
    # replace known tokens
    $_ = $ENV{SIMULATOR_PIPE};
    s/^\$FSC_PIPE/$FSC_PIPE/;
    s/^\$FSI_PIPE/$FSI_PIPE/;
    s/^\$FSI32_PIPE/$FSI32_PIPE/;
    s/\$ISCFLAGS/$ISCFLAGS/;
    s/^\$CSC_PIPE/$CSC_PIPE/;
    s/^\$VBC_PIPE/$VBC_PIPE/;
    s/\$PLATFORM/$ENV{PLATFORM}/;
    
    $sim = $_;
  }

  $ExitCode = RunCommand("Running","$sim $targetName $param");
  my($DeltaTime) = time() - $StartTime;

  LogTime($Sources, $CompileTime, $DeltaTime) if ($TimeTests);

  my ($LinesMatched) = 0;
  my ($LinesToMatch) = $check_output;

  #parse the output
  foreach (@CommandOutput) {
    if ($check_output) {
      my $line = shift @{$Output};
      chop $line eq "\n" || RunExit(TEST_SKIPPED, "Internal error in perl script, expecting newline in \$line \n");
      chop $_ eq "\n" || RunExit(TEST_SKIPPED, "Internal error in perl script, expecting newline in \$_ \n");
  
      if (((length($_) == 0) && (length($line) == 0)) ||
    (($_ =~ /$line/) && (length($line) != 0))) {
  # The good
  print("[matched] $_\n");
  $LinesMatched++;  
      } else {
  # The bad
  print("  Error: Expected: [$line]\n");
  print("  Error: Received: [$_]\n\n");
  $status = TEST_FAIL;
      }

      $check_output = scalar(@{$Output});
    } else {
      # redirect outputs from the exe to runpl.log
      print;
    }
  }
  print("\n");

  RunExit(TEST_FAIL, "Generated Test EXE Failed \n") if ($ExitCode);
  RunExit(TEST_FAIL, "Test EXE had bad output \n") if ($status != TEST_PASS);
  RunExit(TEST_FAIL, "Test EXE had bad output \n") if ($LinesMatched != $LinesToMatch);
}

if ($VerifyStrongName && $targetType <= TARGET_MOD) {
  RunExit(TEST_FAIL, "Assembly failed verification:\n") if RunCommand("VerifyStrongName","sn -q -vf $targetName",1);
}

RunExit(TEST_PASS);

exit (1); #safe stop

################################################################################
#
# SUB ROUTINES
#
################################################################################

#############################################################
# RunCompilerCommand -- runs a compiler command line, either directly or through external host
#
sub RunCompilerCommand {
    my ($useHosted, $msg, $cmd, $dumpOutput) = @_;
    
    if($useHosted){
        my $port = "11000";
        if($ENV{HOSTED_COMPILER_PORT} ne ""){
           $port = $ENV{HOSTED_COMPILER_PORT};
        }
        
        my $attempts = 0;
        my $remote = undef;
        until($remote || ($attempts == 10)) {
            $remote = IO::Socket::INET->new(
                                Proto    => "tcp",
                                PeerAddr => "localhost",
                                PeerPort => $port,
                            ) or sleep(1);
            $attempts++;
        }
        until($remote) {
            # if we were unable to connect to the hosted compiler try to run the one we built
            return RunCommand($msg, $cmd);
        }
        
        my $currDir = getcwd();

        # send current directory and full command line to the compiler host process
        print $remote "$currDir|||$compiler_command";

        # first line of respone is the exit code
        my $ExitCode = 0 + <$remote>;

        # remainder of response is output of compiler
        @CommandOutput = <$remote>;

        print "--------------------------------------------------------\n";
        print "Results from hosted compiler\n";
        print "msg: $msg\n";
        print "cmd: $cmd\n";
        print "Exit code: $ExitCode\n";
        print "Error:     $Type\n";
        print @CommandOutput;
        print "--------------------------------------------------------\n";

        # still some issues with reliability of hosted compiler.
        # if compilation unexpectedly fails, try again with standard compiler
        if ($ExitCode && ($Type < TEST_SEEK_ERROR)) {
                return RunCommand($msg, $cmd); 
        }

        return $ExitCode;
    }else{
        return RunCommand($msg, $cmd);
    }
}

#############################################################
# RunCommand -- execute a cmd, redirecting stdout, stderr.
#
# Redirects STDERR to STDOUT, and then redirects STDOUT to the
# argument named in $redirect.  It is done this way since
# invoking system() with i/o redirection under Win9x masks
# the return code, always yielding a 0.
#
# The return value is the actual return value from the test.
#
sub RunCommand {
  #add Win9x Hack here

  unlink ASSERT_FILE;
  my ($msg,$cmd,$dumpOutput) = @_;


#  open SAVEERR, ">&STDERR"; open STDOUT, ">&STDOUT"; 	# save a copy of stderr and redirect to stdout
  select STDERR; $| = 1; select STDOUT; $| = 1;		# enable autoflush

  print("$msg: [$cmd]\n");
  open(COMMAND,"$cmd 2>&1 |") or RunExit(TEST_FAIL, "Command Process Couldn't Be Created: $! Returned $? \n");
  @CommandOutput = <COMMAND>;
  close COMMAND;
  my $result = $?;
#  close STDERR; open STDERR, ">&SAVEERR"; #resore stderr

  print(join("\n", @CommandOutput)) if ($dumpOutput == 1);

  # Test for an assertion failure
  if (-e ASSERT_FILE) {
    print("Failing Test: Assertion detected. Dump Follows:\n");
    open ASSERT, ASSERT_FILE or RunExit(TEST_SKIPPED, "Can't open:" . ASSERT_FILE . " : $!\n");

    while (<ASSERT>){ print; }

    close ASSERT;
    RunExit(TEST_FAIL, "Command Unexpectedly Failed with ASSERT \n");
  }

  return $result;
}

#############################################################
# GetSrc -- Find the source file to build
#
sub GetSrc() {
  # The environment SOURCE var usually defines what to compile
  $_ = $ENV{SOURCE};
  s/\$CWD/$CWD/;
  my $source = $_;
  return($source) if defined($source);

  # Or if there's only one source file in the directory
  my @src = glob("*.fs *.fsx *.fsscript");
  @src <= 1 || RunExit(TEST_SKIPPED, "No SOURCE env var and > 1 source files in the dir: @src \n");
  return(shift @src);
}

#############################################################
# GetExpectedResults -- 
#
# This routine scans the source for magic cookies that show
# the expected results of the compile.  The format of a cookie
# line is:
# //# Expects: [success|warning|error|skip|notin] : [optional text to search for]
# or
# //<Expects Status=[success|warning|error|skip|notin]> [optional text to search for]</Expects>
# or
# //<Expects Status=[success|warning|error|skip|notin]/>
#
# The second colon is not required if there is no text to search for.
# case is insensitive for success|warning|error.  Note that there is
# no semantic difference between success and warning. It's strictly
# for readability in the source.
#
# Skip is a special state that has higher priority than even Error.
# This allows a skip expectation to be added without removing or
# editing any success, warning or error states.  This will be most
# useful when developing tests for features NYI, or, perhaps, for
# features with known bug entries that you don't want showing
# up on the failure list.  Runall will be given a skip status for
# this test.
#
# Note that multiple 'Expects' lines are legal. The most severe
# status wins. If there are 23 success tags and one error tag, then
# error is the assumed condition, and they all might as well have
# said error.  This is useful for documentation purposes if you have
# a file that has 10 warnings, and 2 errors and you want it to be
# clear in the source 'Expects' line.
#
# '//# Expects:' is a literal to make it readable in the source.
#
# Examples:
# //# Expects: Error
#		Compile should fail. No other criteria.
#
# //# Expects: Success
#		Compile should succeed. No other criteria.
#
# //# Expects: Warning: warning C4244: '=' : conversion from 'int' to 'char', possible loss of data
# //# Expects: Warning: warning C4508: 'main' : function should return a value; 'void' return type assumed
#		This will cause run.pl to expect an executable and expect it to run successfully.
#		Compilation will only be considered successful if both of the strings after
#		Warning: are found.  If both strings are not found, the executable is not run.
#		If the above had been errors instead of warnings, it would not look for
#		an executable.
#
# Getting the OUTPUT
# A source file also documents its expected output.  It does so
# in the style of a perl here document.  The startup line takes the
# form '//[optional white space]<<[optional white space][string]
# followed by the expected output, exactly as expected.  No variable
# substitution currently, and newline occur as they will in the output.
# Then on a blank line by itself: [string] is again placed.
# Please make sure the closing line has no white space before or
# after it.  It will be stripped from the front and back of the source.
sub GetExpectedResults(){
  my $src = shift @_;
  my $TEST_SEEK_SKIP = 99;
  my $_skip = 0;
  my $level;
  my $expect = TEST_SEEK_SUCCESS;
  my (@expected, @dontmatch);
  my %seekHash = ( "success", TEST_SEEK_SUCCESS,
       "warning", TEST_SEEK_WARN,
       "error",   TEST_SEEK_ERROR,
       "skip",    $TEST_SEEK_SKIP
     );
  $src =~ s/\s.*//; #grab only the first source file

  open SRC, $src or RunExit(TEST_FAIL, "GetExpectedResults::Can't open source file: $src: $!\n");
  ##########################################################

  push @dontmatch, "internal error";

 ITEM: while(<SRC>) {
    # Looking for output tags
    if (m@//\s*<<\s*(\S+\n)@i) {
      my $here = $1;
      while(<SRC>){
  s@^\s*//@@;
  next ITEM if ($here eq $_);
  push @expected, $_;
      }
      # Detect unterminated expected output tags
      RunExit(TEST_FAIL, "Unterminated output mark: $here  \n");
    }
    # test for command lines
    # test full xml form
    elsif (m@//<CmdLine>\s*(.*?)\s*<(/CmdLine|/)>@i) {
      if (defined($CmdLine)) # Currently support one command line param
  {
    RunExit(TEST_SKIPPED, "<CmdLine> tag found more than once \n");
  }
      $CmdLine = $1 if defined($1);
    }
    #####################################################
    # Rip out the status and search tag (if there is one)
    next unless m@\s*//[#<\s*]@;

    # Test first form
    if (m@//#\s*Expect\w*\s*:\s*(success|warning|error|skip)\s*:?\s*(.*?)\s*$@i) {
      if ($TEST_SEEK_SKIP == $seekHash{$level = lc($1)}) {
  $Skip_platforms = uc($2) if $2
      } else {
  push @match, $2 if $2;
      }
    }
    # test full xml form
    elsif (m@//\s*<Expect\w*\s*.*?\s*Status\s*=\s*\"?(success|warning|error|skip)\"?\s*.*?\s*.?>\s*(.*?)\s*<(/Expect|/)\w*>@i) {
      if ($TEST_SEEK_SKIP == $seekHash{$level = lc($1)}) {
  $Skip_platforms = uc($2) if $2  
      } else {
        my $text = $2;      # save text for later
        my $id =   $1 if (m@//\s*<Expect\w*\s*.*?\s*id\s*=\s*"?(.*?)"?[\s>]@i);
        my $span = $1 if (m@//\s*<Expect\w*\s*.*?\s*span\s*=\s*"?(.*?)"?[\s>]@i);
        $_ = $span; s/\(/\\\(/; s/\)/\\\)/; $span = $_;
        # Ignore the actual text and only look at ID and SPAN X_SKIPFULLDIAGCHECK is set to 1

        # This is typically for LOC or PLOC runs. Unless we are matching for "success", we
        # disregard the text, because it is most likely a localized string.
        my $res;
        if( ($ENV{X_SKIPFULLDIAGCHECK} == 1) && ($level ne "success")) {
            $res = "";
            $text = "";
        } else {
            $res = $text;
            $id = $level . " " . $id if(($level eq "warning") || ($level eq "error"));
        }

        $res = $id . ":.+" . $res if($id ne "");
        $res = $span . ":.+" . $res if($span ne "");
        if (($text ne "") || ($id ne "") || ($span ne "")){
        push @match, $res;
        }
      }
    }
    # test short xml form
    elsif (m@//\s*<Expect\w*\s*Status\s*=\s*(success|warning|error|skip)\s*/\s*>@i) { 
      $level = lc($1);
    }
    # test first form
    elsif (m@//#\s*Expect\w*\s*:\s*(notin)\s*:?\s*(.*?)\s*$@i) {
      push @dontmatch, $2 if $2;
    }
    # test full xml form
    elsif (m@//\s*<Expect\w*\s*Status\s*=\s*\"?(notin)\"?\s*>\s*(.*?)\s*<(/Expect|/)\w*>@i) {
      push @dontmatch, $2 if $2;
    } else {
      next;
    }

    # Actual work!
    $level = $seekHash{$level};
    if ($level == $TEST_SEEK_SKIP) {
      $_skip = 1;
    } else {
      $expect = $level if ($level > $expect); # max
    }
  }
  
  return($expect, $_skip, \@expected, \@dontmatch);
}


#############################################################
#
# GetExpectedTargetInfo
#
# Parse the /out /t(arget) options from $SCFLAGS:
# 1. If /t(arget) is specified then $targetType is set based on the value of the last /t(arget) in 
# $SCFLAGS. If /t(arget) is not specified then we $targetType is automatically set to TARGET_EXE.
#
# 2. If /out is specified then $targetName is set to the value of the last /out in $SCFLAGS else.
# If /out is not specified, $targetName is determined based on $Sources; this is done by appending 
# the appropriate extension to the extension stripped source name and testing if the file exists until   
# we find a match or we expire all possibilities.  
#
sub GetExpectedTargetInfo()
{
  my 	( $_sources, $_SCFLAGS ) = @_;

  use File::Basename;
  my %target_extension_hash = (
             exe     => ['.exe', TARGET_EXE],
             winexe  => ['.exe', TARGET_EXE],
             library => ['.dll', TARGET_DLL],
             module  => ['.netmodule', TARGET_MOD]
            );
  my $target_name;
  my $target_type = 'exe';
  my $target_extension = $target_extension_hash{$target_type}[0];

  if ($_SCFLAGS =~ /.*(--target:|-a)((\w*)|$)/i) {
    #figure out targetname from SCFLAGS
    if("$1" eq "-a") {
        $target_extension = $target_extension_hash{"library"}[0];
        $target_type = "library";
    } else {
        $target_extension = $target_extension_hash{lc($2)}[0] if (defined($2));
        $target_type = $2 if (defined($2));
    }
  }
  if ($_SCFLAGS =~ /.*(-out:|-o )(\".*?\"|\S*)/i) {
    #grab what is after out:
    $target_name = $2;
  }
  if (defined($target_name)) {
    $target_name =~ s/(^\"|$\")//g;     #remove enclosing "s before testing file if exists. '"' is not a valid file name character
    return undef unless( -e $target_name );
  } else { # Figure it out from sources
    foreach my $source (split(/[\s+]/,$_sources)){
      $source = basename( $source );
      $source =~ s/(\w+)\.\w*$/$1$target_extension/;
      $target_name = $source if (-e $source);
      last if ($target_name);
    }
  }

  return undef unless ( -e $target_name);
  return ($target_name, $target_extension_hash{$target_type}[1]);
}

#############################################################
#
# LogTime -- Log the time it took for a test to execute...
#
sub LogTime{
  my($Src, $CompileTime, $RunTime) = @_;
  my($dir) = $main::root;
  open(TIMELOGFILE, ">>$dir\\timing.log");
  print TIMELOGFILE "$Src\t$CompileTime\t$RunTime\n";
  close TIMELOGFILE;
}

#############################################################
#
# RunExit -- Exits the script with the specified value.  
# 
sub RunExit {
  my (
      $exitVal,		# Our exit value
      $cmtStr,		# Comment string to print before exit
     ) = @_;

  my %status_hash = (
         0 => "PASS",
         1 => "FAIL",
         2 => "SKIP"
        );

  print("$cmtStr") if ($cmtStr);

  my $exit_str;
  my $test_result = $exitVal;
  
  # Run POSTCMD if any
  if (defined($ENV{POSTCMD})) {

     # Do the magic to replace known tokens in the
     # PRECMD/POSTCMD: for now you can write in env.lst
     # something like:
     #    SOURCE=foo.fs POSTCMD="\$FSC_PIPE bar.fs"
     # and it will expanded into $FSC_PIPE before invoking it
     $_ = $ENV{POSTCMD};
     s/\$FSC_PIPE/$FSC_PIPE/g;
     s/\$FSI_PIPE/$FSI_PIPE/g;
     s/\$FSI32_PIPE/$FSI32_PIPE/g;
     s/\$CSC_PIPE/$CSC_PIPE/g;
     s/\$VBC_PIPE/$VBC_PIPE/g;

     if (RunCommand("POSTCMD",$_,1)){
  $exitVal = TEST_FAIL;
  $test_result = TEST_FAIL;
  $exit_str .= "Fail to execute the POSTCMD. ";
     }
  } 
  
  if (exists($ENV{SKIPTEST})) {
    $exit_str = "Test Marked: SKIP using Environment Variable SKIPTEST , Tested as: ";
    $exitVal = TEST_SKIPPED;
  } elsif ($Skip) {
    my @platforms;
    $Skip_platforms=~s/\s//g;
    # skip all platforms if no platforms specified
    if ($Skip_platforms eq "") {
      $exit_str = "Test Marked: SKIP, Tested as: ";
      $exitVal = TEST_SKIPPED;
    }
    # treat garbage comas as fatal error
    elsif (!scalar(@platforms = split(/,/,$Skip_platforms))) {
      $exit_str = "Expects Skip Tag Has Errors: \"$Skip_platforms\" Test Was $status_hash{$test_result}, Tested as: ";
      $exitVal = TEST_FAIL;
      $test_result = TEST_FAIL;
    } else {  
      my $platform_to_skip;
      foreach my $match (@platforms) {
    # treat unrecognized platform or garbage as fatal error
    unless ($platform_to_skip = $Platform_Hash{$match}) {
      $exit_str = "Expects Skip Tag Has Errors: \"$Skip_platforms\" Test Was $status_hash{$test_result}, Tested as: ";
      $exitVal = TEST_FAIL;
      $test_result = TEST_FAIL;
      last;
    }
    # don't break here even if we match because we might run into garbage later on
    elsif ($platform_to_skip == $platform) {
      $exit_str = "Test Marked: SKIP, Tested as: ";
      $exitVal = TEST_SKIPPED;
    }
    }
    }
  }
 
  print $exit_str . $status_hash{$test_result} . "\n";

  exit($exitVal);
}


#############################################################
#
# GetCurrentPlatform
#
sub GetCurrentPlatform(){
  # Get current platform and fail if we don't support it
    

  my %proc_hash = (
       _ => 'WIN9X',
       X86_ => 'X86',
       AMD64_ => 'AMD64',
       IA64_ => 'IA64',
       X86_AMD64 => 'WOW_AMD64',
       X86_IA64 => 'WOW_IA64',
      );

  my $platform_string = uc($ENV{PROCESSOR_ARCHITECTURE})."_".uc($ENV{PROCESSOR_ARCHITEW6432});
  my $res = $Platform_Hash{$proc_hash{$platform_string}};

  unless (defined($res)) {
    my $error_string = "PROCESSOR_ARCHITECTURE:" . $ENV{PROCESSOR_ARCHITECTURE} . " with PROCESSOR_ARCHITEW6432:" . $ENV{PROCESSOR_ARCHITEW6432};
    RunExit(TEST_FAIL, "GetCurrentPlatform::Fatal Error: Run.pl does not support the current $error_string \n");
  }
  return $res;
}
