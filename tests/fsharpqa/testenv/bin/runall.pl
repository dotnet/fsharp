# Runall head version 245 edited 17 Mar 2006

###
### See //depot/vcqa/compqa/plan/runall.doc in depot b11vcfs01:2112
### for documentation.
###
### Todo:
###    - Clean up check_* routines so that their return values are more obvious from 
###      their names, and return values are more consistent across routines.
###
###    - Document where we expect a test result to get printed
# GLOBAL VARIABLES

use Config;

my(
    $cleanup,             # type of cleanup requested
    $compilerServerPort,
    $compilerServerProc,  # handle to external process running hosted version of compilers under test
    $debug,               # flag if debugging turned on
    $debug_retval,        # return value during debugging.
    $fail_env,            # name of failures env file
    $FAILENV,             # file handle for $fail_env
    $fail_list,           # name of failures list file
    $filelock,              # hack level for handling the locked problem
    $FAILLST,             # file handle for $fail_list
    $found_runpl,         # if run.pl is copied, set to location of original
    $genrepro,            # generate repro*.bat files?
    $global_env_file,     # name of environment file
    @global_env_list,     # array of global environments
    $gtags,               # tags for global env
    %have_mod,            # hash of current modules dependencies existence
    @required_mods,       # array of required modules
    @keepfiles,           # array of files never to delete
    $knownfail,           # file containing a list of known test failures to skip (none)
    %knownfail,           # keys are labels of tests that should be skipped 
    $local_env_file,      # local environment file name
    @local_env_list,      # array of local environments
    @local_keepfiles,     # array of preexisting files to keep
    $ltags,               # tags for local env
    $maxfails,            # maximum number of failing tests (of all kinds) we allow before stopping run
    $maxtests,            # maximum number of tests that will be run (-1 is infinite)
    $noclobber,           # clobber preexisting RW files?
    $noise,               # amount of output requested, 0 to 4
    $nottags,             # kinds of tests/envs not to run
    $target,              # target platform for test run
    $target_arch,         # target architecture to run on
    @prehook,             # perl scripts to run before each test
    @posthook,            # perl scripts to run after each test
    $psep,                # path separator used by Cwd::cwd()
    $raw_test_count,      # raw number of tests that have been run
    $relaxerrors,         # relax errorlevel 1 exit conditions to exclude timed-out or test error tests
    $resume_mode,         # indicates whether the harness is in resume mode
    $result_file,         # name of results output file
    $RESULTFILE,          # file handle for $result_file
    $root,                # root directory (where runall.pl is run from)
    $results_root,        # directory where results (results.log, runpl.log, failures.lst,
                          # failures.env are stored).
    $runpl_log,           # name of the log file containing test output
    $RUNPLFILE,           # file handle for concatenation of all test output
    $savelog,             # conditions under which test is logged
    $skipped,             # reason for harness skipping a test
    $terse_count,         # of characters printed to a line in terse mode
    $terse_mode,          # boolean; true if -terse is thrown
    $test_file,           # name of testlist file
    $timeout,             # max run time (in mins) before killing test (0 is infinite)
    @totals_count,        # array of outcome counts
    @totals_strings,      # strings associated with $totals_count
    $ttags,               # tags for test list
    $usage_filter,        # usage tags
    $usage_neg_filter,    # usage tags to exclude
    $defarch_file,        # architecture definition file
    %timing,              # test throughput timing
    %Bt_info,             # cl/link output from /Bt switch
    $file_prefix,         # prefix for all files
    $nProcs,              # max number of processes to use
    $cBatchMax,           # max numbers of tests in a batch
    $mpdebug,             # debug level for multiprocessing runs
    $multiproc_child,     # bool: is this a child run of a multiproc run?
    $xml,                 # 1 or 0 indicating whether -xml was used, 1 means it was
    $xmlresult_file,      # name of results output file
    $XMLRESULTFILE,     # file handle for $xmlresult_file
    $xmlrunpl_file,       # name of xml log file containing test output
    $XMLRUNPLFILE,      # file handle for $xmlrunpl_file
    $xml_previous_test,   # used when resuming on -xml to determine if we are still in a <Test ...> block.
                          # Also used to determine when a <Test ...> block has ended
    $xmlbldcfg,           # string representing the BuildConfiguration attribute in runpl.xml
    $xmlproduct,           # string representing the Product attribute in runpl.xml
    $xmlsuitename,          # Name of the suite for xml results (optional)
    $xmlchangelist, 
    $xmlownername, 
    $xmltestrunbugdb,
    $xmltestrunbugid,
    $xmltoolname,
    %xmldups,              # Hash representing tests with duplicate permutations
	$last_groupID,			# Last groupID seen (or read from Xrunpl.xml in resume mode
    $running_failures,      # boolean: are we (likely) running a failures.lst?
    %target_map,           # target names map
    %readonlylogs,         # Hash representing tests with read-only runpl.logs
	$isWin9x,				# Are we running on Windows 95-based OS?
	$permutation,			# desired permutation
   $testlabel              # Stores a non-unique test label for the test
);

# constants
my $PASSED = 0;
my $FAILED=1;
my $SKIPPED=2;
my $CASCADE=3;
my $NO_RESULT=4;
my $TIMED_OUT=5;
my $TEST_ERROR=6;
my $GROUPSIZE = 8192;     # used by -xml to determine the GroupID attributes in runpl.xml
                          # The value of 8192 was picked b/c this is the value that xresult.exe
                          # uses by default.

my %RETVAL_TO_NAMES = ($PASSED => 'passed',
$FAILED => 'failed',
$SKIPPED => 'skipped',
$CASCADE => 'cascade',
$NO_RESULT => 'no_result',
$TIMED_OUT => 'timed_out',
$TEST_ERROR => 'test_error');
# REVIEW: it'd be nice to have a more compact version of this
my $x;
my %NAMES_TO_RETVAL;
foreach $x (keys %RETVAL_TO_NAMES) { $NAMES_TO_RETVAL{$RETVAL_TO_NAMES{$x}} = $x };

my $TIMING_OFF = 0;
my $TIMING_GLOBAL = 1;
my $TIMING_LOW = 2;
my $TIMING_HIGH = 3;
my %TIMING_NAMES_TO_VALS = ('off' => $TIMING_OFF, 'gl' => $TIMING_GLOBAL, 'lo' => $TIMING_LOW, 'hi' => $TIMING_HIGH);
my %TIMING_VALS_TO_NAMES;
foreach $x (keys %TIMING_NAMES_TO_VALS) { $TIMING_VALS_TO_NAMES{$TIMING_NAMES_TO_VALS{$x}} = $x };

my $FILELOCK_OFF = 0;
my $FILELOCK_ABORT = 1;
my $FILELOCK_SYNC = 2;
my $FILELOCK_USER = 4;

my $NORMAL_EXITVAL = 0;
my $TEST_FAILURES_EXITVAL = 1;
my $SWITCH_ERROR_EXITVAL = 2;
my $INPUT_ERROR_EXITVAL = 3;
my $STATE_ERROR_EXITVAL = 4;
my $FILE_ERROR_EXITVAL = 5;
my $OTHER_ERROR_EXITVAL = 9;

my $perl = $^X;

BEGIN {
    @required_mods = ("Win32\\Process.pm");
    my $curr_mod;

    for (@INC) {
      for $curr_mod (@required_mods) {
        if (-e "$_\\$curr_mod") {
           require "$_\\$curr_mod";
           $have_mod{$curr_mod} = 1;
    }
      }
    }
}

# MAIN STRUCTURES

# Each element of @global_env_list and @local_env_list is an array reference
# with five parts:
#
#	[0]		the index of the environment (used in repro*.bat)
#	[1]		subset tags
#	[2]		in global env, added subset tags; in local, usage tags
#	[3]		the label for the environment
#	[4]		hash reference containing the environment
#
# If there was no explicit label in the env file, [3] == [0].  When there
# is no env file, runall.pl constructs a "dummy" array with one element,
# and sets the label field to be undefined.
#
# Each element of @test_list is an array reference containing three parts:
#
#	[0]		subset tags
#	[1]		usage tags
#	[2]		test directory
#
# [3] actually contains the comment from the end of the test line, if there
# was one, but it is just ignored.
#


# runall.pl needs at least this version of Perl to work correctly
require 5.001;

use Cwd;
use strict;
use IO::File;
use IO::Handle;

#####################
# UTILITY FUNCTIONS #
#####################


#
# cwd -- convert path separators in paths returned from Cwd::cwd to \'s as 
#        appropriate
#
sub cwd 
{
    my $cwd = Cwd::cwd();
    if (defined($psep)) {
	$cwd =~ s#$psep#\\#g;	  
    }
    return $cwd;
}

#
# leave
#
# End with diagnostic and non-zero exit code.
#
sub leave($$) {
    my $errtxt = shift;
    my $exitval = shift;

    print $errtxt;
    print "$0 terminated abnormally.\n";

	write_to_file( \$RESULTFILE,  $errtxt );
	write_to_file( \$RESULTFILE,  "$0 terminated abnormally.\n" );
	
    exit($exitval);
}

#
# assert
#
# End with diagnostic and non-zero exit code.
#
sub assert {
    my $errtxt = shift;

	write_to_file( \$RESULTFILE,  "ASSERT:\n" );
	write_to_file( \$RESULTFILE,  $errtxt );
	write_to_file( \$RESULTFILE,  "$0 terminated abnormally.\n" );
	
    print "ASSERT:\n";
    print $errtxt;
    print " $0 terminated abnormally.\n";

    exit(1);
}

#
# return 0 if nothing no-resulted, cascaded, failed, timed out or test errored. Ignore last two categories if running with
# the -relaxerrors switch (which should only be used for Gauntlet-style situations) to help robustify BTGauntlet.
#
sub normal_exit {
    exit ($NORMAL_EXITVAL) if ( !(grep($_ > 0, @totals_count[$FAILED, $CASCADE, $NO_RESULT])) && ($relaxerrors == 1) );
    exit ($NORMAL_EXITVAL) unless ( grep($_ > 0, @totals_count[$FAILED, $CASCADE, $NO_RESULT, $TIMED_OUT, $TEST_ERROR] ) );
	exit ($NORMAL_EXITVAL) if $debug_retval;
    exit ($TEST_FAILURES_EXITVAL);
}

#
# quote_path -- quote a path if it a.) is not already quoted and b.) contains
#               spaces
#
sub quote_path
{
    my $path = shift;

    return $path if not $path =~ /\s/;
    return $path if $path =~ /^\"\.+\"$/;

    return "\"$path\"";
}

#
# trim -- trim whitespace at the head and/or tail of each string in a list
#
sub trim 
{
    my @strs = @_;

    for (@strs) {
	s/^\s+//;
	s/\s+$//;
    }

    return @strs;
}

#
# expand -- apply variable expansion to a string
#
sub expand
{
    my ($first, $second) = @_;

    if (ref $second eq "HASH") {
	$first =~ s/(%([^%]+)%)/defined($second->{uc($2)})?$second->{uc($2)}:$1/eg;
    } else {
	$first =~ s/(%([^%]+)%)/defined($ENV{uc($2)})?$ENV{uc($2)}:$1/eg;
    }
    return($first);
}


#
# union -- get the union of two comma-delineated sets
#
# If one of the set contains a *, ignore it.  This is not a true union, but
# it serves the purpose here better.
#
sub union
{
    my ($x, $y) = @_;

    if (($x =~ /(^|,)\*(,|$)/) || !$x) {
	return ($y);
    }
    if ($y =~ /(^|,)\*(,|$)/) {
	return ($x);
    }
    $y = join ',', grep {$x !~ /(^|,)$_(,|$)/i} split(/,/, $y);
    if (!$y) {
	return ($x);
    }
    return "$x,$y";
}


#
# intersect -- get the intersection of two comma-delineated sets
#
sub intersect
{
    my ($x, $y) = @_;

    if ($x =~ /(^|,)\*(,|$)/) {
	return($y);
    }
    if ($y =~ /(^|,)\*(,|$)/) {
	return($x);
    }
    return join ',', grep {s/\*/.*/; $x =~ /(^|,)$_(,|$)/i} split(/,/, $y);
}


#
# not_in -- return everything in one set that isn't in another set
#
sub not_in
{
    my ($x, $y) = @_;

    if ($y =~ /(^|,)\*(,|$)/) {
	return('');
    }
    if ($x =~ /(^|,)\*(,|$)/) {
	return($x);
    }
    return join ',', grep {$y !~ /(^|,)$_(,|$)/i} split(/,/, $x);
}

#
# verify_unlink -- unlink and verify
#
sub verify_unlink
{
	my $delete = "del /f";
	$delete = "del" if $isWin9x; # VCQA:6953 del /f isn't available on Win9x
    foreach (@_) {
		# Try to delete - call system DEL to fix unicode filename issues.
	system "DEL \"$_\" > nul 2> nul";
        if (-e $_) {
            # Make file read/writeable
            chmod (0666, $_);
            unlink $_;
    		if (-e $_) {
    			# Maybe it's an access issue. Try administrator delete.
    			system("$ENV{'ADMIN_PIPE'} $delete /q $_");
    			if (-e $_) {
    				# Maybe it's a file lock. Wait 30 seconds, then try to delete again.
    				sleep 30;
    				system("$ENV{'ADMIN_PIPE'} $delete /q $_");
    				if (-e $_) {
    					# Maybe we can rename the file instead of deleting it.
    					my $tmpname = gen_uuid();
    					system("$ENV{'ADMIN_PIPE'} ren $_ $tmpname");	
    					if (-e $_) {
    						# Give up and suffer the consequences.
    						print_noise("WARNING: Can't delete or rename $_!", 1);
    						return 1;	
    					}
    				}
    			}
    		}		
        }
    }
	return 0;
}

#
# text2xml -- takes a text stream and returns an xml compliant text stream, i.e. text2xml("a < b") = "a &lt; b"
#
sub text2xml
{
  my ($str) = @_;
  $$str =~ s/&/&amp;/g;
  $$str =~ s/</&lt;/g;
  $$str =~ s/>/&gt;/g; 
  $$str =~ s/'/&apos;/g;
  $$str =~ s/"/&quot;/g;
  $$str =~ s/%/&#37;/g;

  # REVIEW - Should really use Perl transliteration below instead, tr///
  #
  #Convert anything outside of ANSI Character set into an 'html Unicode.'
  $$str =~ s{(.)} {
                    if (ord($1) == ord("\n") || ord($1) == ord("\t")) {
	                $1;
                    }
                    elsif (ord($1) < ord(' ')) # Do not print control codes
		    {
			'';
		    }
                    elsif (ord($1) > ord('~')) { # > ASCII printable
			"&#" . ord($1) . ";";
                    }
                    else
                    {
			$1;
                    }
		   }xge;
}

#
# gen_uuid -- generate a unique used id
#
sub gen_uuid
{
  my $uuid;

  # Adding this BEGIN block so we can cache the results of 
  # as many system calls as possible.
  BEGIN {
      # Only want to seed once
      srand(time() ^ $$);

      my $avail = 0;
      $uuid = system("uuidgen.exe >nul 2>&1") >> 8;
      $avail = ($uuid == 0);
      sub uuid_exe_avail { return \$avail; }

      my $mac = "";
      sub get_mac { return \$mac; }
    }

  # Need to generate 32 psuedo-random chars to compose a uuid
  if (!${uuid_exe_avail()}) {
      if (${get_mac()} eq "") {
          open IPCONFIG, "ipconfig /all |";
          while(<IPCONFIG>) {
	          $uuid = "$1$2$3$4$5$6" if(/.*:[ \t]*(..)-(..)-(..)-(..)-(..)-(..)$/);
	          # Try to get something a little more random...
	          last if(!($uuid =~ /000000000000/));
	      }
          close(IPCONFIG);
    	  ${get_mac()} = $uuid;
      }
      else {
          $uuid = ${get_mac()};
      }

      #Obtain 10 chars from time. time() return the number of non-leap seconds since what the
      # current system considers to be the epoch, usually 01-01-1970. At the time of coding the
      # numbers of digits in the number was 10. So from the day of coding out this number should
      # always be >= 10 digits.
      $uuid .= substr(time(), 0, 10);

      #not very random but good enough for our purposes
      #10 more char to go... + 4 extra for dashes
      $uuid .= chr(ord( (rand() > 0.5) ? 'A' : 'a') + int(rand(6))) for (0..13);

      #swap elements randomly
      (substr($uuid, int(rand(35)), 1) = 
       substr($uuid, int(rand(35)), 1)) for (0..int(rand(100)));

      #add the dashes
      substr($uuid, $_, 1) = '-' for (8, 13, 18, 23);
  } else {
      $uuid = `uuidgen.exe 2>&1`;
  }

  chomp $uuid;
  return $uuid;
}

#
# get_lang -- Puts the host language and target language in the two reference
#             scalars passed in.
#
sub get_lang
{
    my ($host_lang, $target_lang) = @_;
    $$host_lang = `getoslang.exe 2>&1`;
    $$target_lang = `$ENV{SIMULATOR_PIPE} getoslang.exe 2>&1`;

    # put language in the form that VCTR expects, either
    # English, Japanese, German, or Arabic
    # Cannot just return the string from getoslang.exe, b/c
    # getoslang.exe will return things like English (United States)
    foreach my $l ($host_lang, $target_lang)
    {
	if ($$l =~ /^(English|Japanese|German|Arabic)/) {
	    $$l = $1;
	}
	else
	{
	    $$l = "Unknown";
	}
    }
}

#
# get_os -- Returns the name of the OS currently running.
#
sub get_os 
{
    my ($host_os, $target_os) = @_;
    $$host_os = `getosver.exe 2>&1`;
    $$target_os = `$ENV{SIMULATOR_PIPE} getosver.exe 2>&1`;

    # Put os ver in the form that VCTR expects, either
    # NT4 , Win98 , Win2K , WinXP , XPWow , .NetSvr , Win95 , WinME , WinXP.
    # Ideally this transformation would be performed using a table, but there is too much variance
    # in what getosver.exe produces, given that it show build numbers, service packs, and such.
    foreach my $o ($host_os, $target_os)
    {
    SWITCH: { $_ = $$o;
	      /^Microsoft Windows Server 2003[^\d]+(64-bit)?/  && do {
		  if ($1) {
		      $$o = (/Professional Service Pack/) ? "WinXP" : ".NetSvr";
		  } else {
		      $$o = ".NetSvr";
		  };
		  last SWITCH;
	      };
	      /^Microsoft Windows XP \(Longhorn\)[^\d]+(64-bit)?/           && do {
		  $$o = "Longhorn";
		  last SWITCH;
	      };
	      /^Microsoft Windows XP[^\d]+(64-bit)?/           && do {
		  $$o = "WinXP";
		  last SWITCH;
	      };
	      /^Microsoft Windows 2000/                        && do {
		  $$o = "Win2k";
		  last SWITCH;
	      };
	      /^Microsoft Windows NT.+4\.0/                    && do {
		  $$o = "NT4";
		  last SWITCH;
	      };
	      /^Microsoft Windows 95/                          && do {
		  $$o = "Win95";
		  last SWITCH;
	      };
	      /^Microsoft Windows 98/                          && do {
		  $$o = "Win98";
		  last SWITCH;
	      };
	      /^Microsoft Windows Millennium Edition/          && do {
		  $$o = "WinME";
		  last SWITCH;
	      };
	      $$o = "Unknown";
	  }
    }
}

#
# dump_machine - replicates link -dump -headers without shelling out to the linker
# returns machine type in "VCTR-friendly" format
#
sub dump_machine
{
    my $binary = shift;    
	my $machine;
	if (open BIN, "<$binary") {
		binmode BIN;
		my $buf;
		read BIN, $buf, 2;
		if ((unpack "H4", $buf) eq "4d5a") { # DOS header must start with MZ
			seek BIN, 60, 0;
			read BIN, $buf, 2;
			my $e_lfanew = unpack "v2", $buf; # get pointer to PE32 header
			seek BIN, $e_lfanew, 0;
			read BIN, $buf, 4;
			if ((unpack "H8", $buf) eq "50450000") { # PE32 header must start with PE\0\0
				seek BIN, $e_lfanew + 4, 0;
				read BIN, $buf, 2;
				$machine = unpack "H4", $buf; # machine type is next two bytes
			}
		}
		close BIN;
	}
	$machine eq "0002" and return "ia64";
	$machine eq "6486" and return "amd64";
	$machine eq "4c01" and return "x86";
	return "Unknown";
}
		
#
# get_clver - returns the target arch, host arch, build num, and version num
#
sub get_clver
{
    my ($saveCL, $save_CL_, $save_LINK_, $tool_info, $cl_loc);
    my %vc_info;

    #Save compiler ver, compiler build, and win ver
    $saveCL    = $ENV{CL};
    $save_CL_  = $ENV{_CL_};
    $save_LINK_  = $ENV{_LINK_};
    $ENV{CL}   = undef;
    $ENV{_CL_} = undef;
    $ENV{_LINK_} = undef;
    $tool_info= "";

	# Check that the tools are actually in the path
	# Note that running 'cl -Bv' actually returns an error code of 2 (file not found)
	# So we check just that cl exists, and trust the -Bv will work
	if (0 == (system("cl >nul 2>&1") >> 8)) {
		$tool_info = `cl -Bv 2>&1`;
	} else {
		$vc_info{"vernum"} = $vc_info{"buildnum"} = $vc_info{"targetarch"} = "Unknown";
		$vc_info{"hostarch"} = "Unknown";
	}

	# snag the version, build, and target arch
	if ($tool_info =~ /Version (\d+)\.(\d)\d+\.(\d+)(\.\d+)?\s+for\s+(.+)/) {
		# $1: Major Version
		# $2: Minor Version
		# $3: Build Number
		# $4: Dot build number (e.g. LKGs)
		# $5 (or $4): Architecture compiler was built for

		$vc_info{"vernum"} = "v" . ($1 - 6) . "." . ($2 - 0); # subtract 6 from $1 to get the 
			# more common form of major version; subtract zero from $2 to strip leading zeros
		$vc_info{"buildnum"} = $3;

		if( $target_arch == -1) {
			# target architecture is not set; try to find it out from the cl.exe
			$vc_info{"targetarch"} = $5;
			# get the arch that the compiler is targeting
			if ( $vc_info{"targetarch"} =~ /x86/i ) {
				$vc_info{"targetarch"} = "x86";
			}
			elsif ( $vc_info{"targetarch"} =~ /x64/i ) {
				$vc_info{"targetarch"} = "amd64";
			}
			elsif ( $vc_info{"targetarch"} =~ /IA-64/i ) {
				$vc_info{"targetarch"} = "ia64";
			}
			elsif ( $vc_info{"targetarch"} =~ /Itanium/i ) {
				$vc_info{"targetarch"} = "ia64";
			}
			elsif ( $vc_info{"targetarch"} =~ /IPF/i ) {
				$vc_info{"targetarch"} = "ia64";
			}
			else {
				$vc_info{"targetarch"} = "Unknown";
			}
		} else {
			$vc_info{"targetarch"} = $target_arch;
		}
	}

	# get the arch that the compiler was built to run on
	$vc_info{"hostarch"} = "Unknown";
	if ($tool_info =~ /(\w:\\.+\\cl.exe)/m) {
		$cl_loc = $1;
		$vc_info{"hostarch"} = dump_machine("$cl_loc");
	}


	# if we have any "Unknown" fields in %vc_info then $xml won't give the correct results
	if ($xml && (grep {/Unknown/} values(%vc_info)) ) {
		print "WARNING: Can't determine toolset targets and/or version info;\n" .
			  "         -xml:yes will not give correct results.\n";
	}
	
    $ENV{CL}   = $saveCL;
    $ENV{_CL_} = $save_CL_;
    $ENV{_LINK_} = $save_LINK_;
	return \%vc_info;
}


###########################
# INITIALIZATION ROUTINES #
###########################


#
# init_globals -- set up global variables to default values
#
sub init_globals
{
    # Examine our platform and Perl version to intuit the path separator 
    # that is used in strings returned from Cwd::cwd()
    if ((($^O eq "MSWin32") || ($^O eq "MSWin64")) && ($] > 5.00307)) {
	$psep = '/';
    }

    # If no target platform was specified, attempt to discover it
    # from %PROCESSOR_ARCHITECTURE%.  Default to 'win9x' if the env var
    # has not been set.
    if (!defined($target)) {
	$target = defined($ENV{PROCESSOR_ARCHITECTURE}) ?
	$ENV{PROCESSOR_ARCHITECTURE} : 'win9x';
    }
    $target = lc($target); # VCQA#2226 
    $ENV{TARGET_ARCHITECTURE} = $target;
	
	# Set a flag if we're running on Win9x. Assume that Win32 is built into Perl.exe but eval it for safety.
	$isWin9x = 0;
	eval {
		$isWin9x++ if Win32::IsWin95();
	};

    # This set of variables control settings that the user can
    # change via command-line switches
    $cleanup = 'default';
    $genrepro = 'no';
    $test_file = 'test.lst';
    $result_file = 'results.log';
    $xmlresult_file = 'results.xml';
    $xml_previous_test = '';
    $fail_list = 'failures.lst';
    $fail_env = 'failures.env';
    $filelock = -1; # invalid value that should be set to 1 after the parsing is done
    $knownfail = '';
    $local_env_file = 'env.lst';
    $noclobber = 1;
    $maxfails = -1;
    $maxtests = -1;
    $raw_test_count = 1;
    $resume_mode = 0;
    $runpl_log = 'runpl.log';
	$running_failures = 0;
    $xmlrunpl_file = 'runpl.xml';
    $xml = 0;
    $xmlbldcfg = undef;
    $xmlproduct = undef;
    $xmlsuitename = undef;
    $xmlchangelist = undef; 
    $xmlownername = undef; 
    $xmltestrunbugdb = undef;
    $xmltestrunbugid = undef;
    $xmltoolname = undef;
	$last_groupID=0;
    $savelog = 'fail';
    $gtags = '';
    $ttags = '';
    $ltags = '';
    $nottags = '-,!';
    @keepfiles = ('run.pl',
	'run.exe',
    'delete.lst',
    'keep.lst',
    'notarget.lst',
    $local_env_file,
    # In adding $runpl_log to @keepfiles, we are depending on
    # the fact that we open a new $runpl_log for output
    # (rather than appending to an existing one) every time the
    # test is run.  See cmd_redirect to verify that this is true.
    #
    # The advantage of doing this is that we will
    # not blow away $runpl_log when we move on to the next test
    # directory, even if cleanup_directory is called.
    $runpl_log);

    $noise = 1;
    $debug = 0;
    $mpdebug = 0;
    $multiproc_child = 0;
    $debug_retval = 0;
    $usage_filter = '*';
    $usage_neg_filter = '';
    $terse_mode = 0;
    $timeout = -1;
    $target_arch = -1;

    # internal global variables
    $terse_count = 0;
    $timing{'level'} = $TIMING_GLOBAL;
    $root = cwd();
    # results_root is also set by a switch, but put here to grab 
    # result from $root.
    $results_root = $root;
    @totals_count = (0, 0, 0, 0, 0, 0, 0);
    @totals_strings = ('passed', 
    'failed', 
    'were skipped', 
    'were cascaded failures', 
    'returned "no result"',
    'timed out',
    'had test errors',);

    # Default archdflt.lst is located in the same place as runall, handle both / and \ as path separators
    $defarch_file = substr($0, 0, (((rindex $0, '\\') > (rindex $0, '/')) ? (rindex $0, '\\') : (rindex $0, '/')) + 1 ) . 'archdflt.lst';

    $file_prefix = "";

    $nProcs = 1;
    $cBatchMax = 2;
}

#
# rename_files -- If output files exist, rename them with a common suffix
#
sub rename_files
{
    my (@files) = @_;
    my ($file_exists, $rename, $i, $suffix);

    $file_exists = 1;

    while ($file_exists) {
	$file_exists = 0;
	$suffix = $i ? "." . sprintf("%02d", $i) : "";
	$i++;
	for (@files) {
	    if (-e $_ . $suffix) {
		$file_exists = $rename = 1;
		last;
	    }
	}
    }

    # Only rename files if we found that one or more files already exist
    if ($rename) {
	print_noise ("One or more output files exist from a previous test run and " .
	"will be renamed:\n", 0);

	for (@files) {
	    (not -e $_) and next;
	    my $new_name = $_ . $suffix;
	    print_noise ("\tRenaming '$_' to '$new_name'\n", 0);
	    unless (rename($_, $new_name)) {
		leave("Runall.pl line " . __LINE__ . ": Fatal Error: $!\nCould not rename '$_' to '$new_name'.\n", $FILE_ERROR_EXITVAL);
	    }
	}
    }
}

#
# rename_runall_files -- rename all the files used by a runall run.
#
sub rename_runall_files() {
    # If we're not resuming a test run, and any of these files exist, rename them
    # before beginning test run.  In resume mode, we want to append to files that
    # already exist
    rename_files("$results_root\\$file_prefix$result_file", "$results_root\\$file_prefix$runpl_log", "$results_root\\X$file_prefix$runpl_log", "$results_root\\$file_prefix$fail_list", "$results_root\\$file_prefix$fail_env", "$results_root\\$file_prefix$xmlresult_file", "$results_root\\$file_prefix$xmlrunpl_file", "$results_root\\X$file_prefix$xmlrunpl_file")
    unless $resume_mode;
};

#
# open_files -- get file handles for the various output files
#
# Depends on $resume_mode, $root, $runpl_log, FAILLST, FAILENV, RESULTFILE
#
sub open_files($)
{
    my $mode = $_[0];

    my $failures_mode;
    my $result_mode;


    if ($mode eq "resume") {
		$failures_mode = ">>";
		$result_mode   = "+<";
    } elsif ($mode eq "reopen") {
		$failures_mode = ">>";
		$result_mode   = ">>";
    } else {
		$failures_mode = ">";
		$result_mode   = ">";
		verify_unlink("$results_root\\$file_prefix$runpl_log");
    }
   
    $FAILLST = new IO::File;
    unless ($FAILLST->open( "$failures_mode $results_root\\$file_prefix$fail_list")) {
		leave("Runall.pl line " . __LINE__ . ":  Fatal Error: $!\nCould not open fail list '$file_prefix$fail_list'.\n", $FILE_ERROR_EXITVAL);
    }
    $FAILENV = new IO::File;
    unless ($FAILENV->open("$failures_mode $results_root\\$file_prefix$fail_env")) {
		leave("Runall.pl line " . __LINE__ . ":  Fatal Error: $!\nCould not open fail env '$file_prefix$fail_env'.\n", $FILE_ERROR_EXITVAL);
    }
    $RESULTFILE = new IO::File;
    unless ($RESULTFILE->open( "$result_mode $results_root\\$file_prefix$result_file")) {
		leave("Runall.pl line " . __LINE__ . ":  Fatal Error: $!\nCould not open results file '$file_prefix$result_file'.\n", $FILE_ERROR_EXITVAL);
    }
    $RUNPLFILE = new IO::File;
    unless ($RUNPLFILE->open( "$failures_mode" . "$results_root\\X$file_prefix$runpl_log")) {
		leave("Runall.pl line " . __LINE__ . ":  Fatal Error: $!\nCould not open log file 'X$file_prefix$runpl_log'.\n", $FILE_ERROR_EXITVAL);
    }

    my @handles = ($FAILLST, $FAILENV, $RESULTFILE, $RUNPLFILE);
    
    if ($xml) {
		$XMLRESULTFILE = new IO::File;
        unless ($XMLRESULTFILE->open( "$failures_mode $results_root\\$file_prefix$xmlresult_file")) {
		    leave("Runall.pl line " . __LINE__ . ":  Fatal Error: $!\nCould not open xml results file '$file_prefix$xmlresult_file'.\n", $FILE_ERROR_EXITVAL);
        }

		# Get last GroupID from existing runpl.xml if resume_mode
    	if ($mode eq "resume") {
    		open RUNPL_XML_FILE, "<$results_root\\X$file_prefix$xmlrunpl_file" or leave("Runall.pl line " . __LINE__ . ": Could not open: $file_prefix$xmlrunpl_file to find last GroupID.\n", $FILE_ERROR_EXITVAL);
			while (<RUNPL_XML_FILE>) {
				m/GroupID\s*=\s*'(\d+)'\s*/ and $last_groupID = $1;
			}
			$last_groupID++;
			close RUNPL_XML_FILE;
		}
		$XMLRUNPLFILE = new IO::File;
        unless ($XMLRUNPLFILE->open( "$failures_mode" . "$results_root\\X$file_prefix$xmlrunpl_file")) {
		    leave("Runall.pl line " . __LINE__ . ":  Fatal Error: $!\nCould not open xml log file 'X$file_prefix$xmlrunpl_file'.\n", $FILE_ERROR_EXITVAL);
        }
		push @handles, $XMLRESULTFILE;
		push @handles, $XMLRUNPLFILE;
    }
    
    # seek to the end upon re-open
    if ( $mode eq "reopen" ) {
	foreach ( @handles ) {
	    $_->seek( 0, 2 );
	};
	# print $RESULTFILE "# FILES REOPENED!\n";
    };
    
    use Fcntl ':flock'; # import LOCK_* constants
    
    # Autoflush all files
    my $selected_fh = select;
    no strict 'subs';
    foreach ( @handles ) {
	$_->autoflush( 0 );

	#
	# REVIEW: This might help all the wierd virus-detection related
	#         issues we've had, but it also means you can't look at
	#         the output while runall is running.
	#
	#flock $_, LOCK_EX  || print("Cannot lock output file!\n");
    }
    select $selected_fh;
}

#
# close_files -- close file handles for the various output files
#
sub close_files() {
    if ($xml)
    {
	$XMLRESULTFILE->close();
	$XMLRUNPLFILE->close();
    }

    $RUNPLFILE->close();
    $RESULTFILE->close();
    $FAILLST->close();
    $FAILENV->close();
};


#
# write_to_file -- write some text ($_[1]) to a file ($_[0]). Does the right cleanup on failure.
# Perl's default behavior on print() failure is to close the stream.
#
sub write_to_file($$) {
    my $fh = $_[0];
    my $output = $_[1];
    ##
    # 1 try works fine because after an error has occurred, print always seems to return false, even if output actually appears in
    # the file.
    #
    my $tries = 5;
    my $tried_reopen = 0;

    # Uncomment these for STRESS.
	#close_files();
	#open_files("reopen");

    for ( my $try = 1; $try < ($tries+1); $try++ ){
	     if (print {$$fh} $output) {
		 if ( $try > 1 ) {
		     print "COULD WRITE TO HANDLE (Try $try)! $!\n";
		 };
		 last;
	     };

	     print "COULD NOT WRITE TO HANDLE (Try $try)! $! \"$output\"\n";
	     print "Windows Error: $^E GetLastError: ".Win32::GetLastError()."\n";
	     
	     #
	     # Clear up the error condition on the file handle.
	     # The seek appears to be required.
	     #
	     $$fh->clearerr;
	     seek $$fh, 0, 1 ;
	     
		 # TODO: We shouldn't be bailing out of runall. We should perform some more reasonable error recovery.
		 # For example, if the child process log file can't be read then go ahead and note this fact in the main
		 # runall and continue the run. Bailing out of an unattended process for a nonfatal error is a bad idea.
		 # Also, consider exponential fallback like get_filehandle uses.
	     if ( $try == $tries ) {
		 leave("Even after re-opening, couldn't write to handle. Giving up.", $FILE_ERROR_EXITVAL) if ($tried_reopen == 1);
		 $try = 0;
		 $tried_reopen++;
		 print "\n\nREOPENING OUTPUT FILES!\n\n";
		 close_files();
		 open_files("reopen");
	     };

         sleep 5;
    };
}

    
#
# get_filehandle -- returns a filehandle for reading or writing with retries when it fails to open the file
# in: mode, filename
# out: file handle
#

sub get_filehandle($$) {
	my $mode = shift;
	my $filename = shift;
	my $FILEHANDLE = new IO::File;

	my $sleep = 2;
    my $tries = 5;
    my $tried_reopen = 0;

    for ( my $try = 1; $try < ($tries+1); $try++, $sleep = $sleep * 2 ){
		if ($FILEHANDLE->open("$mode $filename")) {
			if ($try > 1) {
				print "COULD OPEN $filename (Try $try)! $!\n";
			};
			return $FILEHANDLE;
		}
		else {
			print "STALLING ON $filename (Try $try)! $!\n";
		    print "Windows Error: $^E GetLastError: ".Win32::GetLastError()."\n";
		}
		
		# TODO: We shouldn't be bailing out of runall. We should perform some more reasonable error recovery.
		# For example, if the child process log file can't be read then go ahead and note this fact in the main
		# runall and continue the run. Bailing out of an unattended process for a nonfatal error is a bad idea.
		if ($try > $tries) {
		    print "COULD NOT OPEN $filename (Try $try)! $!\n";
		    print "Windows Error: $^E GetLastError: ".Win32::GetLastError()."\n";
	    	leave("Runall.pl line " . __LINE__ . ": Could not open: '$filename'.\n", $FILE_ERROR_EXITVAL);
		};
		sleep $sleep;	
	};
}

#####################
# PRINTING ROUTINES #
#####################

#
# print_help -- print short help message
#
sub print_help
{
    print <<ENDHELP;
Usage:
perl runall.pl <switches>
-cleanup:[yes|no|default]       always or never do cleanup, or let
                                runall.pl decide (default)
-debug:[off|on|fail|clean|list] toggle debug mode (off)
-global <file>                  name global environment file (NONE)
-fail <file>                    name failures output files (failures)
-repro:[yes|no|all]             generate repro batch files (no)
-help                           print this message
-keep <file>                    never delete <file> (run.pl, delete.lst,
                                keep.lst, env.lst)
-log <file>                     output file for run.pl (runpl.log)
-savelog:[all|pass|fail|none]   save run.pl output (fail)
-local:[yes|no]                 do/don't use local environment files (yes)
-maxfails:#                     stop after # tests fail (-1, run all tests)
-maxtests:#                     stop after # tests (-1, run all tests)
-noise:#                        amount of output (1)
-prehook <file>                 * name a prehook Perl script (NONE)
-posthook <file>                * name a posthook Perl script (NONE)
-results <file>                 name results output file (results.log)
-resultsroot <directory>        store all log files here (.\)
-resume:[yes|no]                resume tests based on results file (no)
-target:string                  target platform for test run (auto-detect)
-terse                          print very limited results to screen
-test <file>                    name testlist file (test.lst)
-env:"string"                   * run a particular env.lst permutation (NONE)
-knownfail <file>               name of known failures list (NONE)
-timeout:<number>               minutes before killing test (0 is infinite)
-timing:[off|global|low|high]   * get detailed timing info on the run (global)
-[g|t|l]tags:string             test types to run (*)
-nottags:string                 types not to run (BLANK)
-usage:string                   usage tag filter (BLANK)
-notusage:string                usage tag negative filter (BLANK)
-procs:<number>                 max. number of processes to use
-mpdebug:<number>               noise level for multiproc debugging
-batch:<number>                 max. number of tests per batch
-xml:[yes|no]                   toggle XML results format (no)
-xml*                           * xml string switches, see -help:xmlstrings
-filelock:#                     * hacks to help prevent locked log files (1)

Default values in parentheses.  Flags can be specified multiple times;
the last one is the one that is used, except for "-keep" which adds
the filename to the default list instead of replacing it and "-prehook"/
"-posthook" which add multiple hooks in the specification order. Topics
marked with an asterisk (*) have extended help: type runall -help:topic.
ENDHELP
exit 0;
}


#
# print_extended_help
#
sub print_extended_help($)
{
	my $topic = shift;
	my $text = "";
    # runall_help.txt is located in the same place as runall, handle both / and \ as path separators
    my $runall_help = substr($0, 0, (((rindex $0, '\\') > (rindex $0, '/')) ? (rindex $0, '\\') : (rindex $0, '/')) + 1 ) . 'runall_help.txt';
	if (-e $runall_help) {
		my $EXTENDEDHELP = get_filehandle("<", "$runall_help");
		while (<$EXTENDEDHELP>) {
			next unless m/#\s+-help:$topic/;
			while (<$EXTENDEDHELP>) {
				next if m/#\s+-help:\w+/;	# Allow multiple keywords for a topic
				last if m/#\s+END OF HELP SECTION/;
				$text .= $_;
			}
		}
	}
	if ($text eq "") {
		print "No extended help available on $topic\n";
	}
	else {
		print $text;
	}
	exit 0;
}

#
# terse_print -- print a single character to the screen
#
# Depends upon $terse_count to know when to end the line.
#
sub terse_print
{
    print $_[0];
    $terse_count++;
    if ($terse_count == 75) {
	print "\n";
	$terse_count = 0;
    }
}


#
# print_result_noise -- print noise information to the results file
#
# To distinguish test result from other noise information in the results file,
# each line of noise information is prepended with a "#" comment character.
#
# Depends on RESULTFILE
#
sub print_result_noise
{
    my $buf = shift;    
    $buf =~ s/^/# /;               # Add a "#" comment character to each line
    $buf =~ s/\n[^(\s*$)]/\n# /g;
    $buf =~ s/(\n#[^\n]*)$/$1\n/;  # Make sure buffer ends with a newline
    write_to_file( \$RESULTFILE,  $buf ) if ($RESULTFILE);
}


#
# print_noise -- print noise information
#
# Depends on $terse_mode, $noise, and $resume_mode
#
sub print_noise
{
    my ($message, $level) = @_;

    if ($noise > $level) {
	print $message unless ($terse_mode);
	# If we're in resume mode, we're still reading from the result file
	print_result_noise( $message) unless $resume_mode;
    }
}

# print_skip/testerror used to be print_skip. I introduced print_testerror
# to handle the test error cases (such as "run.pl contains errors") and to 
# disambiguate them from test skips (such as notarget.lst skips.) I renamed
# both functions to make sure I caught all of the instances of print_skip.
# See VCQA Tests & Tools #5648 for details.

#
# print_skip -- print a message if runall.pl skips a test
# Reasons to skip a test:
# 	$gtags incompatibility (determined by &check_global)
# 	$ltags incompatibility (determined by &check_local)
# 	test not run for target platform (due to notarget.lst)
#
# Depends on $terse_mode, $debug, and $resume_mode.
#
sub print_skip
{
    my ($testname, $globenv, $locenv, $message, $msg_is_result) = @_;
    my ($output);

    $message = ": $message" if defined($message);
    $output = $testname . env_label($globenv,$locenv) . " -- skipped $message\n";
    if (!$terse_mode || $debug) {
		print $output;
    } else {
		terse_print("S");
    }

    # If still in resume mode, we're still reading from the result file,
    # and should not write to it. 
    return if $resume_mode;

    if ($msg_is_result) {
		write_to_file( \$RESULTFILE,  $output );
    }
    else {
		print_result_noise($output);
    }
}

#
# print_testerror -- print a message if runall.pl skips a test
# Reasons to generate a test error:
# 	directory doesn't exist
# 	no run.pl script found
# 	run.pl contains errors
#
# Depends on $terse_mode, $debug, and $resume_mode.
#
sub print_testerror
{
    my ($testname, $globenv, $locenv, $message, $msg_is_result) = @_;
    my ($output);

    $message = ": $message" if defined($message);
    $output = $testname . env_label($globenv,$locenv) . " -- test_error $message\n";
    if (!$terse_mode || $debug) {
		print $output;
    } else {
		terse_print("E");
    }

    # If still in resume mode, we're still reading from the result file,
    # and should not write to it. 
    return if $resume_mode;

    if ($msg_is_result) {
		write_to_file( \$RESULTFILE,  $output );
    }
    else {
		print_result_noise($output);
    }
}

#
# print_test_name -- prints name of the test according to debug/terseness settings
#
# Depends on $debug, $terse_mode
sub print_test_name
{
    my ($test_dir, $label) = @_;
    my $test_name = $test_dir . $label;

	# Note: -debug:list doesn't print to the console! 
	# (but still prints linefeeds--see print_results)
    if ($debug eq 'list') {
	write_to_file( \$RESULTFILE,  $test_dir );
    } else {
	print "$test_name" unless ($terse_mode && !$debug);
	write_to_file( \$RESULTFILE,  $test_name );
    };
}

#
# print_results -- output results to screen, files, etc.
#
# Depends on RESULTFILE, $debug, and $terse_mode.
#
sub print_results
{
    my $retval = shift;
    my $results = shift;
    my @ret_strings = (' -- passed',
    ' -- failed',
    ' -- skipped',
    ' -- cascade',
    ' -- no_result',
    ' -- timed_out',
    ' -- test_error');

    my @terse_char = ('.', 'F', 'S', 'C', 'N', 'T', 'E');
    my ($output);

    if ($debug) {
	$output = "\n";
    } elsif (-s 'results.txt') {
	$output = "$ret_strings[$retval] : " . `type results.txt`;
    } elsif ($results ne "" ) {
	$output = "$ret_strings[$retval] : $results";
    } else {
	$output = "$ret_strings[$retval]";
	if ($timing{'level'} >= $TIMING_LOW)
	{
	    $timing{'delta'} = $timing{'stop'} - $timing{'start'};
	    $output .= ' -- ' . $timing{'delta'} . ' sec ';
	    $output .= "c1($Bt_info{'c1'}) "      if $Bt_info{'c1'}; 
	    $output .= "c1xx($Bt_info{'c1xx'}) "  if $Bt_info{'c1xx'};
	    $output .= "c2($Bt_info{'c2'}) "      if $Bt_info{'c2'}; 
	    $output .= "LD($Bt_info{'ld'}) "      if $Bt_info{'ld'};
	    $output .= "MM($Bt_info{'mm'}) "      if $Bt_info{'mm'};
	    $output .= "OR($Bt_info{'or'}) "      if $Bt_info{'or'};
	    $output .= "OI($Bt_info{'oi'}) "      if $Bt_info{'oi'};
	    $output .= "L1($Bt_info{'l1'}) "      if $Bt_info{'l1'};
	    $output .= "L2($Bt_info{'l2'}) "      if $Bt_info{'l2'}; 
	    $output .= "LF($Bt_info{'lf'}) "      if $Bt_info{'lf'};
	    $output .= "LT($Bt_info{'lt'}) "      if $Bt_info{'lt'}; 
        $output .= "exe($Bt_info{'te'}) "     if $Bt_info{'te'};        
	}
	$output .= "\n";
    }
    if (!$terse_mode || $debug) {
	print $output;
    } else {
	terse_print($terse_char[$retval]);
    }

    write_to_file( \$RESULTFILE,  $output );
}

#
# uniq_env_label
# 	takes in an env.lst permutation for a test and returns the same, uniq'd if necessary
#
sub uniq_env_label($$) {
	my $perm = @_[1];
	$perm =~ s/^\s*\((.*)\)\s*$/\1/;
	my $test = @_[0];
	BEGIN {
		# %_seen records the which 'test_dir (test_case)' perms
		# have already been seen.
		my %_seen = ();
		sub seen { return \%_seen; }
    }

    if (seen()->{"$test-$perm"}) {
		# leave("Testcase: '$test $perm', has already been executed.\n", STATE_ERROR_EXITVAL);
		$xmldups{"$test-$perm"}++;
		# Make some unique permutation which won't persist run-to-run
		$perm .= ' env.lst duplicate ' . ((scalar $xmldups{"$test-$perm"}++)+1)/2;
    }
    else {
        seen()->{"$test-$perm"} = 1;
	}
	$perm ? 
		return ' (' . $perm . ')' :
		return "";
	}

#
# print_results_xml -- output results to .xml logs.
#
# Depends on RESULTFILE, $debug, and $terse_mode.
#
sub print_results_xml
{
    my ($test, $perm, $result, $fail_uuid) = @_;

	# Have unique permutation (from call to uniq_env_label), do this for all tests
	# Problem is, we don't know in multiproc if $xml_previous_test is actually previous
	# test so we do the postprocessing at the end to catch exter close_test_element tags
    if ($xml_previous_test ne $test) {
        close_test_element() if ($xml_previous_test ne "");
	add_test_element($test);
		$xml_previous_test = $test;
    }
    add_test_case_element($perm, $RETVAL_TO_NAMES{$result}, $fail_uuid);
    
}

sub fake_xml_for_skipped_tests($$) 
{
	my $skipped_tests = shift;
	my $test_to_id = shift;
	my %id_to_test = reverse %$test_to_id;	# get id# to test mapping

	# If a test is in the test.lst twice then the id_to_test will look like this: 
	#	20  '7;13'
	#	21  'dumpbin\\TLS'
	# This means the key won't resolve from just the $test. Find these and tag them as test errors.
	my @duplicated_test_ids = grep /;/, keys %id_to_test; 
	my %duplicated_test_names;
	for my $duplicated_test_id (@duplicated_test_ids) {
		my $duplicate_counter = 0;
		for (split /;/, $duplicated_test_id) {
			$duplicated_test_names{$_} = $id_to_test{$duplicated_test_id} . " duplicate test.lst entry " . $duplicate_counter++;
		}
	}
	
	$last_groupID++;
	for my $test (keys %$skipped_tests) {
		if (defined $duplicated_test_names{$test}) {
			my $fail_uuid = gen_uuid();
			add_test_element($duplicated_test_names{$test});
			add_test_case_element("", "test_error", $fail_uuid);
			close_test_element();
			add_test_output_element($duplicated_test_names{$test}, "", "", $fail_uuid);
			add_test_output_text("$$skipped_tests{$test}\n");
			close_test_output_element();
		}
		else {	
			my $fail_uuid = gen_uuid();
			add_test_element($id_to_test{$test});
			# Hack: We didn't carry results in here but the only results are test_error and skipped. And skipped is just for check_target/check_tags so...
			$$skipped_tests{$test} =~ /(?:test not run for)|(?:not running subset)/ ?
				add_test_case_element("", "skipped", $fail_uuid) :
				add_test_case_element("", "test_error", $fail_uuid);
			close_test_element();
			add_test_output_element($id_to_test{$test}, "", "", $fail_uuid);
			add_test_output_text("$$skipped_tests{$test}\n");
			close_test_output_element();
		}
	}
}

#
# print_batch_results_xml -- used in multiproc runall to append result.xml files
#    produced by batched runs into the master results.xml
#
sub print_batch_results_xml
{
    my ($results_file) = @_;

	my $RESULTS_FILE = get_filehandle("<", $results_file);

    write_to_file (\$XMLRESULTFILE, $_) while (<$RESULTS_FILE>);
    
    close $RESULTS_FILE;
}

#
# print_batch_runpl_xml -- used in multiproc runall to append runpl.xml files
#    produced by batched runs into the master runpl.xml
#
sub print_batch_runpl_xml
{
    my ($runpl_file) = @_;

	my $RUNPL_FILE = get_filehandle("<", $runpl_file);

    write_to_file (\$XMLRUNPLFILE, $_) while (<$RUNPL_FILE>);
    
    close $RUNPL_FILE;
}


#
# fix_resultsxml -- postprocessing of results.xml files to remove duplicates and XML errors
# Commented line will change results of duplicate test entries to test_error to get them fixed
#
# Format of results.xml should be
# <?xml
# <TestResults ...
#	<RunInfo>
#		NB: This section does not exist if dumpruninfo.pl doesn't exist!
#	</RunInfo>
# 	<Test id='testdir'
# 		<TestCase case='env label'
# 	</Test>
# </TestResults>
sub fix_resultsxml($$) {
	my ($testName, $testEnv);
	my (%seenTest, %UUID, $closed, $comment);
	$closed = 0;
	$comment = 0;
	
	my $IN = get_filehandle("<", $_[0]);
	my $OUT = get_filehandle(">", $_[1]);
	$_ = <$IN>;	# This should be the <?xml line
	write_to_file (\$OUT, $_);
	$_ = <$IN>;	# This should be the <TestResults line
	if (m/TestRunID='([^']*)'/) {
		$UUID{$1}++;	# First UUID is guaranteed unique
	}
	else {
		write_to_file (\$OUT, "<!-- Expected to see TestResults opening element! -->\n");
	}	
	write_to_file (\$OUT, $_);
	
	while (<$IN> ) { 
		if ($closed) {
			write_to_file (\$OUT, "<!-- TestResults closing element already seen! -->");
		}
		if (m/^<!--/) { 
			$comment = 1; 
		}
		if ($comment) {
			write_to_file (\$OUT, $_);
			if (m/^-->/) { 
				$comment = 0; 
			}
			next;
		}
		# Skip the RunInfo section if it exists
		if (m/^\s*<RunInfo>/) {
			write_to_file (\$OUT, $_);
			while (<$IN>) {
				write_to_file (\$OUT, $_);
				last if m/^\s*<\/RunInfo>/; # last if (m/^s\*<RunInfo>/)
			}
			next;	# next while (<IN>) of enclosing scope
		}
		if (m/^\s*<Test id='([^']*)'/) {
			$testName = $1; 
			write_to_file (\$OUT, $_);
			while (<$IN>) {
				if (m/^<!--/) { 
					$comment = 1; 
				}
				if ($comment) {
					write_to_file (\$OUT, $_);
					if (m/^-->/) { 
						$comment = 0; 
					}
					next;
				}
				if (m/^\s*<\/Test>/) {
					write_to_file (\$OUT, $_);	   
					last;
				}
				else {
					unless (m/^\s*<TestCase case='([^']*)'/) {
						write_to_file (\$OUT, "<!-- Expected TestCase element, found:\n");
						write_to_file (\$OUT, $_);
						write_to_file (\$OUT, "-->\n");
					}
					$testEnv = $1;
					# VCTR is inexplicably case-insensitive
					if ($seenTest{lc $testName}{lc $testEnv}) {
						write_to_file (\$OUT, "<!-- Found duplicate TestCase element:\n");
						write_to_file (\$OUT, $_);
						write_to_file (\$OUT, "-->\n");
						$_ =~ s/case='([^']*)'/case='$1 duplicate $seenTest{lc $testName}{lc $testEnv}'/;
						#$_ =~ s/result='(?:[^']*)'/result='test_error'/;
					}
					write_to_file (\$OUT, $_);
					$seenTest{lc $testName}{lc $testEnv}++;
					if (m/FailureUUID='([^']*)'/) {
						if ($UUID{$1}) {
							write_to_file (\$OUT, "<!-- Non-unique UUID seen: $1 -->\n");
						}
						$UUID{$1}++;
					}
				}
			}
		}
		elsif (m/^\s*<\/TestResults>/) {
			if ($closed) {
				write_to_file (\$OUT, "<!-- TestResults closing element already seen! -->");
			}
			else {
				$closed++;
				write_to_file (\$OUT, $_);
			}
		}
		else {
			write_to_file (\$OUT, "<!-- Expected Test element, found:\n");
			write_to_file (\$OUT, $_);
			write_to_file (\$OUT, "-->\n");
		}
	}
	close $IN;
	close $OUT;
	verify_unlink($_[0]);
}

#
# fix_runplxml -- postprocessing of results.xml files to remove duplicates and XML errors
#
# Format of runpl.xml should be
# <?xml
# <RunPLLog ...
# 	<TestOutput testid='testdir' case='case' ...
#		free text
# 	</Test>
# </RunPLLog>
sub fix_runplxml($$) {
	my ($testName, $testEnv);
	my (%seenTest, $closed, $comment);
	$closed = 0;
	$comment = 0;
	
	my $IN = get_filehandle("<", $_[0]);
	my $OUT = get_filehandle(">", $_[1]);
	$_ = <$IN>;	# This should be the <?xml line
	write_to_file (\$OUT, $_);
	$_ = <$IN>;	# This should be the <RunPLLog line
	unless (m/^<RunPLLog/) {
		write_to_file (\$OUT, "<!-- Expected to see RunPLLog opening element! -->\n");
	}	
	write_to_file (\$OUT, $_);
	
	while (<$IN> ) {
		if ($closed) {
			write_to_file (\$OUT, "<!-- RunPLLog closing element already seen! -->");
		}
		if (m/^<!--/) { 
			$comment = 1; 
		}
		if ($comment) {
			write_to_file (\$OUT, $_);
			if (m/^-->/) { 
				$comment = 0; 
			}
			next;
		}
		if (m/TestOutput\s+testid='([^']*)'\s+case='([^']*)'/) {
			$testName = $1; 
			$testEnv = $2;
			# VCTR is inexplicably case-insensitive
			if ($seenTest{lc $testName}{lc $testEnv}) {
				write_to_file (\$OUT, "<!-- Found duplicate TestCase element:\n");
				write_to_file (\$OUT, $_);
				write_to_file (\$OUT, "-->\n");
				$_ =~ s/case='([^']*)'/case='$1 duplicate $seenTest{lc $testName}{lc $testEnv}'/;
			}
			write_to_file (\$OUT, $_);
			$seenTest{lc $testName}{lc $testEnv}++;
			while (<$IN>) {
				if (m/^<!--/) { 
					$comment = 1; 
				}
				if ($comment) {
					write_to_file (\$OUT, $_);
					if (m/^-->/) { 
						$comment = 0; 
					}
					next;
				}
				if (m/TestOutput\s+testid='([^']*)'\s+case='([^']*)'/) {
					write_to_file (\$OUT, "<!-- No closing TestOutput element seen; adding one on next line. -->\n");
					write_to_file (\$OUT, "</TestOutput>\n");
				}
				write_to_file (\$OUT, $_);
				last if (m/^\s*<\/TestOutput>/);
			}
		}
		elsif (m/^\s*<\/RunPLLog>/) {
			if ($closed) {
				write_to_file (\$OUT, "<!-- RunPLLog closing element already seen! -->");
			}
			else {
				$closed++;
				write_to_file (\$OUT, $_);
			}
		}
		else {
			write_to_file (\$OUT, "<!-- Expected TestOutput element, found:\n");
			write_to_file (\$OUT, $_);
			write_to_file (\$OUT, "-->\n");
		}
	}
	close $IN;
	close $OUT;
	verify_unlink($_[0]);
}

#
# print_fail_header -- print out the failures.(lst|env) file headers
#
# Depends on FAILENV, FAILLST, $fail_env, $fail_list, $prehook, $posthook
#
sub print_fail_header
{
    write_to_file( \$FAILLST,  "# Runall.pl generated failures list\n\n" );
    write_to_file( \$FAILLST,  "# Tests may appears several times in the list, once for each environment\n" );
    write_to_file( \$FAILLST,  "# that failed in that directory.  \"$fail_env\" contains all environment\n" );
    write_to_file( \$FAILLST,  "# information for the failures, so local environment files are ignored.\n\n" );
    write_to_file( \$FAILLST,  "RUNALL_COMMAND -global \"$fail_env\" -local:no\n" );
    write_to_file( \$FAILLST,  "RUNALL_COMMAND -tags: -nottags: -noise:0\n" );
    write_to_file( \$FAILLST,  "RUNALL_COMMAND -target:$target\n" );
    if (@prehook) {
		foreach my $hook (@prehook) {
			write_to_file( \$FAILLST,  "RUNALL_COMMAND -prehook \"$hook\"\n" );
		}
    }
    if (@posthook) {
		foreach my $hook (@posthook) {
			write_to_file( \$FAILLST,  "RUNALL_COMMAND -posthook \"$hook\"\n" );
		}
    }
    write_to_file( \$FAILLST,  "\n" );

    write_to_file( \$FAILENV,  "# Runall.pl generated global environment failure list\n\n" );
    write_to_file( \$FAILENV,  "# Each line here corresponds one-to-one with a line in \"$fail_list\".\n" );
    write_to_file( \$FAILENV,  "# The tags will match them up, so each global environment only runs\n" );
    write_to_file( \$FAILENV,  "# the test that it is matched with.\n\n" );
}


#
# print_fail_info -- keep track of failures during test run
#
# Print out a repro batch file and information to failures.(env|lst); the tag
# in the latter is a unique identifier for this failure, and the label is the
# combined environment labels.  Each line in failures.env corresponds to
# exactly one line in failures.lst; this means that a directory might show up
# several times in the test list, but it doesn't matter since each one is tied
# to just one environment.
#
# Depends on FAILENV, FAILLST, RUNPLFILE, $genrepro, $found_runpl, and @totals_count.
#
sub print_fail_info
{
    my ($test, $retval, $globenv, $locenv) = @_;
    my ($tag, $label, %combo, $val, $var);
    my %namehash = ( 1 => 'fail', 3 => 'casc', 5 => 'time' );

    $tag = sprintf("%s%03d",$namehash{$retval},$totals_count[$retval]);
    $label = env_label($globenv->[3], $locenv->[3]);

    write_to_file( \$FAILLST,  "$tag\t\t$test\t# $label\n" );

    # Have to expand local environment variables based upon the global
    # environment, to simulate what happens during the test run
    %combo = %{$globenv->[4]};
    for $var (keys(%{$locenv->[4]})) {
	$combo{$var} = expand($locenv->[4]{$var}, $globenv->[4]);
    }
    write_to_file( \$FAILENV,  "*\t$tag\t" );
    for $var (keys(%combo)) {
	$val = $combo{$var};
	$val =~ s/"/\\"/g;
	$val =~ s/\\/\\\\/g;
	write_to_file( \$FAILENV,  "$var=\"$val\" " );
    }
    $label =~ s/^(.*?)\((.*)\)/$1$2/;
    write_to_file( \$FAILENV,  " \t# $label" );
    write_to_file( \$FAILENV,  "\n" );

    create_repro($test, $retval, $globenv, $locenv);	

}


sub create_repro
{
    return if ($genrepro eq 'no');

	my ($test, $retval, $globenv, $locenv) = @_;
	my($label, $repro, $backref, $val);
	$label = env_label($globenv->[3], $locenv->[3]);
	$label =~ s/^(.*?)\((.*)\)/$1$2/;

	$repro = "repro$globenv->[0]_$locenv->[0].bat";

	write_to_file( \$RUNPLFILE,  "REPRO $test ($label): $repro\n" );
	
	chmod 0777, $repro if (! -w $repro);
	open(BATCH, ">$repro");
	print BATCH "setlocal\n";
	print BATCH "\@REM Runall.pl generated repro file\n";

	print BATCH "set TARGET_ARCHITECTURE=$ENV{TARGET_ARCHITECTURE}\n";
	print BATCH "set TARGET_NATIVE_ARCHITECTURE=$ENV{TARGET_NATIVE_ARCHITECTURE}\n";
	print BATCH "set TARGET_IS_MANAGED=$ENV{TARGET_IS_MANAGED}\n";

	if ($globenv->[3] ne '') {
	    print BATCH "\@REM Global environment \"$globenv->[3]\"\n";
	    for (keys(%{$globenv->[4]})) {
			$val = $globenv->[4]{$_};
			$val =~ s/%/%%/g;
			print BATCH "set $_=$val\n";
	    }
	} else {
	    print BATCH "\@REM No global environment set";
	}

	if ($locenv->[3] ne '') {
	    print BATCH "\n\@REM Local environment \"$locenv->[3]\"\n";
	    for (keys(%{$locenv->[4]})) {
			$val = $locenv->[4]{$_};
			$val =~ s/%/%%/g;
			print BATCH "set $_=$val\n";
	    }
	} else {
	    print BATCH "\n\@REM No local environment set\n";
	}

	if (defined($found_runpl)) {
		# REVIEW: This should be ok for run.exe as long as run.exe doesn't start with a \
	    $backref = substr(cwd() . '\\',length($found_runpl)-6);
	    $backref =~ s/[^\\]+\\/..\\/g;
	}

	print BATCH "\nattrib +r repro*.bat\nattrib +r %0*\n";
 	print BATCH "echo \"Select one of the next two commands:\"\n";
 	print BATCH "'   ${backref}run.exe\n";
 	print BATCH "    perl ${backref}run.pl\n";
	print BATCH "attrib -r repro*.bat\nattrib -r %0*\n";
	print BATCH "endlocal\n";
	close(BATCH);
}

sub dump_run_info() {
	return if $isWin9x; # VCQA: 6953 dumpruninfo has problems if we're on Windows 9x
    # DumpRunInfo.pl is located in the same place as runall, handle both / and \ as path separators
    my $dumpruninfo = substr($0, 0, (((rindex $0, '\\') > (rindex $0, '/')) ? (rindex $0, '\\') : (rindex $0, '/')) + 1 ) . 'dumpruninfo.pl';
    open D, "<$dumpruninfo" or return;
    local $/;
    my $sub = <D>;
    close D;
	my $output = eval $sub;
    write_to_file( \$RUNPLFILE,  $output );
	if ($xml) {
    	write_to_file( \$XMLRESULTFILE,  "\t<RunInfo>\n" );
		text2xml(\$output);
    	write_to_file( \$XMLRESULTFILE, $output );
    	write_to_file( \$XMLRESULTFILE,  "\t</RunInfo>\n" );
	}
}

#
# print_fail_xml -- keep track of failures during test run and logs them to results.xml
#
sub print_fail_xml
{
	my ($test, $globenv, $locenv, $label, $errtext, $fail_uuid) = @_;
    my ($failenv, %combo, $val, $var);

    # Have to expand local environment variables based upon the global
    # environment, to simulate what happens during the test run
    %combo = %{$globenv->[4]};
    for $var (keys(%{$locenv->[4]})) {
	$combo{$var} = expand($locenv->[4]{$var}, $globenv->[4]);
    }

    for $var (keys(%combo)) {
	$val = $combo{$var};
	$val =~ s/"/\\"/g;
	$failenv .= "$var=\"$val\" ";
    }

    add_test_output_element($test, $label, $failenv, $fail_uuid);
    add_test_output_text($errtext);
    close_test_output_element();
}

#########################
# XML PRINTING ROUTINES #
#########################

#
# AddTestResultsElement
#
sub add_test_results_element
{
    my ($suite, $uuid) = @_;
    my (
	$cl_info, $host_winver, $target_winver,
	$host_lang, $target_lang, $runtimeenv
       );

    $cl_info = get_clver();
    get_os(\$host_winver, \$target_winver);
    get_lang(\$host_lang, \$target_lang);
    $runtimeenv = ($ENV{TARGET_IS_MANAGED}) ? "IJW" : "native";
    $runtimeenv = "pure" if (lc($target) eq "clrpure");
	
    write_to_file( \$XMLRESULTFILE,  "<?xml version='1.0' encoding='utf-8' ?>\n" );
    write_to_file( \$XMLRESULTFILE,  "<TestResults " );
    write_to_file( \$XMLRESULTFILE,  "TestRunID='$uuid' " );
    write_to_file( \$XMLRESULTFILE,  "TestSuite='$suite' " );
    write_to_file( \$XMLRESULTFILE,  "TargetArchitecture='$cl_info->{targetarch}' " );
    write_to_file( \$XMLRESULTFILE,  "HostArchitecture='$cl_info->{hostarch}' " );
    write_to_file( \$XMLRESULTFILE,  "TargetProcessor='$ENV{TARGET_NATIVE_ARCHITECTURE}' " );
    write_to_file( \$XMLRESULTFILE,  "HostProcessor='$ENV{PROCESSOR_ARCHITECTURE}' " );
    write_to_file( \$XMLRESULTFILE,  "HostRuntimeEnvironment='native' " );
    write_to_file( \$XMLRESULTFILE,  "TargetRuntimeEnvironment='$runtimeenv' " );
    if (defined $xmlproduct) {
		write_to_file( \$XMLRESULTFILE,  "Product='$xmlproduct' " );
	}
	else {
		write_to_file( \$XMLRESULTFILE,  "Product='$cl_info->{vernum}' " );
	}
    write_to_file( \$XMLRESULTFILE,  "Language='$target_lang' " );
    write_to_file( \$XMLRESULTFILE,  "HostLanguage='$host_lang' " );
    write_to_file( \$XMLRESULTFILE,  "BuildNumber='$cl_info->{buildnum}' " );
    write_to_file( \$XMLRESULTFILE,  "BuildConfiguration='$xmlbldcfg' " ) if (defined $xmlbldcfg);
    write_to_file( \$XMLRESULTFILE,  "ChangeList='$xmlchangelist' " ) if (defined $xmlchangelist);
    write_to_file( \$XMLRESULTFILE,  "TestRunOwner='$xmlownername' " ) if (defined $xmlownername);
    write_to_file( \$XMLRESULTFILE,  "TestRunBugDatabase='$xmltestrunbugdb' " ) if (defined $xmltestrunbugdb);
    write_to_file( \$XMLRESULTFILE,  "TestRunBugID='$xmltestrunbugid' " ) if (defined $xmltestrunbugid);
    write_to_file( \$XMLRESULTFILE,  "ToolName='$xmltoolname' " ) if (defined $xmltoolname);
    write_to_file( \$XMLRESULTFILE,  "OperatingSystem='$target_winver' " );
    write_to_file( \$XMLRESULTFILE,  "HostOperatingSystem='$host_winver'" );
    write_to_file( \$XMLRESULTFILE,  ">\n" );
}

#
# AddTestElement
#
sub add_test_element
{
    my ($id) = @_;

    write_to_file( \$XMLRESULTFILE,  "\t<Test id='$id'>\n" );
}

#
# AddTestCaseElement
#
sub add_test_case_element
{
    my ($perm, $result, $fail_uuid) = @_;

    $perm =~ s/\s+//; #strip white space
    $perm =~ s/^\((.*)\)$/$1/; #strip parens
    text2xml(\$perm);
    write_to_file( \$XMLRESULTFILE,  "\t\t<TestCase case='$perm' result='$result'" );
    write_to_file( \$XMLRESULTFILE,  " FailureUUID='$fail_uuid'" ) if defined $fail_uuid;
    write_to_file( \$XMLRESULTFILE,  "/>\n" );
}

#
# AddRunPLLogElement
#
sub add_runpl_log_element
{
    my ($suite, $uuid) = @_;

    write_to_file( \$XMLRUNPLFILE,  "<?xml version='1.0' encoding='utf-8' ?>\n" );
    write_to_file( \$XMLRUNPLFILE,  "<RunPLLog TestRunID='$uuid' TestSuite='$suite'>\n" );
}

#
# AddTestOutputElement
#
sub add_test_output_element
{
	my ($path, $perm, $env, $fail_uuid) = @_;

    $perm =~ s/\s+//; #strip white space
    $perm =~ s/\((.+)\)/$1/; #strip parens
    text2xml(\$perm);
    text2xml(\$path);
    text2xml(\$env);
    write_to_file( \$XMLRUNPLFILE,  "\t<TestOutput " );
    write_to_file( \$XMLRUNPLFILE,  "testid='$path' " );
    write_to_file( \$XMLRUNPLFILE,  "case='$perm' " );
    write_to_file( \$XMLRUNPLFILE,  "environment='$env' " );
    write_to_file( \$XMLRUNPLFILE,  "FailureUUID='$fail_uuid' " );
    write_to_file( \$XMLRUNPLFILE,  "GroupID='$last_groupID'" );
    write_to_file( \$XMLRUNPLFILE,  ">\n" );
}

#
# AddTestOutputText
#
sub add_test_output_text
{
    my ($text) = @_;

    $text =~ s#\n(?!$)#\n\t\t#g;
    text2xml(\$text);
    $text .= "\n" unless ($text =~ m/\n$/);
    write_to_file( \$XMLRUNPLFILE,  "\t\t$text" );
}

#
# CloseRunPLLog
#
sub close_runpl_log_element
{
    write_to_file( \$XMLRUNPLFILE,  "</RunPLLog>\n" );
}

#
# CloseTestOutput
#
sub close_test_output_element
{
  write_to_file( \$XMLRUNPLFILE,  "\t</TestOutput>\n" );
}

#
# CloseTestElement
#
sub close_test_element
{
    write_to_file( \$XMLRESULTFILE,  "\t</Test>\n" );
}

#
# CloseTestResultsElement
#
sub close_test_results_element
{
    write_to_file( \$XMLRESULTFILE,  "</TestResults>\n" );
}


###############################
# FILE INPUT/PARSING ROUTINES #
###############################


#
# parse_string -- split lines into tokens
#
# All the rigamarole in the third switch is necessary to handle quotation
# marks that can occur at any point.  This can also parse environment
# files. RUNALL_DEFAULT and _ra_noenv are simply treated as no-opts.  Tags in
# environment files are returned as a single token at the end of the list.
#
# The complicated-looking regular expression simply means:  "a string with
# two non-escaped double quotes."
#
sub parse_string
{
    my ($origstr, $delim) = @_;
    my (@lst, $temp, $start, $end, @result);

    @result = ();
    $delim = ' ' unless defined($delim);
    @lst = split(/$delim/,$origstr);
    while (@lst > 0) {
	if ($lst[0] =~ /^(RUNALL_DEFAULT|_ra_noenv)$/i) {
	    shift(@lst);
	} elsif ($lst[0] =~ /^#/) {
	    @result = (@result,join($delim, @lst));
	    last;
	} elsif ($lst[0] =~ /(^|[^\\])(\\\\)*"/) {
	    $temp = shift(@lst);
	    while ((@lst > 0) && ($temp !~ /(^|[^\\])(\\\\)*"(|.*?[^\\])(\\\\)*"/)) {
		$temp = "$temp$delim" . shift(@lst);
	    }
	    $temp =~ s/((^|[^\\])(\\\\)*)"((|.*?[^\\])(\\\\)*)("|$)/$1$4/;
	    #stick back into @lst, in case there are more quotes
	    @lst = ($temp, @lst);
	} else {
	    if ($lst[0] ne '') {
		# resolve escaped characters
		$lst[0] = eval "\"$lst[0]\"";
		@result = (@result, $lst[0]);
	    }
	    shift(@lst);
	}
    }
    return(@result);
}


#
# parse_switches -- process command line arguments
#
# Input is an array of switches that have already been split via
# parse_string into individual strings.
#
# Depends on a whole mess of global variables.
#
sub parse_switches
{
    my ($sw, $filename, $op, $num);

    while (@_ > 0) {
	$sw = shift;
	if ($sw !~ /^[-\/]/) {
	    leave("Runall.pl line " . __LINE__ . ":  Unexpected parameter $sw\n", $SWITCH_ERROR_EXITVAL);

	} elsif ($sw =~ s/^[-\/]cleanup://i) {
	    if ($sw =~ /^(yes|no|default)$/) {
		$cleanup = $sw;
	    } else {
		leave("Runall.pl line " . __LINE__ . ":  Bad -cleanup switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }

	} elsif ($sw =~ s/^[-\/]debug://i) {
	    if ( $sw eq 'off'){
                $debug = 0;
            } else {
                $debug = $sw;
                if ( $filelock > 0 ) {
                    leave("Runall.pl line " . __LINE__ . ":  Bad -debug switch value: '$sw'\n\t (-filelock switch is not 0 so -debug:$sw cannot be used\n", $SWITCH_ERROR_EXITVAL);
                } else {
                    $filelock = 0;
                }
            }
	    if ($sw =~ /^(off|clean)$/) {
		$debug_retval = 0;
	    } elsif ($sw =~ /^(list|on)$/) {
		$debug_retval = 4;
	    } elsif ($sw eq 'fail') {
		$debug_retval = 1;
	    } else {
		leave("Runall.pl line " . __LINE__ . ":  Bad -debug switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }

	} elsif ($sw =~ /^[-\/](env|global)$/i) {
	    $global_env_file = shift;
	    if ($global_env_file =~ /(^|\\)$local_env_file$/) {
		leave("Runall.pl line " . __LINE__ . ":  Global env file cannot be '$local_env_file'\n", $SWITCH_ERROR_EXITVAL);
	    }

	} elsif ($sw =~ /^[-\/]fail(|ures)$/i) {
	    $sw = shift;
	    $fail_list = "$sw.lst";
	    $fail_env = "$sw.env";

	} elsif ($sw =~ /^[-\/](\?|help)$/i) {
	    print_help();

	} elsif ($sw =~ s/^[-\/]help://i) {
	    print_extended_help($sw);

	} elsif ($sw =~ /^[-\/]keep$/i) {
	    push @keepfiles, shift;

	} elsif ($sw =~ s/^[-\/]knownfail$//i) {
		$knownfail = shift;

	} elsif ($sw =~ /^[-\/]log$/i) {
	    $runpl_log = shift;
	    $xmlrunpl_file = "$runpl_log.xml";

	} elsif ($sw =~ s/^[-\/]maxfails://i) {
	    if ($sw =~ /^\d+$/) {
		$maxfails = $sw;
	    } else {
		leave("Runall.pl line " . __LINE__ . ":  Bad -maxfails switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }

	} elsif ($sw =~ s/^[-\/]maxtests://i) {
	    if ($sw =~ /^\d+$/) {
		$maxtests = $sw;
	    } else {
		leave("Runall.pl line " . __LINE__ . ":  Bad -maxtests switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }

	} elsif ($sw =~ s/^[-\/]resultsroot$//i) {
		$results_root = shift;

	} elsif ($sw =~ s/^[-\/]savelog://i) {
	    if ($sw =~ /^(all|pass|fail|none)$/) {
		$savelog = $sw;
	    } else {
		leave("Runall.pl line " . __LINE__ . ":  Bad -cleanup switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }

	} elsif ($sw =~ s/^[-\/]local://i) {
	    if ($sw =~ /^(yes|no)$/) {
		$local_env_file = ($sw eq 'no') ? undef : 'env.lst';
	    } else {
		leave("Runall.pl line " . __LINE__ . ":  Bad -local switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }

	} elsif ($sw =~ s/^[-\/]noise://i) {
	    if ($sw =~ /^\d+$/) {
		$noise = $sw;
	    } else {
		leave("Runall.pl line " . __LINE__ . ":  Bad -noise switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }
	} elsif ($sw =~ s/^[-\/](child|silent)//i) {

	    $multiproc_child = 1;

	    open STDOUT, ">NUL" or leave("Runall.pl line " . __LINE__ . ": Can't redirect stdout to NUL for -child\n", $OTHER_ERROR_EXITVAL);
	    open STDERR, ">NUL" or leave("Runall.pl line " . __LINE__ . ": Can't dup stdout\n", $OTHER_ERROR_EXITVAL);

	    select STDERR; $| = 1;
	    select STDOUT; $| = 1;

	} elsif ($sw =~ s/^[-\/]target://i) {
	    if ($sw =~ /^\w+$/) {
		$target = lc($sw); # VCQA#2226
		$ENV{TARGET_ARCHITECTURE} = $target;
	    } else {
		leave("Runall.pl line " . __LINE__ . ":  Bad -target switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }

	} elsif ($sw =~ s/^[-\/]target_arch://i) {
	    if ($sw =~ /^\w+$/) {
		$target_arch = lc($sw);
		if ( $target_arch =~ "x86" || 
		     $target_arch =~ "amd64" ||
		     $target_arch =~ "ia64") {
		}
		else {
		    leave("Runall.pl line " . __LINE__ . ":  Bad -target_arch switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
		}
	    } else {
		leave("Runall.pl line " . __LINE__ . ":  Bad -target_arch switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }

	} elsif ($sw =~ /^[-\/]prehook$/i) {
		$sw = shift;
	    if ($sw !~ /^([A-Z]:)?\\/i) {
			$sw = "$root\\$sw";
	    }
	    push @prehook, $sw;

	} elsif ($sw =~ /^[-\/]posthook$/i) {
		$sw = shift;
	    if ($sw !~ /^([A-Z]:)?\\/i) {
			$sw = "$root\\$sw";
	    }
	    push @posthook, $sw;

	} elsif ($sw =~ s/^[-\/]repro://i) {
	    if ($sw =~ /^(yes|no|all)$/) {
	    $genrepro = $sw;
	    }
	    else {
		leave("Runall.pl line " . __LINE__ . ":  Bad -repro switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }

	} elsif ($sw =~ /^[-\/]results$/i) {
	    $result_file = shift;
            $xmlresult_file = "$result_file.xml";

	} elsif ($sw =~ s/^[-\/]resume://i) {
	    if ($sw =~ /^(yes|no)$/) {
		$resume_mode = ($sw eq 'no') ? 0 : 1;
	    } else {
		leave("Runall.pl line " . __LINE__ . ":  Bad -resume switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }

	} elsif ($sw =~ /^[-\/]test$/i) {
	    $test_file = shift;
		
	} elsif ($sw =~ s/^[-\/]env://i) {
	    $permutation = $sw;
		$permutation =~ s/^\s*\"(.*)\"\s*$/\1/;
		$permutation =~ s/^\s*\((.*)\)\s*$/\1/;

	} elsif ($sw =~ /^[-\/]terse$/i) {
	    $terse_mode = 1;

	} elsif ($sw =~ s/^[-\/]timeout://i) {
		if ($isWin9x) {
			print_noise("WARNING: -timeout not available on Win9x; setting is ignored\n");
		}
	    if ($sw =~ /^\d+$/) {
                if ( $sw > 0 ){
                    $timeout = $sw * 60; # timeout is in seconds for stopit.exe
                } else {
                    print_noise("WARNING: -timeout not a valid value $sw; setting is ignored\n");
                }
	    } else {
			print_noise("WARNING: -timeout not a valid value $sw; setting is ignored\n");
	    }       

	} elsif ($sw =~ s/^[-\/](g|t|l|)tags://i) {
	    if ($1 eq '') {
		$gtags = $sw ? union($gtags,$sw) : '';
		$ttags = $sw ? union($ttags,$sw) : '';
		$ltags = $sw ? union($ltags,$sw) : '';
	    } elsif ($1 eq 'g') {
		$gtags = $sw ? union($gtags,$sw) : '';
	    } elsif ($1 eq 't') {
		$ttags = $sw ? union($ttags,$sw) : '';
	    } elsif ($1 eq 'l') {
		$ltags = $sw ? union($ltags,$sw) : '';
	    }

	} elsif ($sw =~ s/^[-\/]nottags://i) {
	    $nottags = $sw ? union($nottags,$sw) : '';

	} elsif ($sw =~ s/^[-\/]usage://i) {
	    $usage_filter = $sw ? union($usage_filter,$sw) : '*';

	} elsif ($sw =~ s/^[-\/]notusage://i) {
	    $usage_neg_filter = $sw;

	} elsif ($sw =~ s/^[-\/]nodefarch//i) {
	    $defarch_file = "";

	} elsif ($sw =~ s/^[-\/]procs:(\d+)//i) {
	    $nProcs = $1;

	} elsif ($sw =~ s/^[-\/]mpdebug:(\d+)//i) {
	    $mpdebug = $1;

	} elsif ($sw =~ s/^[-\/]batch:(\d+)//i) {
	    $cBatchMax = $1;

	} elsif ($sw =~ s/^[-\/]fileprefix://i) {
	    $file_prefix = $sw;
		
	} elsif ($sw =~ /^[-\/]port:(\d+)/i) {
	    $compilerServerPort = $1;
	} elsif ($sw =~ s/^[-\/]timing://i) {
	    TIMING_SWITCH:
	    {
		my $tl;
		foreach $tl (keys %TIMING_NAMES_TO_VALS) {
		    $sw =~ /^$tl/ and $timing{'level'} = $TIMING_NAMES_TO_VALS{$tl}, last TIMING_SWITCH;
		};
		'DEFAULT'    and leave("Runall.pl line " . __LINE__ . ":  Bad -timing switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }
	} elsif ($sw =~ s/^[-\/]xml://i) {
            if ($sw =~ /^(yes|no)$/) {
		$xml = ($sw eq 'no') ? 0 : 1;
	    } else {
			leave("Runall.pl line " . __LINE__ . ":  Bad -resume switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }
	} elsif ($sw =~ s/^[-\/]xmlproduct://i) {
		 # xmlproduct switch can only use valid values from VCTR
	     if ($sw =~ /^[\.\w]+$/ && (grep m/$sw/, qw(v3ET v4ET sys v6 v6.sp6 v6.sp5 v6.qfe v6.2 v7 v7.1 v8.0 Phoenix))) {
		 $xmlproduct = $sw;
	    } else {
			leave("Runall.pl line " . __LINE__ . ":  Bad -xmlproduct switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }
	} elsif ($sw =~ s/^[-\/]xmlbldcfg://i) {
	     if ($sw =~ /^\w+$/) {
		 # VCTR only allows a 16-character xmlbuildcfg
		 $xmlbldcfg = substr($sw, 0, 16);
		 if (length $sw > 16) {
			print "-xmlbldcfg too long, truncated to 16 chars: '$xmlbldcfg'\n";
		 }
	    } else {
			leave("Runall.pl line " . __LINE__ . ":  Bad -xmlbldcfg switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }
	# We used to accept 'suitename' but want to change to 'xmlsuitename'
	} elsif ($sw =~ s/^[-\/](xml)?suitename://i) {
	     if ($sw =~ /^\w+$/) {
		 $xmlsuitename = $sw;
	    } else {
			leave("Runall.pl line " . __LINE__ . ":  Bad -xmlsuitename switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }
	} elsif ($sw =~ s/^[-\/]xmlownername://i) {
	     if ($sw =~ /^\w+\\\w+$/) {
		 $xmlownername = $sw;
	    } else {
			leave("Runall.pl line " . __LINE__ . ":  Bad -xmlownername switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }
	} elsif ($sw =~ s/^[-\/]xmltestrunbugdb://i) {
	     if ($sw =~ /^[ \w]+$/ && (grep m/$sw/, ('VCQA Test Runs'))) {
		 $xmltestrunbugdb = $sw;
	    } else {
			leave("Runall.pl line " . __LINE__ . ":  Bad -xmltestrunbugdb switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }
	} elsif ($sw =~ s/^[-\/]xmltestrunbugid:(\d+)//i) {
	    $xmltestrunbugid = $1;
	} elsif ($sw =~ s/^[-\/]xmlchangelist:(\d+)//i) {
	    $xmlchangelist = $1;
	} elsif ($sw =~ s/^[-\/]xmltoolname://i) {
	     if ($sw =~ /^\w+$/ && (grep m/$sw/, qw(phxc2 phxjit phxprejit phxprejitc2 phxpereader))) {
		 $xmltoolname = $sw;
	    } else {
			leave("Runall.pl line " . __LINE__ . ":  Bad -xmltoolname switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }
	} elsif ($sw =~ s/^[-\/]filelock://i) {
	    if ($sw =~ /^\d+$/ && $sw < 8 && $sw > -1) {
		if ("$debug" ne "0" && $sw > 0){
		    leave("Runall.pl line " . __LINE__ . ":  Bad -filelock switch value: '$sw'\n\t (-debug switch was specified and filelock must be set to 0\n", $SWITCH_ERROR_EXITVAL);
		} else {
		    $filelock = $sw;
		}
	    } else {
		leave("Runall.pl line " . __LINE__ . ":  Bad -filelock switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }
	} elsif ($sw =~ s/^[-\/]bzerror://i) {
        if ($sw =~ /^(yes|no)$/) {
		$relaxerrors = ($sw eq 'no') ? 0 : 1;
	    } else {
			leave("Runall.pl line " . __LINE__ . ":  Bad -bzerror switch value: '$sw'\n", $SWITCH_ERROR_EXITVAL);
	    }
	} else {
	    leave("Runall.pl line " . __LINE__ . ":  Unknown switch '$sw'\n", $SWITCH_ERROR_EXITVAL);
	}
    }
    if ( $filelock == -1 ) {
        $filelock = 1; # by default abort the run when a file lock is detected
    }
}


#
# get_switches -- build up user preferences
#
# The environment variable RUNALL is checked first, so that the values
# it holds can be overridden on the command line.
#
sub get_switches
{
    # parse RUNALL environment variable, if it exists
    if ($ENV{'RUNALL'} ne '') {
	# VCQA#3098: expand escape characters before parse_string
	my $runall_env = $ENV{'RUNALL'};
	$runall_env =~ s/\\/\\\\/g;
	parse_switches(parse_string($runall_env));
    }

    # parse command line switches
    parse_switches(@ARGV);

    # parse default architecture file switches
    my $defarch_switches = get_defarch_file();
    if ($defarch_switches ne "") {
		my @defarch_switches = split(/\s/, $defarch_switches);
		parse_switches(@defarch_switches);
    };

    $gtags = '__ra_alltag__' unless $gtags;
    $ttags = '__ra_alltag__' unless $ttags;
    $ltags = '__ra_alltag__' unless $ltags;

    # set TARGET_MANAGED env variable if we're targeting the CLR
    $ENV{TARGET_IS_MANAGED} = 0;
    $ENV{TARGET_IS_MANAGED} = 1 if (($target eq "cee") or ($target =~ /clr$/) or ($target eq "clrpure"));

	# get TARGET_NATIVE_ARCHITECTURE from the get_clver function
	$ENV{TARGET_NATIVE_ARCHITECTURE} = get_clver()->{targetarch};
	# fallback: check environment variables in case compiler isn't found
	if ($ENV{TARGET_NATIVE_ARCHITECTURE} eq "Unknown") {
		my $tna = `$ENV{SIMULATOR_PIPE} cmd /c set PROCESSOR_ARCHITEW6432 2>&1`;
		if ($tna =~ m/^PROCESSOR_ARCHITEW6432\=([A-Z0-9]+)\s*$/i) {
			$ENV{TARGET_NATIVE_ARCHITECTURE} = $1;
		}
		else {
			$tna = `$ENV{SIMULATOR_PIPE} cmd /c set PROCESSOR_ARCHITECTURE 2>&1`;
			if ($tna =~ m/^PROCESSOR_ARCHITECTURE\=([A-Z0-9]+)\s*$/i) {
				$ENV{TARGET_NATIVE_ARCHITECTURE} = $1;
			}
		}
	}
}

#
# read_list_file -- read in a file containing a list of values, discarding
# blanks and comments
#
# Returns the empty list if the file does not exist or if the file contains
# no valid items.
#
sub read_list_file
{
    my $filename = shift;
    my @elements = ();
    my $trimmed;

    if (not -e $filename) {
	return @elements;
    }    

	my $LISTFILE = get_filehandle("<", $filename);

    while (<$LISTFILE>) {
	# Tokenize a text buffer.  Don't break apart quoted strings.  
	# Comments start with a pound mark (#) outside of quotes and continue
	# to the end of line.  Adapted from perlfaq4.
	for(m{
	    \#.+$                         # a comment
	    | ([^"#\s]+)                    # an unquoted filename
	    | "([^\"\\]*(?:\\.[^\"\\]*)*)"  # groups filename inside quotes
	}gx) {

	    # Strip leading/trailing whitespace
	    $trimmed = (trim($_))[0];          
	    push(@elements, $_) unless not length($trimmed);
	}
    }

    close $LISTFILE;
    
    return @elements;
}

#
# read_file -- read in a file, discarding blanks and comments
#
# "Blank" lines contain zero or more ws chars, and nothing else; "comment"
# lines contain a "#" as the first non-ws char on the line.  Return value
# is an array containing the remaining lines of the file.  The $level
# parameter is used to create a unique file handle at each level of recursion,
# if RUNALL_INCLUDE is used.
#
# get_filehandle doesn't handle this level of complexity (RFH$level) so we use
# the standard "open" in this function without any extra failure handling
#
sub read_file
{
    my ($filename, $isTest, $level) = @_;
    my $fh = "RFH$level";
    my (@filearray, $cwd, $aref, @fields, $tag, $startdir);

    $startdir = cwd();
    if ($filename =~ /\\([^\\]+)$/) {
	chdir("$`\\");
	$filename = $1;
    }
    print_noise("\tReading file $filename\n",2);
    no strict 'refs';
    unless (open($fh,$filename)) {
		leave("Runall.pl line " . __LINE__ . ":  Fatal Error: $!\nCould not open file '$filename'\n", $FILE_ERROR_EXITVAL);
    }
    $cwd = cwd();
    while (<$fh>) {
	s/\s+$//; # This is a chomp but will also chomp \t, \s, etc. VCQA:3150
	if (s/^RUNALL_INCLUDE\s+//i) {
	    if (-e $_) {
			push(@filearray,read_file($_, $isTest, $level+1));
	    } else {
			leave("Runall.pl line " . __LINE__ . ":  Fatal error!\nCannot find included file '$_'\n", $FILE_ERROR_EXITVAL);
	    }

	} elsif (/^RUNALL_COMMAND\s+/i) {
		# If test.lst contains -global and -local:no we assume it's a failures.lst
		$_ = expand($');
		m/-global/ and m/-local:no/ and $running_failures=1;
	    parse_switches(parse_string($_));

	} elsif (/^\s*[^\s#]/) {
	    $aref = ['__ra_alltag__','*'];
	    @fields = split(/\t+/, $_, 4);
	    # If non-whitespace exists after '#' comment, use it as a label
	    if ($fields[$#fields] =~ /^\s*#\s*/) {
		$#fields--;
		$aref->[3] = $' if length($');	       
	    }
	    if (@fields == 3) {
		$aref->[0] = "__ra_alltag__," . join(",", trim(split(/,/, $fields[0])));
		$aref->[1] = join(",", trim(split(/,/, $fields[1])));
		$aref->[2] = $fields[2];
	    } elsif (@fields == 2) {
		$aref->[0] = "__ra_alltag__," . join(",", trim(split(/,/, $fields[0])));
		$aref->[2] = $fields[1];
	    } elsif (@fields == 1) {
		$aref->[2] = $fields[0];
	    } else {
		leave("Runall.pl line " . __LINE__ . ":  Parsing error in $startdir\\$filename, with '$_'\n", $INPUT_ERROR_EXITVAL);
	    }
	    # reject invalid characters in tags
	    if ($aref->[0] =~ /\s/) {
		leave("Runall.pl line " . __LINE__ . ":  Parsing error in $startdir\\$filename, with '$_': invalid space character in tags\n", $INPUT_ERROR_EXITVAL);
	    }
	    if ($aref->[1] =~ /\s/) {
		leave("Runall.pl line " . __LINE__ . ":  Parsing error in $startdir\\$filename, with '$_': invalid space character in usage\n", $INPUT_ERROR_EXITVAL);
	    }
	    # breaks fe_mcia64sniff test in Gauntlet
	    #if ($isTest && ($aref->[2] =~ /\s/)) {
	    #leave("Runall.pl line " . __LINE__ . ":  Parsing error in $startdir\\$filename, with '$_': invalid space character in test directory\n", $INPUT_ERROR_EXITVAL);
	    #}
	    # fix up test list fields
	    if ($isTest) {
		if ($aref->[1] eq '*') {
		    $aref->[1] = '__ra_nomatch__';
		}
		if ($cwd ne $root) {

		    #
		    # If we're not in the root, we assume that $aref->[2] is
		    # a pathname relative to cwd and attempt to turn it
		    # into a pathname relative to root.
		    # 
		    # If this fails (i.e. if $cwd is not a child of root), we
		    # assume that $aref[2] is already a specified relative to 
		    # root. This enables -test ..\..\foo.lst to work
		    #
		    my $fullpathname = "$cwd\\$aref->[2]";
		    if ( lc(substr($fullpathname, 0, length($root) )) eq lc($root) ) {
			$aref->[2] = substr($fullpathname,length($root)+1);
		    };
		}
	    }
	    push(@filearray,$aref);
	    undef $aref;
	}
    }
    close($fh);
    chdir ($startdir) if ($startdir ne $cwd);
    return(@filearray);
}


#
# cross_lists -- get the cross section of two env lists
#
# Tags are handled by taking the intersection of them.  Actual environment
# settings, however, are appended if they exist in both environments being
# crossed, separated by a blank.  So if one list has an environment with
# CL=/Ox, and the other has an environment with CL=/Gy, then when those
# two environments are combined, CL="/Ox /Gy".
sub cross_lists
{
    my @first = @{$_[0]};
    my @second = @{$_[1]};
    my $count = 0;
    my ($f, $k, $s, @result);

    return(@first) unless (scalar(@second));
    for $f (@first) {
	for $s (@second) {
	    $result[$count][0] = $count + 1;
	    # iainb: VCQA#2184 can't be solved this easily.
	    # $result[$count][1] = union($f->[1], $s->[1]);
	    $result[$count][1] = intersect($f->[1], $s->[1]);
	    $result[$count][2] = intersect($f->[2], $s->[2]);
	    $result[$count][3] = "$f->[3] $s->[3]";
	    for $k (keys(%{$f->[4]})) {
		$result[$count][4]{$k} = $f->[4]{$k};
	    }
	    for $k (keys(%{$s->[4]})) {
		if (exists($result[$count][4]{$k})) {
		    $result[$count][4]{$k} .= " ${$s->[4]}{$k}";
		} else {
		    $result[$count][4]{$k} = ${$s->[4]}{$k};
		}
	    }
	    $count++;
	}
    }
    return (@result);
}

# 
# read_knownfail
#
sub read_knownfail
{
    if (!length($knownfail)) {
	    return;
    }

    unless (-e $knownfail) {
    	return;
    }

	my $KNOWNFAIL = get_filehandle("<", $knownfail);

    while (<$KNOWNFAIL>) {
        # Skip comments
        next if /^\s*#/;

        # Does this line contain a test result from a results.log?
        # $1 - test name and env label 
        if (/^\s*((?:.+)(?: \(.+\))?)\s*--/) {
            my ($trimmed) = trim($1);
            $knownfail{lc($trimmed)} = 1;
        }
        # Does this line contain a test result from a failures.lst?
        # $1 $2 - test name and env label 
        elsif (/^(?:fail|casc)(?:\d{3,})\s*([^\s]*)\s*\#\s*(\(.*\))?\s*$/) {
            my ($trimmed) = trim("$1 $2");
            $knownfail{lc($trimmed)} = 1;
        }
       }

    close($KNOWNFAIL);
    return;
}

#
# get_env_file -- find and read an environment file
#
# Works for both local and global environment files.  Sets up and returns an
# array of array references.  Each referenced array contains four elements:
# the environment number, the subsets the environment belongs to, the tag,
# and a hash reference which contains the environment changes.
#
# At the beginning, @envlist contains just a single (empty) element, which
# means that the first cross done -- which might be the only one done --
# just results in the contents of the second list.  In this way, the
# traditional style of env list is still supported, but crossing lists
# can be done as well.
#
# Depends on $debug, $local_env_file, and $root.
#
sub get_env_file
{
    my (@dirlist, $found, $search, $temp);
    my (@file, @tokens, @envlist, $envnum);
    my (@sublist);
    my $env;

    # Technique for static locals ; see perlfaq7
    BEGIN {
	# %_envlst_hash caches known env.lst files.  key is directory
	# name, value is an array of array refs with same structure
	# as global @test_list
	my %_envlst_hash = ();
	sub envlst_hash { return \%_envlst_hash; }
    }

    # setup a default array of one element containing a "non-environment"
    $envlist[0] = [1, '*', '*', undef, {}];

    # if running debug:clean, there's no point in parsing environments
    return (@envlist) if ($debug eq 'clean');
    if (!defined($_[0])) {
	return (@envlist);
    } elsif ($_[0] ne $local_env_file) { # global env file
	$found = $_[0];
    } else { # local env file
	$search = cwd();
	$temp = extract_root($root);
	while (!defined($found) && ($search ne $temp)) {
	    if (exists envlst_hash->{$search}) {
		for (@dirlist) {
		    envlst_hash->{$_} = envlst_hash->{$search};
		}
		return @{envlst_hash->{$search}};
	    }
	    @dirlist = (@dirlist,$search);
	    if (-e "$search\\$local_env_file") {
		$found = "$search\\$local_env_file";
	    } else {
		$search =~ s/\\[^\\]+$//;
	    }
	}
    }
    $envlist[0][3] = '';

    if (defined($found)) {
	$envnum = 0;
	ENV_LINE: for $env (read_file($found)) {
	    # if the RUNALL_CROSSLIST divider is found, pause momentarily
	    # to cross the current list with what we already have.
	    if ($env->[2] eq 'RUNALL_CROSSLIST') {
		@envlist = $envlist[0][3] ? cross_lists(\@envlist, \@sublist) : @sublist;
		# start a fresh list
		undef(@sublist);
		$envnum = 0;
		next ENV_LINE;
	    }
	    # environment number
	    $sublist[$envnum][0] = $envnum + 1;
	    # subset and usage tags
	    $sublist[$envnum][1] = $env->[0];
	    $sublist[$envnum][2] = $env->[1];
	    @tokens = parse_string($env->[2]);
	    # check for label
	    CHECK_LABEL: {
		# if non-whitespace exists after '#' comment, use it as an environment label
		($tokens[$#tokens] =~ /^# */) && do {
		    $#tokens--;

		    if (length($')) {
			$sublist[$envnum][3] = $';
			last CHECK_LABEL;
		    }
		};	       

		(defined $env->[3]) && do {
		    $sublist[$envnum][3] = $env->[3];
		    last CHECK_LABEL;
		};
		
		# add default environment label, ie, the env number
		$sublist[$envnum][3] = $sublist[$envnum][0]
			unless $running_failures;
	    } # end CHECK_LABEL
	    # create environment hash
	    for (@tokens) {
		if (/^\s*(\w+)\s*=/)
		{
		    my $var = uc($1);

		    $sublist[$envnum][4]{$var} = $';
		    if ($' =~ /%(\w+)%/)
		    {
			# This code handles variables which rely on prior assignments being expanded. If the
			# variable was set on the current line it's in the $sublist. If it was set before a
			# cross, it's in the $envlist. If it doesn't appear in either, we don't mess with it.
			# Modified by apardoe to correct Perl hash behavior change from build 314 to 616.
			$sublist[$envnum][4]{uc($1)} and $sublist[$envnum][4]{$var} = $sublist[$envnum][4]{uc($1)};
			$envlist[$envnum][4]{uc($1)} and $sublist[$envnum][4]{$var} = $envlist[$envnum][4]{uc($1)};
		    }

            if($var eq "FSIMODE")
            {
                $sublist[$envnum][1] = $sublist[$envnum][1] . ",FSI,NoMT";
            }
            
            if($var eq "SCFLAGS" && $sublist[$envnum][4]{$var} =~ /--noframework\b/)
            {
                $sublist[$envnum][1] = $sublist[$envnum][1] . ",NoCrossVer,NoMT";
            }
		} else {
		    print_noise("Bad assignment '$_', skipping...\n", 1);
		    next ENV_LINE;
		}
	    }
	    $envnum++;
	}
	@envlist = $envlist[0][3] ? cross_lists(\@envlist, \@sublist) : @sublist;
    }
    for (@dirlist) {
	envlst_hash->{$_} = \@envlist;
    }
    return (@envlist);
}

#
# get_defarch_file -- read in a default architecture file
#
# read through the default architecture file to see if it
# has an entry for the current $target. return the remainder
# of the line specifying the command line arguments to add.
#
sub get_defarch_file() {
    return if ($defarch_file eq "");
    return if ($target eq "");

    open(DEFARCH, "<".$defarch_file) || return;
    my $targetarchdef = "";

    print_noise("Reading default architecture file $defarch_file. Looking for entry for target $target.\n", 2);

	my $found = 0;
    while(<DEFARCH>) {
	if (s/^($target\s+)//gi) {
		$found++;
	    chomp;
	    $targetarchdef = $_;
	    last;
	};
    };
    
    close(DEFARCH);
	# get_defarch_file was returning 1 from a successful call to print_noise for empty defarch_file entry.
	# Modified to always return $targetarchdef which is "" when appropriate. VCQA:5745
    if ($targetarchdef ne "") {
		print_noise( "Found default architecture arguments: \"$targetarchdef\".\n", 2);
    } 
	# VCQA:5708 Runall should warn when using unknown target
	if ($found == 0) {
		print_noise( "WARNING: architecture defaults file \"$targetarchdef\" contains no line for $target.\n", 0);
    };
	return $targetarchdef;
}

#
# get_targetdef_file -- read in an target definition file
#
# IN:
#	$_[0]: The name of the target list file.
#	$_[1]: Canonicalize the return mapping.
#
# OUT:
#       RETURN	hash which maps compound architecture names to simple(r) ones.
#
# Parse the target list file. The format is simple; each line has the form:
#
# tag:subtag_1,subtag_2,...,subtag_n
#
# The canonical form for the return value is a map between compound target
# names to all the names for nodes below it in the tree.
#
sub get_targetdef_file($$) {

    my $arch_fname = $_[0];
    my $cannon = $_[1];
    my %target_map;

    open(ARCH, "<".$arch_fname) || return;

    #
    # read in definitions.
    #
    my $line_num = 1;
    while( <ARCH> ) {
	
	#
	# clean up the line.
	#
	chomp;
	s/\#.*//g; # strip comments
	s/\s*//g; # strip ws

	next if ( $_ eq "" );
	
	my @line = split( ':' );

	if ( $#line != 1 ) {
	    leave("Runall.pl line " . __LINE__ . ": Fatal Error: $!\nInvalid format in $arch_fname: \"$_\".\n", $INPUT_ERROR_EXITVAL);
	};

	$target_map{ lc($line[0]) } = lc($line[1]);

	$line_num++;
    };

    close(ARCH);

    if ( $cannon ) {
	#
	# Reduce to canonical form. Expand all compound architecture names for
	# each element in the rhs of the map.
	#
	my $key;
	foreach $key (keys %target_map) {

	    if ( $target_map{ $key } ne $key ) {

		my %def_map;

		$def_map{ $key } = 1;
			    
		my $unexpanded_tags = "";
		$unexpanded_tags = $target_map{ $key };

		while ($unexpanded_tags ne "") {

		    my @next_unexpanded_tags = ();

		    foreach my $tag (split( /,/, $unexpanded_tags ) ) {

			leave("Runall.pl line " . __LINE__ . ": Fatal Error: $!\nCycle detected in tag $key\n", $STATE_ERROR_EXITVAL) if ( $tag eq $key );

			$def_map{ $tag } = 1;

			if ( ($target_map{ $tag } ne $tag) and ($target_map{ $tag } ne "" ) ) {
			    foreach my $child ( split(/,/, $target_map{ $tag } ) ) {
				if ( $def_map{$child} != 1) {
				    push ( @next_unexpanded_tags, $child );
				};
			    };
			};

		    };

		    $unexpanded_tags = join(',', @next_unexpanded_tags );
		};

		$target_map{ $key } = join( ',', keys( %def_map ) );
	    };
	};
    };


    if ( $debug ) {
	print "Target map:\n";
	foreach my $key (keys %target_map) {
	    print "$key : ".$target_map{ $key }. "\n";
	};
    };

    return %target_map;
}

#
# expand_target 
#
# IN architecture name -- potentially a compound name.
# OUT command-separated list of irreducible architecture names.
#
sub expand_target($) {
    my $target = $_[0];
    return $target_map{ $target } if ( $target_map{ $target } ne "" );
    return "";
};


# read_dependence -- read a dependence.lst file
#
# IN:
#     PARAM1			name of dependence file
#     PARAM2	test_to_id	hash mapping test name to test_ids
#
# OUT:
#	  RETURN	dependents	hash mapping test to semicolon-separated list of dependent test ids
#
# POTENTIAL OPT1: point dependents to an index of unique dependent lists. this will make the determining
#			whether the dependents of a given test are completed faster since we can make the 
#			decision in only one place.
sub read_dependence($$) 
{
    my $dependence_fname = $_[0];
    my %test_to_id = %{$_[1]};
    my @cur_dependents;
    my $cur_level = -1;
    my $serial_level = -1;
    my $line = 1;
    my %dependents;
    my @cur_dep_list;
    my $serial_chain = "";
 

    # When finding the test_id for the given test name, 
    # choose the test_id that is the closest (numerically) 
    # to its first dependent. In the event that duplicate test
    # directory names exist, this heuristic should pick the right
    # test -- the closest one in the test list.
    #
    # This is a pseudo-hack. note that there is a bit of hole if the 
    # test list isn't ordered sanely.
    #
    # WHY DO WE HAVE TO ALLOW TEST DIRECTORIES WITH THE SAME NAME?
    sub pick_test_id($$$) {
		my $testname = $_[0];
		my $deps = $_[1];
		my %test_to_id = %{$_[2]};
		my $id;
		my $delta = -1; 
		my $test_id;
	
		# If there's only one test id, test's it.
		my @possible_test_ids = split(/;/, $test_to_id{$testname});
		return $possible_test_ids[0] if ($#possible_test_ids == 0);
	
		print_noise( "WARNING: Multiple tests named ``$testname''; runall is guessing which one is intended in the dependence list. This is dangerous.\n", 0);
	
		# if there are no dependents to guide our choice, choose the last one.
		my @a = split(/;/, $deps);
		return $possible_test_ids[$#possible_test_ids] if ($#a == -1);
	
		foreach $id (@possible_test_ids) {
		    my $depval = pop(@a);
		    if (($delta == -1) || ($delta > ($id - $depval))  ) {
			if ($id > $depval) {
		    	$delta = ($id - $depval);
			};
			$test_id = $id;
	    	};
		};
		return $test_id;
    };



    unless (open(DEP, $dependence_fname)) {
		print_noise( "WARNING: Could not open dependence file $dependence_fname. Assuming no dependencies.\n", 1);
		return;
    };
    print_noise( "reading dependence file $dependence_fname.\n", 3);
    while (<DEP>) {
	
	chomp;

	# ignore lines which are whole-line comments
	next if /^\s*#/;
	
	# strip in-line comments
	s/^(.*)#.*$/$1/g;

	# strip trailing tabs
	s/^(.*)\t+$/$1/g;

	# warn if we see a space
	if (/ +/) {
	    print_noise( "WARNING: could not open dependence file $dependence_fname line $line contains a space. this is dangerous\n", 1);
	};

	if (/^(\t*)(\S[^\t]+)$/) {
	    my $indent_level = length($1);
	    my $test = $2;

	    if ($test eq "BEGIN_SERIAL") {
		leave("Runall.pl line " . __LINE__ . ":  Parsing error in $dependence_fname, BEGIN_SECTION seen before END_SECTION\n", $INPUT_ERROR_EXITVAL) if ($serial_level != -1);
		$serial_level = 0;
	    } elsif ($test eq "END_SERIAL") {
		leave("Runall.pl line " . __LINE__ . ":  Parsing error in $dependence_fname, END_SECTION seen before BEGIN_SECTION\n", $INPUT_ERROR_EXITVAL) if ($serial_level == -1);
		$serial_level = -1;
		$serial_chain = "";
	    } else {
		if ( !defined($test_to_id{$test}) ) {
		    #print "not gonna run $test\n"; 
		    next;
		};

		my $test_id = pick_test_id( $test, ($indent_level>0) ? $cur_dependents[$indent_level-1] : "", \%test_to_id );

		if (($serial_level > 0) and ($cur_level != $indent_level)) {
		    leave("Runall.pl line " . __LINE__ . ":  Parsing error in $dependence_fname, illegal change of indent in SERIAL section.\n", $INPUT_ERROR_EXITVAL);
		    
		};

		$serial_level++ if ($serial_level > -1);

		if ($cur_level != $indent_level) {
		    $cur_dependents[$indent_level] = $test_id;

		    # join all the dependents
		    my %seen;
		    my $x;
		    my $y;
		    @cur_dep_list = ();
		    foreach $x (@cur_dependents) {
			foreach $y (split(/;/, $x)) {
			    push (@cur_dep_list, $y) if (($y != $test_id) && ($seen{$y} != 1));
			    $seen{$y}=1;
			};
		    };

		} else {
		    $cur_dependents[$indent_level] .= ";".$test_id;

		};

		# @cur_dep_list isn't set during the parsing of a serial chain: this looks like an error originally
		# When in a serial section we want to add the cur_dep_list and serial_chain
		# When not in a serial section we want to add the cur_dep_list only if it is non-null
		if ($serial_level > -1) {
        	$dependents{$test_id} = join(';', @cur_dep_list).$serial_chain; 
   		}
      	else {
			# serial_chain should be null in this case
   			$dependents{$test_id} = join(';', @cur_dep_list).$serial_chain if ($#cur_dep_list >= 0);
      	}

		#
		# If we drop n levels of indent, remove the dependent tests for
		# each level above our new position. Note that we do this after
		# we set the dependents for the current test. This allows cleanup 
		# tests to be specified like so:
		# setup
		#	test
		# cleanup
		# and have cleanup depend upon test.
		#
		if ($cur_level > $indent_level) {
		    for (my $i = $cur_level; $i < $#cur_dependents; $i++) {
			$cur_dependents[$i] = "";
		    };
		};

		$cur_level = $indent_level;

		if ( $serial_level > -1 ) {
		    $serial_chain .= ";".$test_id;
		};
	    };

	} else {
	    @cur_dependents = ();
	    @cur_dep_list = ();
	    $cur_level = -1;
	};

	$line ++;
    };
    close DEP;
    return %dependents;
};


# parse_failures -- read in a failures.lst and failures.env file
#
# IN:
#	PARAM1	faillst_name	name of the failures list
#	PARAM2	failenv_name	name of the failures env
#
# OUT:
#	RETURN			a reference to an array of array references indexed by id.
#				each inner array has following elements:
#                               [0] result: casc|fail
#                               [1] id
#                               [2] test name
#                               [3] label
#                               [4] environment array (see get_env_line)
#                                   -> [0] undefined
#                                   -> [1] undefined
#                                   -> [2] undefined
#                                   -> [3] label
#                                   -> [4] environment hash
#
sub parse_failures($$) {

    my $faillst_name = $_[0];
    my $failenv_name = $_[1];

    my @out;

    open (FLST, "<$faillst_name") || return \@out;
    open (FENV, "<$failenv_name");

    my $seq = 0;

    while (<FLST>) {
	if (/^(fail|casc|time)(\d{3,})\s*([^\s]*)\s*\#\s*(\(.*\))?\s*$/) {

	    my $result = $1;
	    my $id = $2;
	    my $testname = $3;
	    my $label = $4;
	    $label =~ s/\((.*)\)/$1/g;
	    my $env = "";
	    my $bFoundEnvEntry;

	    while (<FENV>) {
		if (/^\*\s*$result$id\s*(.*)\#.*/) {
		    $env = $1;
		    $bFoundEnvEntry = 1;
		    last;
		};
	    };

	    leave ("$faillst_name or $failenv_name corrupt! can't find $result$id in the env file", $FILE_ERROR_EXITVAL) if (!$bFoundEnvEntry);

	    my @aref;
	    my @locenv;
	    my %envhash;

	    my @tokens = parse_string($env);
	    # create environment hash (copied from get_env_line)
	    for (@tokens) {
		if (/^\s*(\w+)\s*=/)
		{
		    my $var = uc($1);
		    $envhash{$var} = $';
		    if ($' =~ /%(\w+)%/)
		    {
			$envhash{uc($1)} and $envhash{$var} = $envhash{uc($1)};
			$envhash{uc($1)} and $envhash{$var} = $envhash{uc($1)};
		    }                    
		}
	    }
	    $locenv[3] = $label;
	    $locenv[4] = \%envhash;

	    $aref[0] = $result;
	    $aref[1] = $id;
	    $aref[2] = $testname;
	    $aref[3] = $label;
	    $aref[4] = \@locenv;

	    $out[$seq++] = \@aref;
	};
    };

    close (FLST);
    close (FENV);

    return \@out;
};

# parse_results -- read in a results.log
#
# IN:
#	PARAM1	results_filename	name of the results log
#
# OUT:
#	RETURN			a reference to an array of array references.
#				each of these has following elements:
#                               [0] result (integer)
#                               [1] test name (directory)
#                               [2] environment (with parenthesis)
#                               [3] result comment
#
sub parse_results($) {
    my $results_filename = $_[0];
    my @out;


	my $SUBTEST_RESULTFILE = get_filehandle("<", $results_filename);

    while (<$SUBTEST_RESULTFILE>) {
	last if /tests passed/;
	my @a = split(/ -- /);
	if ($#a >= 1) {
	    if ( $a[0] =~ /^([^\)\(]+)(\(.*\))?\s*/ ) {
		my $testname = $1;
		my $env = $2;
		$testname =~s/\s+$//;
		$env =~ s/\s*\((.*)\)/\1/g;
		if ( $a[1] =~ /([a-zA-Z_]*)\s*(.*)?$/ ) {
		    my $result = $1;
		    my $comment = $2;
		    $comment = "-- ".$a[2] if ($a[2] ne ""); # handle timing
		    $comment =~ s/^: //;

		    my $aref;
		    $aref->[0] = $NAMES_TO_RETVAL{$result};
		    $aref->[1] = $testname;
		    $aref->[2] = $env;
		    $aref->[3] = $comment;

		    push( @out, $aref );
		};
	    };
	};

    };
    close SUBTEST_RESULTFILE;

    return \@out;

}

################################
# TEST SETUP/CHECKING ROUTINES #
################################


#
# find_runpl -- look for a run.pl script to run the test
#
# If run.pl is not found in the test directory, it is looked for in
# successive parent directories until (a) the directory from which
# runall.pl was run or (b) the root directory of the filesystem is
# reached.
#
# relies upon $root and %runpl_hash.
#
sub find_runpl
{
    my (@dirlist, $search, $temproot, $found);

    # Technique for static locals ; see perlfaq7
    BEGIN {
	# %_runpl_hash caches known run.pl files.  key is directory
	# name, value is the pathname of the run.pl file to use
	my %_runpl_hash = ();
	sub runpl_hash { return \%_runpl_hash; }
    } 

    $search = cwd();
    $temproot = extract_root($root);
    while (!defined($found) && (index($temproot, $search) == -1)) {
	if (exists runpl_hash->{$search}) {
	    $found = runpl_hash->{$search};
	    last;
 	} elsif (-e "$search\\run.exe") {
 	    $found = "$search\\run.exe";
	} elsif (-e "$search\\run.pl") {
	    $found = "$search\\run.pl";
	}
	@dirlist = (@dirlist, $search);
	$search =~ s/\\[^\\]+$//;
    }
    for (@dirlist) {
	runpl_hash->{$_} = $found;
    }
    return ($found);
}



#
# check_maxfails -- check whether we've reached the max # of tests to run
#                   
# Returns non-zero if we've reached the max # of failures, zero otherwise.  
# This is a separate function from check_maxtests because it needs to work in multiproc mode
#
# Depends on $maxfails, $failure_count.
#
sub check_maxfails
{
    return 0 if $maxfails == -1;

    # Technique for static locals ; see perlfaq7
    BEGIN {
		my $_failure_count = 0;
		sub increment_failure_count {
	    	return ++$_failure_count;
		}
	}
    return increment_failure_count() > $maxfails - 1;
}

#
# check_maxtests -- check whether we've reached the max # of tests to run
#                   
# Returns non-zero if we've reached the max # of tests, zero otherwise.  
# Increments raw test count if we're not in resume mode.
#
# Depends on $maxtests, $resume_mode, $raw_test_count
#
sub check_maxtests
{
    return 0 if ($maxtests == -1 || $resume_mode == 1);

    return ++$raw_test_count > $maxtests;
}

#
# check_target -- check whether a test is run for the target platform
#
# Returns non-zero if the test should be run, zero otherwise.
#
# Depends on $target, $root
#
sub check_target {
    my (@dirlist, $search, $temproot, $dorun);

    # Technique for static locals ; see perlfaq7
    BEGIN {
	# %_notgt_hash caches known notarget.lst files.  Key is directory
	# name, value is 1 if test should be run, 0 otherwise
	my %_notgt_hash = ();
	sub notgt_hash { return \%_notgt_hash; }
    }

    $search = cwd();
    $temproot = extract_root($root);
    while (!defined($dorun) && (index($temproot, $search) == -1)) {
	if (exists notgt_hash->{$search}) {
	    $dorun = notgt_hash->{$search};
	    last;
	} elsif (-e "$search\\notarget.lst") {
	    for (read_list_file("$search\\notarget.lst")) {

		my $notarget = lc($_);

		#$notarget =~ s/cee/x86clr/ig; # HACK -- DO NOT CHECKIN
		$notarget =~ s/x86clr/cee/ig; # HACK -- DO NOT CHECKIN
		$notarget =~ s/amd64clrpure/amd64clr/i;
		$notarget =~ s/ia64clrpure/ia64clr/i;
		
		if ($notarget eq lc($target)) {
		    $dorun = 0;
		    last;
		};

		my @target_map_keys = keys %target_map;
		if ( $#target_map_keys > 0 ) {

		    if ( expand_target($notarget) eq "" ) {
			leave("Runall.pl line " . __LINE__ . ": Fatal Error: $!\nUnknown target in notarget.lst ($notarget in $search\\notarget.lst).\n", $INPUT_ERROR_EXITVAL);
		    };

		    my $expanded_notarget = expand_target( $notarget );

		    if (intersect( $expanded_notarget, lc($target) ) ) {
			$dorun = 0;
			last;
		    }
		};
	    }
	    # VCQATest:1269
	    # Stop at the first notarget.lst found. This matches the 
	    # behavior of the other files (run.pl, env.lst).
	    last;
	}
	@dirlist = (@dirlist, $search);
	$search =~ s/\\[^\\]+$//;
    }

    if (!defined($dorun)) {
		$dorun = 1;
    }

    for (@dirlist) {
		notgt_hash->{$_} = $dorun;
    }

    return $dorun;
}

#
# check_tags -- check subset tags to see if they are currently valid
#
# If the tags contain a "*", they will match anything, so don't check.
# Returns skip string if tags don't match, zero/empty string otherwise.
#
# Depends on $nottags.
#
sub check_tags
{
    my ($tags, $currtags) = @_;
    my $match;
    my $skipped;

    $skipped = '';
    print_noise("\tChecking tags\n", 2);
    print_noise("\t\tcurrent: $tags\n\t\tagainst: $currtags\n",3);
    if (($tags !~ /(^|,)\*(,|$)/) && ($currtags !~ /(^|,)\*(,|$)/)) {
	if (!intersect($tags, $currtags)) {
	    $skipped = 'no active subset';
	} elsif ($match = intersect($tags, $nottags)) {
	    $skipped = "not running subset(s) '$match'";
	}
    }
    return $skipped;
}


#
# check_global -- test if global environment is to be run
#
# Depends on $noise and $gtags.
#
sub check_global
{
    my ($globenv) = @_;
    my $skipped;

    unless ($skipped = check_tags($globenv->[1], $gtags)) {
	return 1;
    }
    if ($noise > 1) {
	print_skip('global environment',$globenv->[3],undef,$skipped);
    }
    return 0;
}

#
# check_resume -- make sure specified test syncs with current point in
#                 resume file
#
# Depends on RESULTFILE, $resume_mode, @totals_count, and the
# naming conventions for test results (see print_results, print_skip)
#
sub check_resume
{
    my ($label) = @_;
    my $curline;
    my $filepos;
    my %result_index = (
	'passed'     => $PASSED,
    'failed'     => $FAILED,
    'skipped'    => $SKIPPED,
    'cascade'    => $CASCADE,
    'no_result'  => $NO_RESULT,
    'timed_out'  => $TIMED_OUT,
    'test_error' => $TEST_ERROR);

    if (!$resume_mode) {
	return 0;
    }

    # Technique for static locals ; see perlfaq7
    BEGIN {
	my $_resume_test_count = 0;
	sub next_resume_test_count {
	    return ++$_resume_test_count;
	}
	sub print_resume_update {
	    my $name = shift;
	    if (0 == (next_resume_test_count() % 1000)) {
			print_noise("Found test #$_resume_test_count: '$name', while " .
			"resuming test run\n", 0);
	    }
	}
    }

    # Loop until we find the next completed test in the results log
    RECORDSEEK:
    while (!eof($RESULTFILE)) {
		$curline = <$RESULTFILE>;
		chomp $curline;

		# Skip comments or blank/whitespace lines
		next if ($curline =~ /^\s*#/ || $curline =~ /^\s*$/);

		# Previously we used a regex to split this line but it became well too 
		# complicated--and would have gotten slower--when we needed to handle file
		# paths with embedded spaces. -- is the natural results.log delimiter, so 
		# why not use it? We skip any lines which are blank or comments above. This
		# assumes a regular format of the results log but we control that too...
		my ($test_name, $test_result) = split / -- /, $curline; 
		($test_result) = split / /, $test_result; # trim runall_skipped messages
		$test_name =~ s/\s*$//;	# Trim trailing whitespace from test_name
	    if ($label eq $test_name) {
		print_resume_update($label);

		# Save the last test executed in the previous run. This has to be done b/c if a testrun was aborted
		# inside a testcase group (it was not on the last permutation we aborted) we need to know the last
		# test run so we can determine if we are in a testcase group and do not print the opening <Test ...>
		# tag twice. By the time we are done resuming $xml_previous_test should contain the last test executed.
		$xml_previous_test = $label;

		# Reconstruct totals count
		if (exists $result_index{$test_result}) {
		    ++$totals_count[$result_index{$test_result}];
		}
		else {
		    print_noise("Test '$test_name' has unknown result value '$test_result'; " .
		    "test totals will be suspect.\n", 0);
		}

		return 1;
	    }
	    else {
		leave("Runall.pl line " . __LINE__ . ":  Error resuming tests.\n" .
		"\tExpected: '$label' in '$result_file'\n" .
		"\tFound: '$test_name'\n", $STATE_ERROR_EXITVAL);
		}
    }

    # We've already read the last test result, turn off resume mode and 
    # start appending at this point
    seek $RESULTFILE, 0, 1;	      # Reset eof error, we're through resuming
    write_to_file( \$RESULTFILE,  "\n" );	      # Guarantee we start on a new line
    $resume_mode = 0;
    print_noise("Runall.pl line " . __LINE__ . ":  Test run resumed at test #" .
    next_resume_test_count() . ": '$label'.\n", 0);
    if ($xml) {
		$xml_previous_test =~ s/ \(.+\)//; #strip permutation tag
	}
    return $resume_mode;
}

#
# check_test -- make sure test is set up correctly for running
#
# Returns 1 if there are no errors in the test setup. Returns the string
# describing why the test was wrong otherwise.
#
# Depends on $debug, @totals_count, $noise, $found_runpl, $ttags and $root.
#
sub check_test
{
    my ($test, $globenv) = @_;
    my $error;

    # If we're only listing test subdirectories, there is no need to check
    # the correctness of the test setup
    return 1 if ($debug eq 'list');

    if (chdir("$root\\$test->[2]") == 0) {
       if("$root\\$test->[2]" =~ m/\btestsprivate\b/) {
           $error = "Skipping private test"
       } else {
           $error = "test directory '$root\\$test->[2]' doesn't exist";
       }
    } elsif (!defined($found_runpl = find_runpl())) {
	$error = "no run.pl script found";
	
    } elsif ($found_runpl =~ /run\.exe/ ) {
		#print "Using '$found_runpl'\n";
	} elsif (!check_target()) {

    # perl is noisy during syntax checks; redirect to "nul".
	# Removed !$resume_mode from this conditional to allow resumes past tests with run.pl errors
    } elsif (!$debug && (cmd_redirect("$perl -c " . quote_path($found_runpl), 'nul') != 0)) {
	$error = "run.pl contains errors";

    }

    if ($error) {
		return $error;
    } else {
		return 1;
    }
}

#
# check_local -- test if local environment is to be run
#
# Depends on $noise and $ltags.
#
sub check_local
{
    my ($test, $globenv, $locenv) = @_;
    my ($skipped, $utags);

    unless ($skipped = check_tags($locenv->[1],union($ltags,$globenv->[2]))) {
	# check usage tags
	print_noise("\tChecking usage tags\n", 2);
	print_noise("\t\tlocal env: $locenv->[2]\n\t\ttest list: $test->[1]\n", 3);
	$utags = not_in(intersect($test->[1],$usage_filter),$usage_neg_filter);
	$utags = '__ra_nomatch__' unless ($utags);
	if (!intersect($locenv->[2], $utags)) {
	    $skipped = "env not valid with test";
	} else {
	    return 1;
	}
    }
    # Never print skip if in resume mode - the previous log already
    # reflects that
    if (($noise > 1) && !$resume_mode) {
		print_skip($test->[2], $globenv->[3], $locenv->[3], $skipped);
    }
    return 0;
}



########################
# ENVIRONMENT ROUTINES #
########################


#
# apply_changes -- apply environment file changes
#
# Return value is a hash containing the portion of the original environment
# that was overwritten.
#
sub apply_changes
{
    my %envhash = @_;
    my %delta;

    print_noise("\tApplying environment changes\n", 2);
    for (keys(%envhash)) {
	$delta{$_} = $ENV{$_};
	$ENV{$_} = expand($envhash{$_});
	print_noise("\t\t$_ = $ENV{$_}\n", 3);
    }
    return %delta;
}


#
# undo_changes -- restore original environment from a delta
#
# %ENV is a pseudo-hash, so undefined elements have to actually be deleted,
# lest they become Banquo's ghost.  Input to this subroutine is the hash
# returned from apply_changes().
#
sub undo_changes
{
    my %delta = @_;

    print_noise("\tUndoing environment changes\n", 2);
    for (keys(%delta)) {
	if (defined $delta{$_}) {
	    $ENV{$_} = $delta{$_};
	} else {
	    delete $ENV{$_};
	}
	print_noise("\t\t$_ = $ENV{$_}\n", 3);
    }
}


#
# env_label -- create an environment label
#
# Uses the local and global environment labels to produce a unique label for
# a given test run.  If one of them is undefined, it means that that kind
# of environment file is not in use for this run.
#
sub env_label
{
    my ($gtag, $ltag) = @_;
    my $label = '';

    if (!(defined $ltag) && ($gtag ne '')) {
	$label = " ($gtag)";
    } elsif (!(defined $gtag) && ($ltag ne '')) {
	$label = " ($ltag)";
    } elsif (($gtag ne '') || ($ltag ne '')) {
	$label = " ($gtag:$ltag)";
    }
    return $label;
}


#
# dump_env -- print an environment set
#
sub dump_env
{
    my ($glob, $loc) = @_;

    print "   Global:\n";
    for (keys(%{$glob})) {
	print "      $_ = $glob->{$_}\n";
    }
    print "   Local:\n";
    for (keys(%{$loc})) {
	print "      $_ = $loc->{$_}\n";
    }
    print "\n";
}



#################################
# TEST RUNNING/CLEANUP ROUTINES #
#################################


#
# run_hook -- execute a hook function
#
# A hook function here is just a user-defined Perl script.  Copy it to the
# current directory, execute it, and then delete it.  The script is run via
# require() so that any environment changes get saved.
#
sub run_hook
{
    my $filename = $_[0];

    return if ($filename eq "");		# perl 5.6.0 now thinks an empty filename exists.
    return unless (-e $filename);
    print_noise("Running hook function $_[0]\n", 2);
    eval `type $filename`;
}

#
# grep_Bt_info -- grep timing info from /Bt compiler switch
# 
# Usually this comes from runpl.log but some tests redirect compiler output
# to a file called "compiler.out". Obviously this isn't foolproof.
# The $tmp variable is used because of an apparent bug in Perl 5.314 wherein
# the $_ variable is not getting reset properly in while<> and for() loops
#
sub grep_Bt_info
{
    my $tmp = shift;
    return unless (-r "$tmp");    
    open COMPILER_OUTPUT_REDIRECT, "<$tmp" or return;
    while(<COMPILER_OUTPUT_REDIRECT>)
    {
	/time/i and do
	{
	    /c1\.dll/ and /(\d*\.\d{3}s)/,      
	    $Bt_info{'c1'} ? $Bt_info{'c1'} += $1 : $Bt_info{'c1'} = $1,
	    next;
	    /c1xx\.dll/ and /(\d*\.\d{3}s)/,
	    $Bt_info{'c1xx'} ? $Bt_info{'c1xx'} += $1 : $Bt_info{'c1xx'} = $1,
	    next;
	    /c2\.dll/ and /(\d*\.\d{3}s)/,
	    $Bt_info{'c2'} ? $Bt_info{'c2'} += $1 : $Bt_info{'c2'} = $1,
	    next;
	    /LibDef/ and /(\d*\.\d{3}s)/,
	    $Bt_info{'ld'} ? $Bt_info{'ld'} += $1 : $Bt_info{'ld'} = $1,
	    next;
	    /MD Merge/ and /(\d*\.\d{3}s)/,
	    $Bt_info{'mm'} ? $Bt_info{'mm'} += $1 : $Bt_info{'mm'} = $1,
	    next;
	    /OptRef/ and /(\d*\.\d{3}s)/,
	    $Bt_info{'or'} ? $Bt_info{'or'} += $1 : $Bt_info{'or'} = $1,
	    next;
	    /OptIcf/ and /(\d*\.\d{3}s)/,
	    $Bt_info{'oi'} ? $Bt_info{'oi'} += $1 : $Bt_info{'oi'} = $1,
	    next;
	    /Pass 1/ and /(\d*\.\d{3}s)/,
	    $Bt_info{'l1'} ? $Bt_info{'l1'} += $1 : $Bt_info{'l1'} = $1,
	    next;
	    /Pass 2/ and /(\d*\.\d{3}s)/,
	    $Bt_info{'l2'} ? $Bt_info{'l2'} += $1 : $Bt_info{'l2'} = $1,
	    next;
	    /Final:/ and /(\d*\.\d{3}s)/,
	    $Bt_info{'lf'} ? $Bt_info{'lf'} += $1 : $Bt_info{'lf'} = $1,
	    next;
	    /link\.exe/ and /(\d*\.\d{3}s)/,
	    $Bt_info{'lt'} ? $Bt_info{'lt'} += $1 : $Bt_info{'lt'} = $1,
	    next;
	    /Time of execution/ and /(\d+\.\d{3})/,
        $Bt_info{'te'} ? $Bt_info{'te'} += $1 : $Bt_info{'te'} = $1,
        next;
	}
    }
    close COMPILER_OUTPUT_REDIRECT;
}

#
# starts an external process which can host an in-proc version of compiler under test
# Run.pl contains the required logic to communicate with this process
#
sub launch_compiler_host
{
  use File::Basename;
  use Win32::Process;

  if($ENV{HOSTED_COMPILER} eq "1"){
    if(defined $compilerServerPort){}
    else{ $compilerServerPort = 11000 }
    my $dirName = dirname(__FILE__);
    Win32::Process::Create($compilerServerProc,
                           "$dirName\\HostedCompilerServer.exe",
                           "HostedCompilerServer.exe $compilerServerPort",
                           0,
                           CREATE_NO_WINDOW,
                           ".")|| die "Error starting compiler server";
                           
    $ENV{HOSTED_COMPILER_PORT} = $compilerServerPort;
  }
}

#
# kills the external compiler host process, if it has been started
#
sub kill_compiler_host
{
  if(defined $compilerServerProc){ $compilerServerProc->Kill(1)}
}

#
# run_test -- execute a test directory
#
# The return value is the return value from the test, modified (if necessary)
# to be within the 0 to 5 required range.
#
# Depends on $terse_mode, $found_runpl, $debug, $debug_retval
#
sub run_test
{
    my $count = 0;
    my $retval;
	my $test_dir = cwd();

    return $debug_retval if ($debug);

    if (-e "$test_dir\\stopit_abnormal.txt") {
        verify_unlink("$test_dir\\stopit_abnormal.txt");
    }
	$ENV{CURRENT_TEST_DIRECTORY} = $test_dir; # Tell stopit where to put semaphore file
    # We should never have a runpl.log sitting in a directory BEFORE the test executes.
    if (-e "$file_prefix$runpl_log") {
        unless (-w "$file_prefix$runpl_log") {
            $readonlylogs{$ENV{CURRENT_TEST_DIRECTORY}}++;
        }
        verify_unlink("runpl.log");
    }

    unless (defined($found_runpl)) {
        leave("\a\a\aRunall.pl line " . __LINE__ . ": No run.pl, shouldn't be here!!!\n", $STATE_ERROR_EXITVAL);
    }
    print " RUNNING" unless ($terse_mode);
    $retval = cmd_redirect( sub { return launch_runpl(quote_path($found_runpl)); } , "$file_prefix$runpl_log");
    print "\cH" x 8, " " x 8, "\cH" x 8 unless ($terse_mode);
    if (($retval > $TIMED_OUT) || ($retval < $PASSED)) {
        $retval = $FAILED;
    }
    # If user has selected timing:high grep /Bt info from local runpl.log (or compiler.out)
    if ($timing{'level'} ge $TIMING_HIGH)
    {
        undef %Bt_info; 
        # Tests redirect the compiler output to many filenames. Search until we find one.
        # The $tmp variable is used because of an apparent bug in Perl 5.314 wherein
        # the $_ variable is not getting reset properly in while<> and for() loops
        my $tmp;
        for $tmp ("$runpl_log", 'compiler.out', 'compile.log', 'tmp.log')
        {
            last if %Bt_info;
            grep_Bt_info($tmp);
        }
    }   

    # If test has been stopped by stopit then we need to return TIMED_OUT
    if (-e "$test_dir\\stopit_abnormal.txt") {
        return $TIMED_OUT;
    } 
    else {
        return $retval;
    }
}


#
# cmd_redirect -- execute a cmd, redirecting stdout, stderr.
#
# cmd can either be a string or a reference to code.  strings
# are passed to system(), references are called as subs with
# no arguments.
#
# Redirects STDERR to STDOUT, and then redirects STDOUT to the
# argument named in $redirect.  It is done this way since
# invoking system() with i/o redirection under Win9x masks
# the return code, always yielding a 0.
#
# The return value is the actual return value from the test.
#
sub cmd_redirect
{
    my ($cmd, $redirect) = @_;
    my $retval;

    open SAVEOUT, ">&STDOUT";
    open SAVEERR, ">&STDERR";

    open STDOUT, ">$redirect" or leave("Runall.pl line " . __LINE__ . ": Can't redirect stdout to '$redirect'\n", $OTHER_ERROR_EXITVAL);
    open STDERR, ">&STDOUT" or leave("Runall.pl line " . __LINE__ . ": Can't dup stdout\n", $OTHER_ERROR_EXITVAL);

    select STDERR; $| = 1;
    select STDOUT; $| = 1;

    if ( (ref $cmd) eq 'CODE' ) {	
	$retval = &$cmd();
    }
    else {
	$retval = system($cmd) >> 8;
    }

    close STDOUT;
    close STDERR;

    open STDOUT, ">&SAVEOUT";
    open STDERR, ">&SAVEERR";

    return $retval;
}

#
# get_localkeepfiles
#
# Returns a list of the RW files in the current directory.
#
# This is run from a test dir BEFORE executing the test so that we
# can keep checked-out source code (which will be RW) but still hack
# object files and other intermediates after the test.
#
# runpl.log files that exist in this dir or any subdir will *not* be
# included in this list.
#
# No dependencies.
#
sub get_localkeepfiles
{
    my $qmeta_runpl_log = quotemeta("$file_prefix$runpl_log");

    return if (($cleanup eq 'no') || ($debug && ($debug ne 'clean')));

    # REVIEW - This is a temporary measure to allow cleanup on Win9x.
    # Win9x doesn't like stderr redirection, and dir fails if it is seen
    my $redir = $ENV{OS} eq "Windows_NT" ? "2> nul" : "";
    chomp(@local_keepfiles = `dir /s /b /a-r-d $redir`);
    @local_keepfiles = grep { !m/\\$qmeta_runpl_log$/i } @local_keepfiles;
}

#
# launch_runpl
#
sub launch_runpl
{
	my $runpl = shift;
	my ($proc, $retval);

	$timing{'start'} = time if ($timing{'level'} ge $TIMING_LOW); 

	# be very conservative here. note that if we use CreateProcess, the 
	# Perl environment doesn't get imparted to the new process. ActiveState
	# bug #16448, affecting ActivePerl <= 623 at least.
    if ( $timeout > 0 ) {
 		if ( $runpl =~ /run\.exe/ ) {
			$retval = system("stopit.exe -s$timeout $runpl");
		}
		else {
			$retval = system("stopit.exe -s$timeout $perl $runpl") >> 8;
		}
	} 
	else {
 		if ( $runpl =~ /run\.exe/ ) {
			$retval = system("$runpl");
		}
		else {
			$retval = system("$perl $runpl") >> 8;
		}
	}

	$timing{'stop'} = time if ($timing{'level'} ge $TIMING_LOW); 
	return $retval;
}

#
# cleanup_directory -- cleans up after a test is finished running
#
# Relies upon the @keepfiles global array and $cleanup.  This function
# operates by building two lists -- one a list of files to delete, and
# the other a list of files to keep that have R/W attributes.  It then
# changes the R/W keep files to be read-only, modifies the delete list to
# weed out R/O files, deletes the rest, and changes the permissions back
# on the first list.  In this way, all files in the keep list are
# preserved and also keep their access attributes intact.
#
# Depends on $debug, $noise and @keepfiles.
#
sub cleanup_directory
{
    my(@repros, @delete_list, @tempkeep, @keep_list, $print_list);
    my ($i, $filename, $cwd);

    return if (($cleanup eq 'no') || ($debug && ($debug ne 'clean')));
    print_noise("\tClean up directory\n", 2);
    if (-e 'delete.lst') {
	@delete_list = map {/[\*?]/?glob $_:$_} read_list_file('delete.lst');    
    }
    else {
	# REVIEW - This is a temporary measure to allow cleanup on Win9x.
	# Win9x doesn't like stderr redirection, and dir fails if it is seen
	my $redir = $ENV{OS} eq "Windows_NT" ? "2> nul" : "";
	chomp(@delete_list = `dir /s /b /a-r-d $redir`);
	@keep_list = read_list_file('keep.lst');
	# Hack for VCQA:6834 (Unicode parsing) but also saves time in the general case
	return if (grep {$_ eq '*'} @keep_list);	
	push @keep_list, @local_keepfiles if ($noclobber);
    }
    return if (0 == scalar(@delete_list));
    
    # save build keep list, and mark R/W elements as R/O to protect them;
    # keep list is repro*.bat files from the current run, "always keep"
    # files, the keep list for this particular directory, and any special
    # files passed to the function.
    @repros = grep { -M $_ <= 0 } <repro*.bat>;
    @tempkeep = grep {-w $_} map {/[\*?]/?glob $_:$_} (@repros, @keepfiles, @keep_list, @_);
    chmod 0555, @tempkeep;

    # Perl 5.003 allows unlink to delete R/O files, so get rid of those
#    @delete_list = grep {-w $_} @delete_list;

    # find relative path
    $cwd = cwd;
    $cwd =~ tr/\//\\/;
    foreach $filename ( @tempkeep ){
        for ($i = 0; $i <= $#delete_list; $i++) {
            if ( "\L$delete_list[$i]" eq "\L$filename"  || "\L$delete_list[$i]" eq "\L$cwd\\$filename") {
                splice @delete_list, $i, 1;
            }
        }
    }
    if ($noise > 3) {
	$print_list = '';
	map { $print_list .= "\t\t$_\n" } @delete_list;
	print_noise($print_list, 3);
    }

    verify_unlink(@delete_list);

    # change permissions back on files that were changed
    chmod 0777, @tempkeep;
}


#
# Extracts root for the specified path
#
sub extract_root()
{
    my $path = shift();
    return (rindex($path, '\\') < 0)? $path : substr($path, 0, rindex($path,'\\'));
}

#
# Get the suite name from the first line in the test.lst.
#
sub get_suite_name()
{
	if (defined $xmlsuitename) {
		return $xmlsuitename;
	}
    my $name = "";
	# Don't try to read test name if we are running a single directory
	return $name if -d ($test_file);
	
	my $TFILE = get_filehandle("<", $test_file);
    my $first_line = <$TFILE>;

    if ($first_line =~ /^#\s*SuiteName:\s*(.+)$/)
    {
        $name = $1;
    }
    else
    {
        leave ("Runall.pl line " . __LINE__ . ": \"#SuiteName:suite\" comment must be at the beginning of test.lst file or\nspecified on command line!\n", $INPUT_ERROR_EXITVAL);
    }

    close $TFILE;
    
    return $name;
}

sub get_results_filesize() {
    my $fname = $results_root."\\".$file_prefix.$result_file;
    my $size = (stat( $fname ))[7];
    #print "SIZE=$!;$size;$fname; \n". join(',', @_)."\n";
    return $size;
}
    
sub check_filelock {
    my $results_filesize_before = shift;
    my $results_filesize_after = get_results_filesize();

    if ( $results_filesize_before == $results_filesize_after ) {
        
        if ($filelock & $FILELOCK_SYNC) {
            # Also try calling sync.exe (from www.sysinternals.com) on x86 architectures
            # Binary won't be found on 64-bit architectures but this shouldn't matter
			# VCQA:6746 output of sync.exe interfering with test output in conformance suite
            system("$ENV{ADMIN_PIPE} sync.exe > NUL");
            if ($ENV{SIMULATOR_PIPE}) {
                system("$ENV{SIMULATOR_PIPE} $ENV{ADMIN_PIPE} sync.exe > NUL");
            }
        }
        $results_filesize_after = get_results_filesize();
    }

    if ( $results_filesize_before == $results_filesize_after ) {
        print STDERR "Runall Error!\nResults filesize didn't change during a test $results_filesize_before vs. $results_filesize_after!\n";
        if ($filelock & $FILELOCK_USER) {
            print "Please press <Enter> to continue the run:";
            getc;
            print "\n";
        }
        # NB: $FILELOCK_USER overrides $FILELOCK_ABORT so this always needs to be in an else clause
        elsif ($filelock & $FILELOCK_ABORT) {
            leave( "Runall.pl line " . __LINE__ . ": Runall Error!\nResults filesize didn't change during a test!\n", $FILE_ERROR_EXITVAL);
        }
    }; 
}

# This function is common to RunallSingleProcess and RunallMultiProcess
sub save_the_log_file {
    my ($retval, $test, $label, $globenv, $locenv, $skipmsg) = @_;
    
    # Runpl.xml requires a 'GroupID' attribute on the TestOutput tags.
    # The id is incremented by 1 every time $GROUPSIZE lines are seen.
    my $lines_seen;
    BEGIN {
    	my $_lines_seen = 0;
	    sub add_lines_seen {
            $_ = shift;
            $_ eq "reset" ? $_lines_seen = 0 : $_lines_seen += $_;
	        return $_lines_seen;
        }
	}

    my $fail_uuid = undef; # initing seems unnecessary...
    
    if (!$debug && (($savelog eq 'all') ||
        (($savelog eq 'pass') && ($retval eq $NAMES_TO_RETVAL{'passed'})) ||
        # Was previously $savelog eq 'fail' and save 1|2|3|5, now save all but passing tests
        (($savelog eq 'fail') && ($retval ne $NAMES_TO_RETVAL{'passed'})))) {
        my $errtext;

        write_to_file (\$RUNPLFILE, "+++ $test->[2]$label +++\n");
        my (@local_runpl_text, $local_runpl_size);
		if (-e "$file_prefix$runpl_log") {
			my $LOCAL_RUNPLFILE = get_filehandle("<", "$file_prefix$runpl_log");
    	    @local_runpl_text = <$LOCAL_RUNPLFILE>;
        	$local_runpl_size = scalar @local_runpl_text;
	        close($LOCAL_RUNPLFILE);
		}

		# Have no local_runpl_text for TEST_ERROR tests and some SKIPPED tests
		if ($local_runpl_size < 1) {
        	if ($retval eq $TEST_ERROR || $retval eq $SKIPPED) { 
            	@local_runpl_text = ($skipmsg, "\n");
				$local_runpl_size = 2;
    	    }
		}

        if ($xml) {
			# Truncate runpl.xml output at $halflimit * 2 lines per test
			my $halflimit = 500;
			if ($local_runpl_size > $halflimit * 2) {
				$lines_seen = $halflimit;
				for (my $i = 0; $i < $halflimit; $i++) {
		            $errtext .= $local_runpl_text[$i];
				}
				$errtext .= "\n!!! runpl.log entry is too long -- runall.pl has removed the middle from the xml logfile !!!\n\n";
				for (my $i = $halflimit; $i > 0; $i--) {
		            $errtext .= $local_runpl_text[$local_runpl_size - $i];
				}
			}
			else {
	            $errtext .= join "", @local_runpl_text;
			}
   	        $lines_seen = add_lines_seen($local_runpl_size);
        }
        write_to_file(\$RUNPLFILE, join ("", @local_runpl_text) );
        write_to_file(\$RUNPLFILE, "\n") 
			unless @local_runpl_text[$local_runpl_size - 1] =~ m/\n$/; 

        if ($xml) {
            $fail_uuid = gen_uuid();
            if ($lines_seen >= $GROUPSIZE) {
                $last_groupID++;
                $lines_seen = add_lines_seen("reset");
            }
            print_fail_xml($test->[2], $globenv, $locenv, $label, $errtext, $fail_uuid);
        }
    }

    # Printing the results_xml after the fail_xml (if any) b/c results_xml
    # is dependent upon whether a fail_uuid was generated or not.
    print_results_xml($test->[2], $label, $retval, $fail_uuid) if ($xml);
}
        
########################
##### MAIN PROGRAM #####
########################

use vars qw($globenv
$locenv 
%ORIG_GLOB_ENV
%ORIG_LOC_ENV
$label
$retval
$test
@testlist);


#
# main test execution loop for the single processing
# mode of runall
#
sub RunSingleProcess() {
    my %testname_seen;

    read_knownfail();
    rename_runall_files();
    open_files($resume_mode ? "resume" : "open");
    print_fail_header() unless $resume_mode;
    if ($terse_mode) {
    if ($ttags eq '__ra_alltag__') {
        print substr($root, rindex($root,'\\') + 1) . "\n";
    } else {
        print "$ttags\n";
    }
    }

	unless ($resume_mode || $multiproc_child) {
	    # !$resume_mode b/c we only want root XML
	    # elements once.
	    # !$multiproc_child is for -procs:n > 1 runs so that
	    # the local runpl.xml can easily be merged
	    # back into the master runpl.xml without having
	    # to strip off the root and tail XML elements
	    if ($xml) {
		    my $uuid = gen_uuid();
			my $suite_name = get_suite_name();	
	        add_test_results_element($suite_name, $uuid);
	        add_runpl_log_element($suite_name, $uuid);
	    }
	    dump_run_info();
	}
    
    for $globenv (@global_env_list) { 
    print_noise("\tstart GLOBAL\n", 2);
    next unless check_global($globenv);
    %ORIG_GLOB_ENV = apply_changes(%{$globenv->[4]});
    for $test (@testlist) {
        print_noise("\tstart TEST\n", 2);
        
        # REVIEW - moved the following check_tags call out of check_test because it is 
        # not an "error" condition. This throws off resumed test runs.
        if ($skipped = check_tags($test->[0],union($ttags,$globenv->[2]))) {
            if ($noise > 1) {
                print_skip($test->[2], $globenv->[3], undef, $skipped);
            	save_the_log_file($SKIPPED, $test, $label, $globenv, $locenv, $skipped);
            }
            next;
        }

        if ($compilerServerProc){
          # Some tests don't support hosted compiler. So in the case where
          # a hosted compiler has been started, but the current test has the tag "NoHostedCompiler",
          # revert back to exe compiler
          if($test->[0] =~ /(^|,)NoHostedCompiler($|,)/i) {
            $ENV{HOSTED_COMPILER} = "0"
          } else {
            $ENV{HOSTED_COMPILER} = "1"
          }
        }      

        if (($skipped = check_test($test, $globenv)) != 1) {
            # NOTE: check_resume can modify $resume_mode.
            if ($resume_mode) {
                check_resume($test->[2] . env_label($globenv->[3]));
            };

            # Never indicate skip if in resume mode - the previous log already
            # reflects that
            if ($noise > 0 && !$resume_mode) {
                if($skipped == "Skipping private test") {
                    print_skip($test->[2], $globenv->[3], undef, $skipped, 1);
                    save_the_log_file($SKIPPED, $test, $label, $globenv, $locenv, $skipped);
                    $totals_count[$SKIPPED]++;
                } else {
                    print_testerror($test->[2], $globenv->[3], undef, $skipped, 1);
                    save_the_log_file($TEST_ERROR, $test, $label, $globenv, $locenv, $skipped);
                    $totals_count[$TEST_ERROR]++;
                }
            }
            next;
        }

        # TODO comment, see above comment to understand what this comment needs to say
        # TODO this needs to be located AFTER check_test. Why???
        # TODO what about resume_mode?
        # TODO what about multiproc?
        if (($skipped = check_target()) != 1) {
            if ($resume_mode) {
               check_resume($test->[2] . env_label($globenv->[3]));
            };
            if ($noise > 0 && !$resume_mode) {
                $skipped = "test not run for target '$target'";
                print_skip($test->[2], $globenv->[3], undef, $skipped, 1);
            	save_the_log_file($SKIPPED, $test, $label, $globenv, $locenv, $skipped);
                $totals_count[$SKIPPED]++;
            }
            next;
        }

        @local_env_list = get_env_file($local_env_file);
        get_localkeepfiles() if ($noclobber);
        for $locenv (@local_env_list) {
    
            # 
            # Don't print duplicate dirs for -debug:list.
            #
            if ($debug eq 'list') {
                if ($testname_seen{$test->[2]} ne 1) {
	                $testname_seen{$test->[2]} = 1;
                } 
				else {
                	next;
                };
            };

            print_noise("\tstart LOCAL\n", 2);
            next unless check_local($test, $globenv, $locenv);

	    # Get a unique label
            $label = env_label($globenv->[3],$locenv->[3]);
	    $testlabel = $label;
	    $label = uniq_env_label($test->[2], $label);
	    if ($label ne $testlabel)
	    {
		# This means that we had to generate a unique label for this test
		# so we need to return a test error so someone will fix this.
		my $msg = "duplicate test environment";
		print_testerror($test->[2], $globenv->[3], $locenv->[3], $msg, 1);
		save_the_log_file($TEST_ERROR, $test, $label, $globenv, $locenv, $msg);
		$totals_count[$TEST_ERROR]++;
		next;
	    }

	    if ($permutation) {
		next unless $label =~ /^\s*\(\Q$permutation\E\)\s*$/;
	    }
			
            # Skip to next test if we're still resuming test run
           	unless (exists $knownfail{lc("$test->[2]$label")}) {
	            next if $resume_mode and check_resume("$test->[2]$label");
			}

			# TODO this needs to appear in the xml log file to be VCTR'd. Low pri as it's unlikely 
			# for a "real" run. Also, we don't have a test retval code for "not run" - fail is bad, so is skip 
            # REVIEW - needs to go before check_resume, lest we throw off resume mode?
            if (exists $knownfail{lc("$test->[2]$label")}) {
                unless ($resume_mode) {
					write_to_file (\$RESULTFILE, "# $test->[2]$label -- not run, known failure\n");
				}
				#save_the_log_file($retval, $test, $label, $globenv, $locenv, "not run, known failure");
                next;
            }
            
            my $results_filesize_before;
            if ($filelock) {
                $results_filesize_before = get_results_filesize();
            }
    
            %ORIG_LOC_ENV = apply_changes(%{$locenv->[4]});
			foreach my $hook (@prehook) {
	            run_hook($hook);
			}
            print_test_name($test->[2], $label);
            $retval = run_test();
            $ENV{'RUNALL_TEST_RESULT'} = $retval;
			foreach my $hook (@posthook) {
	            run_hook($hook);
			}
            $ENV{'RUNALL_TEST_RESULT'} = '';
            $totals_count[$retval]++;
            print_results($retval, "");
            dump_env($globenv->[4], $locenv->[4]) if ($debug =~ /^(fail|on)$/); 

            if ($retval =~ /^($FAILED|$CASCADE|$TIMED_OUT)$/) {
                print_fail_info($test->[2], $retval, $globenv, $locenv);
            }
    
            #if repro:all make repros even for passing tests
            if($genrepro eq 'all') {
                create_repro($test->[2], $retval, $globenv, $locenv);
            }
    
			save_the_log_file($retval, $test, $label, $globenv, $locenv, "");
    
            if (($retval =~ /^($PASSED|$SKIPPED|$NO_RESULT)$/)   
            || ($cleanup eq 'yes')
            || ($locenv->[3] ne '')) {
                cleanup_directory();
            }

            if ($retval =~ /^($FAILED|$CASCADE|$NO_RESULT|$TIMED_OUT|$TEST_ERROR)$/) {
				if (check_maxfails()) {
                	print_noise("Runall.pl line " . __LINE__ . ":  Saw $maxfails failures, stopping execution.", 0);
	                normal_exit();
				}
            	# VCQA:6436 repro:yes needs to make repros even for failing tests
	            if($genrepro eq 'yes') {
    	            create_repro($test->[2], $retval, $globenv, $locenv);
        	    }
			}
				
            if (check_maxtests()) {
                print_noise("Runall.pl line " . __LINE__ . ":  Ran $maxtests tests, stopping execution.", 0);
                normal_exit();
            }
            undo_changes(%ORIG_LOC_ENV);
    
            check_filelock($results_filesize_before) if ($filelock);
    
        } continue {
        print_noise("\tend LOCAL\n", 2);
        }
    } continue {
        print_noise("\tend TEST\n", 2);
		undef $label; # Not needed previously because we didn't save results for test_error and skip, etc.
    }
    undo_changes(%ORIG_GLOB_ENV);
    } continue {
	    print_noise("\tend GLOBAL\n", 2);
    }
}


#
# main test execution loop for the multi-processing
# mode of runall
#
# TODO
# *  devise some hack to prevent two long-running tests from appearing in the same batch (e.g. "expensive" ttag)
#
sub RunMultiProcess() {

    my $cur_test_id = 0;
    my $next_retired_test_id = 0;
    my ($i, $new_test_slot);
    my @execlist;         # list of tests currently executing
    my @waitlist;         # list of tests waiting for a dependent
    my @readylist;        # list of tests that were postponed, but are now ready
    my @procs;            # Win32::Process objects for currently executing tests

    my $FINISHED_NOTSTARTED = 0;	# test not started?
    my $FINISHED_EXECUTED = 1;		# test executed?
    my $FINISHED_TESTERROR = 2;		# test not run due to test_error, used to be runall_skipped
    my $FINISHED_NOTRUN = 3; 		# test not run due to ltags/gtags incompatibility, etc
	my $FINISHED_SKIPPED = 4;		# test skipped due to notarget.lst
    my @finished;         # status of finished test (index by test id)

    my $BATCH_SIZE = 2;   # maximum number of tests that are executed by one subprocess
    my %finished_batch;	  # map from test id to batchname
    my %batch_refcnt;     # map from batchname to the number of un-retired tests it still contains

    my %test_to_id;       # map a test name to all its indices in @testlist (semicolon separated)
    my $test;
    my @has_predecessors; # 1 if the test has (unresolved) predecessors
    my @has_successors;   # 1 if the others depend upon this test
    my %dependents;       # see read_dependence.

    my %failures_map;     # map from batchname to the result of parse_failures for that batch
    my %results_map;      # map from batchname to the result of parse_results for that batch

    my %skip_msg;         # messages associated with skipped tests

    my %is_expensive;     # map from test id to a 1 if the test is expensive

    my $cLoopActions;     # number of queue actions in this loop (used for deadlock detection)

    my @queueIdleTime;    # number of seconds each process is idle
    my @queueEmptyTime;   # time when each process became idle

    my @run_with;         # a semicolon separated list of each test the indexed test is run with

    leave("Runall.pl line " . __LINE__ . ":  Fatal error!\nResume not implemented for multi-processor runs \n", $SWITCH_ERROR_EXITVAL) if ($resume_mode);
   
	leave("Runall.pl line " . __LINE__ . ":  Fatal error!\n-env switch not implemented for multi-processor runs\n", $SWITCH_ERROR_EXITVAL) if ($permutation);
   
    leave("Runall.pl line " . __LINE__ . ":  Fatal error!\n-maxtests not implemented for multi-processor runs\n", $SWITCH_ERROR_EXITVAL) if ( $maxtests != -1 );
    
    open MPDEBUG, ">MPDEBUG.LOG" if ($mpdebug > 0);

    sub mp_log($$) {
	return unless ($mpdebug > 0);
	print MPDEBUG $_[0] if ($_[1] < $mpdebug);
    };

    # determine whether the queues are consistent
    #
    #
    sub check_queues_consistency() {
	my $prev = -1;
	my $x;
	foreach $x (sort @waitlist) {
	    if ($x == $prev) {
		mp_log("QUEUE ERROR! duplicate $x in waitlist\n",1);
	    };
	    $prev = $x;
	};
	$prev = -1;
	foreach $x (sort @readylist) {
	    if ($x == $prev) {
		mp_log("QUEUE ERROR! duplicate $x in readylist\n",1);
	    };
	    $prev = $x;
	};
	$prev = -1;
	foreach $x (sort (@readylist, @waitlist)) {
	    if ($x == $prev) {
		mp_log("QUEUE ERROR! duplicate $x in readylist U waitlist\n",1);
	    };
	    $prev = $x;
	};
    }

    # unblock the dependents of the given test
    #
    #
    sub unblock_dependents($) {
	my $test_id = $_[0];

	mp_log( "UNBLOCKING DEPENDENTS of  $test_id has succ: $has_successors[$test_id]\n", 5);
	delete $dependents{$testlist[$test_id]->[2]};

	# if we're retiring a test with successors, remove it from all 
	# dependence lists
	if (defined($has_successors[$test_id])) {

	    my $dep_test_id;
	    foreach $dep_test_id (keys %dependents) {
		if ($dependents{$dep_test_id} =~ /$test_id/) {
		    my $pre = length($dependents{$dep_test_id});
		    $dependents{$dep_test_id} = join(';', grep { ($test_id != $_); } split(/;/, $dependents{$dep_test_id}) );
		    mp_log( "PARTIALLY UNBLOCKED $dep_test_id: $dependents{$dep_test_id}\n", 6);
		    my $post = length($dependents{$dep_test_id});
		    leave ("Runall.pl line " . __LINE__ . ": Assert! pre $pre post $post\n", $STATE_ERROR_EXITVAL) if ($post > $pre);
		};

		# if we unblocked a test, reset its entry in $has_predecessors and
		# move it from the waitlist to the readylist (if it is in the waitlist). 
		if ($dependents{$dep_test_id} =~ /^;*$/) {
		    my @newly_unblocked = ();
		    mp_log( "UNBLOCKED $dep_test_id\n", 4);
		    $has_predecessors[$dep_test_id] = 0;
		    my $wait_test;
		    for ($wait_test = $#waitlist; $wait_test > -1; $wait_test--) {
			if ($dep_test_id == $waitlist[$wait_test]) {
			    push (@newly_unblocked, $waitlist[$wait_test] );
			    splice @waitlist, $wait_test, 1;
			}
		    };
		    unshift ( @readylist, @newly_unblocked );
		    $cLoopActions++;
		};

	    };
	};
	check_queues_consistency();	
    };

    sub add_queue_idle($) {
	my $exec_slot = $_[0];
	my $idleTime = time - $queueEmptyTime[$exec_slot];
	$queueIdleTime[$exec_slot] += $idleTime;
	$queueEmptyTime[$exec_slot] = -1;
	if ( $idleTime > 2 ) {
	    mp_log( "Queue $exec_slot idled for $idleTime\n", 3);
	};
    };

    leave ("Runall.pl line " . __LINE__ . ": Must have Win32 package to use multiple processes\n", $SWITCH_ERROR_EXITVAL) unless ( defined $have_mod{"Win32\\Process.pm"} );

    # build the test name to test id mapping
    $i = 0;
    for ($i = $#testlist; $i > -1; $i--) {
	if (defined($test_to_id{$testlist[$i]->[2]})) {
	    @test_to_id{$testlist[$i]->[2]} = 
	    join(';', ($i, split(/;/, @test_to_id{$testlist[$i]->[2]}) ) ) ;
	} else {
	    $test_to_id{$testlist[$i]->[2]} = $i;
	};
    };

    # read the dependence list
    %dependents = read_dependence("$root\\dependence.lst", \%test_to_id);

    # Since we do cleanup recursively, nake tests that are
    # subdirectories of other tests dependent.
    for ($i = 0; $i <= $#testlist; $i++) {
	my $parentdir = "";
	my @pathcomps = split( /\\/, $testlist[$i]->[2] );
	pop @pathcomps; # only parent directories
	foreach my $comp ( @pathcomps ) {
	    $parentdir .= "\\" unless ($parentdir eq "");
	    $parentdir .= $comp;

	    my $parent_id = $test_to_id{ $parentdir };
	    if ( $parent_id ) {
		my $cur_id = $test_to_id{ $testlist[$i]->[2] };
		# Don't let subdirectory become dependent on parent if parent is already related to subdirectory in dependence.lst
		if ( $dependents{ $parent_id } !~ /$cur_id/ ) {	
			if ( $dependents{ $cur_id  } !~ /$parent_id/ ) {
		    	$dependents{ $cur_id } .= ";".$parent_id;
			    mp_log ("Adding subdirectory dependence: $testlist[$i]->[2] ($cur_id) -> $parentdir ($parent_id)\n", 2);
			} else {
			    mp_log ("Subdirectory dependence already exists: $testlist[$i]->[2] ($cur_id) -> $parentdir ($parent_id)\n", 2);
			};
		} else {
			mp_log ("Parent directory $parentdir ($parent_id) is already dependent on subdirectory $testlist[$i]->[2] ($cur_id): not creating circular dependency\n", 2);
	    };
	};
    };
    };
    
    # build up the has_predecessors and has_successors arrays
    foreach $test (keys %dependents) {
	$has_predecessors[$test] = 1;
	foreach $i (split(/;/, $dependents{$test})) {
	    $has_successors[$i] = 1;
	};
    };

    # regurgitate the dependence list for debugging purposes
    mp_log( "##################################################\n", 4);
    mp_log( "#####   DEPENDENTS ###############################\n", 4);
    mp_log( "##################################################\n", 4);

    foreach $test (keys %dependents) {
	mp_log ($testlist[$test]->[2]."(".$test.")\n", 1);
	foreach $i (split(/;/, $dependents{$test})) {
	    mp_log ("\t".$testlist[$i]->[2]."(".$i.")\n", 1);
	};
    };

    # determine which tests are "expensive"
    #
    # the complication here has to do with dependencies. since
    # we want expensive test to run first, all the predecessors
    # of an expensive test must also become expensive
    my @explist;
    for ($i = 0; $i <= $#testlist; $i++) {
	if ($testlist[$i]->[0] =~ /expensive/) {
	    push @explist, $i;
	};
    };
    while ($#explist >= 0) {
	my $exptest = pop @explist;
	if (!$is_expensive{$exptest}) {
	    $is_expensive{$exptest} = 1;
	    mp_log( "EXPENSIVE: $testlist[$exptest]->[2]\n", 3 );
	    foreach $i (split(/;/, $dependents{$exptest})) {
		push @explist, $i;
	    };
	};
    };

    rename_runall_files();
    open_files($resume_mode ? "resume" : "open");
    print_fail_header() unless $resume_mode;
    
    # open XML files for multiproc
    if ($xml && !$resume_mode) {
		my $uuid = gen_uuid();
		my $suite_name = get_suite_name();	
        add_test_results_element($suite_name, $uuid);
        add_runpl_log_element($suite_name, $uuid);
    }
	unless ($resume_mode || $multiproc_child) {
	    dump_run_info();
	}

    # Get the root of $fail_list. The code to parse switches adds extensions
    # onto the filename, so we have remove them;
    $fail_list =~ /(.*)\.lst$/;
    my $fail_list_root = $1;
    
    # initialize the execution list
    for ($i = 0; $i < $nProcs; $i++) {
	$execlist[$i] = -1;
	$queueIdleTime[$i] = 0;
	$queueEmptyTime[$i] = time;
    };

    # initialize the finished list
    for ($i = 0; $i < $#testlist; $i++) { $finished[$i] = $FINISHED_NOTSTARTED; };

    mp_log("Main Loop Starting\n", 1);
    for $globenv (@global_env_list) { 

	next unless check_global($globenv);
	%ORIG_GLOB_ENV = apply_changes(%{$globenv->[4]});


	while (1) {

	    check_queues_consistency();

	    $cLoopActions = 0;

	    # check for finished subtests. 
	    for ($i = 0; $i < $nProcs; $i++) {

		next if ($execlist[$i] == -1);

		if ( $procs[$i]->Wait( 0 ) ) {
		    my $retval;
		    $procs[$i]->GetExitCode($retval);

		    $queueEmptyTime[$i] = time;

		    # remove the subtest list
		    mp_log ("unlinking $root\\test$execlist[$i].lst\n", 2);
		    verify_unlink ("$root\\test$execlist[$i].lst") || mp_log("WARNING: can't unlink $root\\test$execlist[$i].lst $!\n", 2);

		    mp_log( "Test $execlist[$i] has finished executing.\n",4);

		    # set finished for all the tests just finished. also
		    # setup the record of this batch
		    my $end_test_id;
		    $batch_refcnt{@execlist[$i]} = 0;
		    foreach $end_test_id (split(/,/, @execlist[$i])) {
			$finished[$end_test_id] = $FINISHED_EXECUTED;
			mp_log( "Test $end_test_id has finished executing.\n",4);
			$finished_batch{$end_test_id} = @execlist[$i];
			$batch_refcnt{@execlist[$i]}++;
			unblock_dependents($end_test_id);
			$cLoopActions++;
		    };

		    # read the results.log, failures.lst,env for the just-finished subtest

		    leave ("Runall.pl line " . __LINE__ . ": ASSERT: `$execlist[$i]$result_file' doesn't exist", $FILE_ERROR_EXITVAL) if (!(-e "$root\\$execlist[$i]$result_file"));
		    $results_map{$execlist[$i]} = parse_results("$root\\$execlist[$i]$result_file");

		    if ($xml)
		    {
			print_batch_results_xml("$root\\$execlist[$i]$result_file.xml");
			verify_unlink ("$root\\$execlist[$i]$result_file.xml");

			print_batch_runpl_xml("$root\\$execlist[$i]$runpl_log.xml");
			verify_unlink ("$root\\$execlist[$i]$runpl_log.xml");
		    }
		    
		    if (-e "$root\\$execlist[$i]$fail_list") {
			$failures_map{$execlist[$i]} = parse_failures( "$root\\$execlist[$i]$fail_list", "$root\\$execlist[$i]$fail_env" );
			mp_log("got ".$#{ $failures_map{$execlist[$i]} }." failures for ".$execlist[$i]."\n", 2);
		    };

		    $execlist[$i] = -1;
		};
	    };

	    # retire any skipped tests
	    while ($finished[$next_retired_test_id] > $FINISHED_EXECUTED) { 

		$test = $testlist[$next_retired_test_id];

		if ($finished[$next_retired_test_id] == $FINISHED_TESTERROR) {
		    if ($noise > 0 && !$resume_mode) {
			print_testerror($test->[2], $globenv->[3], undef, $skip_msg{$next_retired_test_id}, 1);
			$totals_count[$TEST_ERROR]++;
		    }
		};

		# TODO REVIEW: Added this to fix counting issues in multiproc after separating F_ERROR and F_SKIPPED
		if ($finished[$next_retired_test_id] == $FINISHED_SKIPPED) {
		    if ($noise > 0 && !$resume_mode) {
			print_skip($test->[2], $globenv->[3], undef, $skip_msg{$next_retired_test_id}, 1);
			$totals_count[$SKIPPED]++;
		    }
		};

		if ($finished[$next_retired_test_id] == $FINISHED_NOTRUN) {
		    if ($noise > 1) {
			print_skip($test->[2], $globenv->[3], undef, $skip_msg{$next_retired_test_id});
		    };
		};

		$next_retired_test_id++;
		$cLoopActions++;
	    };

	    # retire tests that actually executed
	    #
	    # REVIEW: runpl.log's order is not guaranteed to be the same
	    # as result.log's when $cBatchMax > 1.
	    while ($finished[$next_retired_test_id] == $FINISHED_EXECUTED) {

		my $retired_test_id = $next_retired_test_id;
		$next_retired_test_id++;

		# determine whether the test was part of a batch.
		my $batchname = $retired_test_id;
		if (defined($finished_batch{$retired_test_id}))
		{
		    $batchname = $finished_batch{$retired_test_id};
		    $batch_refcnt{$batchname}--;
		};

		# print out the results and failures
		#
		# loop through the results map until we find the results associated with this batch.
		# if the test failed, look the failures up in the failures map and print them out.
		my $failindex = 0;
		leave("Runall.pl line " . __LINE__ . ": ASSERT: no results for $batchname!", $STATE_ERROR_EXITVAL) if ( !defined($results_map{$batchname}) );

		for (my $i = 0; $i <= $#{ $results_map{$batchname} }; $i++) {

		    my $result_record = ${ $results_map{$batchname} }[$i];

		    next unless ($result_record->[1] eq $testlist[$retired_test_id]->[2]);

		    $totals_count[$result_record->[0]]++;

		    my $label;
		    $label = " ($result_record->[2])" if ($result_record->[2] ne "");

		    #my $resultstr = $result_record->[1];
		    #$resultstr .= $label;
		    #$resultstr .= " -- ".$RETVAL_TO_NAMES{$result_record->[0]};
		    #$resultstr .= " ".$result_record->[3] if ($result_record->[3] ne "");
		    #$resultstr .= "\n";
		    #write_to_file( \$RESULTFILE,  $resultstr );
		    #print $resultstr;
		    
                    if ($result_record->[0] =~ /^($FAILED|$CASCADE|$NO_RESULT|$TIMED_OUT|$TEST_ERROR)$/) {
			if (check_maxfails()) {
                	    print_noise("Runall.pl line " . __LINE__ . ":  Saw $maxfails failures, stopping execution.", 0);
	                    normal_exit();
			}
		    }
				
		    # TODO This seems wrong...$TEST_ERROR going to &print_skip instead of &print_testerror
		    # Verified that it still does test_error, though so it's not going to get fixed now.
		    if ( ($result_record->[0] == $TEST_ERROR) ) {
				print_testerror( $result_record->[1], $globenv->[3], $result_record->[2], $result_record->[3] );
		    } else {
			print_test_name( $result_record->[1], $label );
			print_results( $result_record->[0], $result_record->[3] );
		    };

		    if (($result_record->[0] == $FAILED) || ($result_record->[0] == $CASCADE) || ($result_record->[0] == $TIMED_OUT) ) {

				# print out the "fails"
				leave("Runall.pl line " . __LINE__ . ": ASSERT: no failures for $batchname!", $STATE_ERROR_EXITVAL) if ( !defined($failures_map{$batchname}) );
	
				# This section gets invoked when we have failing tests in a multiproc run. It doesn't appear that it was well tested 
				# because there are a lot of illegal Perl statements. The first is an undefined value as an array ref so I added a 
				# null dereference guard. The second is evaluation of a non-scalar reference in a scalar context which I think we
				# wanted to be a count of the failures for this batchname in the failures_map. We appear to be iterating through the
				# set of failure entries in this batch's map until the test name in the fail_record is the current testname.
				# APardoe 3 March 2005. 
				my $fail_record;
				# Null guard for this section added for VCQA:5796, runall crash "undefined value as an ARRAY reference"
				eval { # Perl equivalent of try with empty catch
					do {
						mp_log("FAIL: $failindex (for $batchname)\n",2);
						if ($failindex > scalar @{ $failures_map{$batchname} } ) {
							leave("Runall.pl line " . __LINE__ . ": ASSERT: $batchname batch failures don't contain entry for $result_record->[1]\n", $STATE_ERROR_EXITVAL);
						}
						mp_log("$failindex: " . join (',', @{ ${ $failures_map{$batchname} }[$failindex] } ) . "  \n", 2); # Runall crash: "Can't use undefined value as ARRAY reference."
						# $fail_record = ${ $failures_map{$batchname} }[$failindex]; # Runall crash: "Not a SCALAR reference". 
						$fail_record = @{ $failures_map{$batchname} }[$failindex];
						$failindex++;
					} until ($fail_record->[2] eq $testlist[$retired_test_id]->[2]);
					mp_log("FOUND: $fail_record->[2] \n",2);
				};
				if ($@) {
					mp_log("ERROR: Runall had undefined value in the eval block of RunallMultiProcess()",0);
				}
				print_fail_info ($testlist[$retired_test_id]->[2], $result_record->[0], $globenv, $fail_record->[4]);
			};
		};

		# print out the runpl.log
		my $SUBTEST_RUNPLFILE;
		if (-e "$root\\$batchname$runpl_log") {
			$SUBTEST_RUNPLFILE = get_filehandle("<", "$root\\$batchname$runpl_log");
		    #while (<SUBTEST_RUNPLFILE>) { write_to_file( \$RUNPLFILE, $_ ); };
			#print RUNPLFILE "\n" unless $_ =~ m/\n$/; # make sure new tests start on a new line
			my @local_runpl_text = <$SUBTEST_RUNPLFILE>;
			my $local_runpl_size = scalar @local_runpl_text;
		    close $SUBTEST_RUNPLFILE;
			#if ($xml) {
			#	$errtext .= join "", @local_runpl_text;
			#	$lines_seen += $local_runpl_size;
			#}
			write_to_file ( \$RUNPLFILE, join ("", @local_runpl_text) );
 			write_to_file ( \$RUNPLFILE, "\n" ) unless @local_runpl_text[$local_runpl_size - 1] =~ m/\n$/; 
		    verify_unlink ("$root\\$batchname$runpl_log");
		}

		mp_log ("batch refcount for $batchname: $batch_refcnt{$batchname}\n", 4);
		if ( ($batch_refcnt{$batchname} == 0) || ($batchname eq $retired_test_id) ) {
		    delete $failures_map{$batchname};
		    delete $results_map{$batchname};

		    # note: these files could be deleted right after the test results are parsed,
		    # but for debugging purposes it's better to keep them around longer
		    verify_unlink ("$root\\$batchname$result_file") || mp_log("can't unlink $root\\$batchname$result_file $!",1);
		    verify_unlink ("$root\\$batchname$fail_list") || mp_log("can't unlink $root\\$batchname$fail_list $!",1);
		    verify_unlink ("$root\\$batchname$fail_env") || mp_log("can't unlink $root\\$batchname$fail_env $!",1);
		    mp_log("Finished unlinking $batchname$result_file $batchname$fail_list $batchname$fail_env\n", 1);
		};

		$cLoopActions++;

		mp_log( "Test $retired_test_id has retired.\n", 4);
	    };

	    # determine whether we have space for a new test process
	    for ($new_test_slot = 0; $new_test_slot < $nProcs; $new_test_slot++) {
		last if ($execlist[$new_test_slot] == -1);
	    };

	    my $sum = 0;
	    map { $sum += $_; } @execlist;

	    if ($mpdebug) {
		my $currently_running = join(';', @execlist);
		$currently_running =~ s/;/,/g;
		for $test (split(/,/, $currently_running)) {
		    $run_with[$test] = union( $run_with[$test], $currently_running ) unless ($test == -1);
		};
		mp_log( "runninglist ".join(',', @execlist)."\n", 5);
		mp_log( "waitlist ".join(',', @waitlist)."\n", 5);
		my $readymax = $#readylist;
		$readymax = 50 if ($readymax > 50);
		mp_log( "readylist ".join(',', @readylist[0 ... $readymax])."\n", 5);
		mp_log( "next test to be retired: $next_retired_test_id\n", 5);
	    };

	    # are we finished?
	    if ($next_retired_test_id > $#testlist) {
		leave("Runall.pl line " . __LINE__ . ": ASSERT: finishing with work left to do!", $STATE_ERROR_EXITVAL) if ( ($sum != -1*$nProcs) || ($#readylist>=0) || ($#waitlist>=0) );

		for ($i = 0; $i < $nProcs; $i++) {
		    add_queue_idle( $i ) if ( $queueEmptyTime[$i] != -1 );
		};

		last;
	    }

	    # search for the next available test to run, add it to the readylist
	    for (; ($cur_test_id <= $#testlist); $cur_test_id++) {

		last if ($#readylist >= 10000);

		$test = $testlist[$cur_test_id];

		# REVIEW - we are unblocking the dependents of tests that aren't
		#          run due to tags or architecture or whatnot. This should
		#          probably be an error instead.

		# REVIEW - moved the following check_tags call out of check_test because it is 
		# not an "error" condition. This throws off resumed test runs.
		if ($skipped = check_tags($test->[0],union($ttags,$globenv->[2]))) {
		    mp_log($test->[2]." ($cur_test_id) has skipped due to ttags '$test->[0]' vs. '$ttags'.\n",4);
		    $skip_msg{$cur_test_id} = $skipped;
		    $finished[$cur_test_id] = $FINISHED_NOTRUN;
		    unblock_dependents($cur_test_id); 
		    next;
		}
		if (($skipped = check_test($test, $globenv)) != 1) {
		    mp_log($test->[2]." ($cur_test_id) has skipped.\n",4);
		    $skip_msg{$cur_test_id} = $skipped;
            if($skipped == "Skipping private test") {
                $finished[$cur_test_id] = $FINISHED_SKIPPED;
            } else {
		        $finished[$cur_test_id] = $FINISHED_TESTERROR;
            }
		    unblock_dependents($cur_test_id);
		    next;
		};
		# TODO added this for separating notarget.lst from previous runall_skipped cases
	    if (check_target() != 1) {
			$skipped = "test not run for target '$target'";
		    mp_log($test->[2]." ($cur_test_id) has skipped.\n",4);
		    $skip_msg{$cur_test_id} = $skipped;
		    $finished[$cur_test_id] = $FINISHED_SKIPPED;
		    unblock_dependents($cur_test_id);
		    next;
	    }
		
		if ($has_predecessors[$cur_test_id]) {
		    mp_log($testlist[$cur_test_id]->[2]." ($cur_test_id) has unresolved predecessors.\n",4);
		    push(@waitlist, $cur_test_id);
		    next;
		};


		push (@readylist, $cur_test_id);
		mp_log($testlist[$cur_test_id]->[2]." ($cur_test_id) added to the ready list\n",4);
	    };

	    # if we have an available process, run a new test.
	    # otherwise, sleep and restart the loop.
	    if ( ($new_test_slot == $nProcs) || ($#readylist<0) ) {
			Win32::Sleep(100);
	    } else {
	
			my @start_subtest_list = ();
			my $start_subtest_name = "";
	
			next unless ($#readylist >= 0);
	
			if ( defined($is_expensive{$readylist[0]}) ) {
			    push ( @start_subtest_list, shift(@readylist) );
			} else {
			    # sort the readylist so that the expensive test go to the front
			    #
			    # if perf is an issue, maintain the expensive ready list incrementally
			    my @explist = ();
			    for ($i = 0; $i <= $#readylist; $i++) {
				if ( $is_expensive{$readylist[$i]} == 1 ) {
				    push @explist, $readylist[$i];
				    splice @readylist, $i, 1;
				};
			    };
	
			    mp_log("EXPENSIVE ".join(',', @explist)."\n", 3);
	
			    if ($#explist >= 0) {
				unshift @readylist, @explist;
				push ( @start_subtest_list, shift(@readylist) );
			    } else {
				while ( ($#readylist >= 0) && ($#start_subtest_list < ($cBatchMax-1) ) ) {
	
				    push ( @start_subtest_list, shift(@readylist) );
				};
			    };
			};
	
			$start_subtest_name = join(',', @start_subtest_list);
	
			chdir ($root);
	
			# write the test sublist
			# TODO need to write skipped tests, etc to this in order to get test_error in VCTR

			my $SUBTESTLST = get_filehandle(">", "$root\\test$start_subtest_name.lst");
			foreach $i (@start_subtest_list) {
	
			    $test = $testlist[$i];
	
			    if ($test->[0]) {
				$test->[0] =~ s/__ra_alltag__,//g;
				print $SUBTESTLST $test->[0]."\t" unless ($test->[0] eq '__ra_nomatch__');
			    };
			    if ($test->[1]) {
				print $SUBTESTLST $test->[1]."\t" unless ($test->[1] eq '__ra_nomatch__');
			    };
			    if ($test->[2]) {
				print $SUBTESTLST $test->[2]."\t# foo\n";
			    };
			};
			close $SUBTESTLST;
	
			# Just in case -- cleanup old temp files
			verify_unlink ("$start_subtest_name$fail_env");
			verify_unlink ("$start_subtest_name$fail_list");
			verify_unlink ("$start_subtest_name$runpl_log");
			verify_unlink ("$start_subtest_name$result_file");
			verify_unlink ("X$start_subtest_name$runpl_log");
	
			# execute runall on the test sublist
			# ($0 means "this program's name")
			my $subtestcmd = "$perl $0 -target:$target -test test$start_subtest_name.lst ";
	
			# deliberately omited switches: -terse
            if(defined $compilerServerPort){
               my $newPort = $compilerServerPort + $new_test_slot + 1;
               $subtestcmd .= "-port:$newPort ";
            }
			$subtestcmd .= "-procs:1 -child ";
			$subtestcmd .= "-fileprefix:$start_subtest_name ";
			$subtestcmd .= "-ttags:$ttags ";
			$subtestcmd .= "-nottags:$nottags ";
			$subtestcmd .= "-usage:$usage_filter ";
			$subtestcmd .= "-notusage:$usage_neg_filter ";
			$subtestcmd .= "-ltags:$ltags ";
			$subtestcmd .= "-global $global_env_file " if ($global_env_file ne "");
	
			$subtestcmd .= "-results $result_file " if ($result_file);
			$subtestcmd .= "-failures $fail_list_root " if ($fail_list_root);
			$subtestcmd .= "-log $runpl_log " if ($runpl_log);
			$subtestcmd .= "-savelog:$savelog " if ($savelog);
			$subtestcmd .= "-repro:$genrepro " if ($genrepro);
			$subtestcmd .= "-knownfail $knownfail " if ($knownfail);
			$subtestcmd .= "-debug:$debug " if ( ($debug ne 'off') and ($debug ne 0) );
		    
			$subtestcmd .= "-timeout:$timeout " if ($timeout > 0); 
			$subtestcmd .= "-timing:".$TIMING_VALS_TO_NAMES{$timing{'level'}}. " " unless ($timing{'level'} == 0);
			$subtestcmd .= "-xml:yes " if ($xml);
    		if (@prehook) {
				foreach my $hook (@prehook) {
					$subtestcmd .= "-prehook $hook ";
				}
		    }
    		if (@posthook) {
				foreach my $hook (@posthook) {
					$subtestcmd .= "-posthook $hook ";
				}
		    }
	
			mp_log("Executing subtest command: ".$subtestcmd."\n", 4);
			
			# APARDOE next four lines are debugging aid for multiproc runall
			#open SUBTESTTXT, ">>subtest.txt";
			#print SUBTESTTXT $subtestcmd."\n\n";
			#close SUBTESTTXT;
			#my $subtestcmd = "$perl e:\\compqa\\testenv\\bin\\newcmd.pl ";
			
			my $newproc;
			my $cp = Win32::Process::Create($newproc, $^X, "$subtestcmd", 1, &Win32::Process::NORMAL_PRIORITY_CLASS, '.');
			my $finished_status;
			if ($cp == 0) {
			    print "Could not CreateProcess for batch $start_subtest_name\n";
			    print "$newproc $^X GetLastError: ".Win32::GetLastError()."\n";
	
			    $finished_status = $FINISHED_NOTRUN;
			} else {
			    $procs[$new_test_slot] = $newproc;
	
			    $execlist[$new_test_slot] = $start_subtest_name;
			    mp_log("Started (cp=$cp) $execlist[$new_test_slot] (".$test->[2].") in slot $new_test_slot\n", 4);
			    $finished_status = $FINISHED_NOTSTARTED;
			};
	
			my $start_subtest_list;
			foreach $start_subtest_list (@start_subtest_list) {
			    mp_log("\t".$testlist[$start_subtest_list]->[2]." $start_subtest_list \n", 4);
			    $finished[$start_subtest_list] = $finished_status;
			};
	
			add_queue_idle( $new_test_slot );
	    };

	    loopend:
	    # do some final deadlock detection
	    my $sum;
	    map { $sum += $_; } @execlist;
	    if ( ($sum == -1*$nProcs ) && ($#readylist == -1) && ($#waitlist >= 0) && ($cur_test_id > $#testlist) && ($cLoopActions == 0) ) {
		leave("Runall.pl line " . __LINE__ . ": ASSERT: deadlock!\n", $STATE_ERROR_EXITVAL);
	    };
	}
	undo_changes(%ORIG_GLOB_ENV);
    } continue {
    }

	# Note that we don't get the runpl.log output for these because they weren't run by child processes
	fake_xml_for_skipped_tests(\%skip_msg, \%test_to_id) if ($xml);
	
    if ($mpdebug) {
	mp_log("Concurrency list:\n", 5);
	for ($i = 0; $i < $#run_with; $i++) {
	    mp_log( "$testlist[$i]->[2] ($i):\n", 5);
	    foreach $test (split(/,/, $run_with[$i])) {
		mp_log( "\t$testlist[$test]->[2] ($test):\n", 5);
	    };
	};
	mp_log("Queue idle time ".join(',', @queueIdleTime)."\n", 1);
    };
    close MPDEBUG if ($mpdebug);
}

#
# Common main code
# 

init_globals();
get_switches();

# Initialize cl throughput timing: runall start and /Bt switch
$ENV{'_CL_'} = '/Bt ' . $ENV{'_CL_'} if $timing{'level'} ge $TIMING_HIGH;
$timing{'runall'} = time if $timing{'level'} ge $TIMING_GLOBAL;

# $test_file variable is set to the parameter of -test. When running
# with -test failures.lst this condition sets the value of $global_env_file.
# $global_env_file must be set before @global_env_list is initialized.
if (-e $test_file) {
	if (-d $test_file) {
		$testlist[0] = ['*', '__ra_nomatch__', $test_file];
	} else {
		@testlist = read_file($test_file, 1);
	}
}
else {
	# Can't use leave() because leave() prints to the results log files and we haven't opened them yet
	print "Runall.pl line " . __LINE__ . ":  Fatal Error: $!\nFile '$test_file' does not exist!\n";
	exit($FILE_ERROR_EXITVAL);
}

@global_env_list = get_env_file($global_env_file);
%target_map = get_targetdef_file( "targets.lst", 1 ) if ( -e "targets.lst" );

launch_compiler_host();

if ($nProcs > 1) {
    RunMultiProcess();
} else {
    RunSingleProcess();    
};

kill_compiler_host();

unless ($debug) {

    print "\n\n";
    write_to_file( \$RESULTFILE,  "\n\n" );
    
    # print final totals
    my $grand_total = 0;
    for (0, 1, 3, 4, 5, 6) { # pass, fail, cascade, no result, timed out, test error
        $grand_total += $totals_count[$_];
    }

    for (0, 1, 3, 4, 5, 6) { # pass, fail, cascade, no result, timed out, test error
		if ($grand_total) {
		        my $pct_line;
		        $pct_line = sprintf( "%s tests %s (%.2f%)\n", $totals_count[$_], $totals_strings[$_], ($totals_count[$_] / $grand_total) * 100);
			print $pct_line;
			write_to_file( \$RESULTFILE, $pct_line );
		}
		else { # all tests skipped: no percentages
		        my $pct_line;
			$pct_line = sprintf( "%s tests %s (0%)\n", $totals_count[$_], $totals_strings[$_]);
			print $pct_line;
			write_to_file( \$RESULTFILE, $pct_line );
		}
    }
    print "========\n$grand_total Total executed tests\n\n";
    write_to_file( \$RESULTFILE, "========\n$grand_total Total executed tests\n\n" );

    for (2) { # skipped
		$grand_total += $totals_count[$_];
		print "$totals_count[$_] tests $totals_strings[$_]\n";
		write_to_file( \$RESULTFILE, "$totals_count[$_] tests $totals_strings[$_]\n");
    }
    print "========\n$grand_total Total tests (executed & skipped)\n\n";
    write_to_file( \$RESULTFILE, "========\n$grand_total Total tests (executed & skipped)\n\n" );
    
    # print total run time 
    if ($timing{'level'} ge $TIMING_GLOBAL) {
    my ($timing_output, @timepieces);
    @timepieces = gmtime(time - $timing{'runall'});
    $timing_output .= @timepieces[7] . 'd ' if @timepieces[7];
	$timing_output .= @timepieces[2] . 'h ' if @timepieces[2];
	$timing_output .= @timepieces[1] . 'm ' if @timepieces[1];
	$timing_output .= @timepieces[0] . "s Total time\n";
	print $timing_output;
	write_to_file( \$RESULTFILE,  $timing_output );        
    }
}

my $key;
if (keys %readonlylogs) {
    print "\nWARNING: The following test directories had read-only $file_prefix$runpl_log files:\n";
    write_to_file (\$RESULTFILE, "\nWARNING: The following test directories had read-only $file_prefix$runpl_log files:\n");
    for $key (keys %readonlylogs) {
        print "$key\n";
        write_to_file( \$RESULTFILE, "$key\n");
    }
}
if ($xml)
{
    if (keys %xmldups) {
        print "\nWARNING: The following tests had duplicate environment comments:\n";
		write_to_file( \$RESULTFILE, "\nWARNING: The following tests had duplicate environment comments:\n" );
        for $key (keys %xmldups) {
            $xmldups{$key}++;
			my $count = ((scalar $xmldups{$key})+1)/2;
            print "$key had $count instances\n";
            write_to_file(\$RESULTFILE, "$key had $count instances\n");
        }
    }

    # this check keeps the multiproc run from 
    # closing the results.xml with an exter </Test>
    # element 
    if ($nProcs == 1)
    { 
        close_test_element();
    }

    if (!$multiproc_child)
    {
        close_test_results_element();
        close_runpl_log_element() if (!$multiproc_child);
    }

    close(XMLRESULTFILE);
    close(XMLRUNPLFILE);
}

close_files();

chdir($root);

if ($xml) {
	if ($multiproc_child) {
		verify_unlink("$results_root\\$file_prefix$xmlrunpl_file");
		unless (rename("$results_root\\X$file_prefix$xmlrunpl_file", "$results_root\\$file_prefix$xmlrunpl_file")) {
			print "Rename of $xmlrunpl_file failed!\n";
		}
	}
	else {
		# If rename of the xml file fails then don't try to fix it up which will overwrite it with a blank file
		if (rename("$results_root\\$file_prefix$xmlresult_file", "$results_root\\X$file_prefix$xmlresult_file")) {
			fix_resultsxml("$results_root\\X$file_prefix$xmlresult_file", "$results_root\\$file_prefix$xmlresult_file");
		}
		else {
			print "Rename of $xmlresult_file failed!\n";
		}
		verify_unlink("$results_root\\$file_prefix$xmlrunpl_file");
		fix_runplxml("$results_root\\X$file_prefix$xmlrunpl_file", "$results_root\\$file_prefix$xmlrunpl_file");
	}
}

verify_unlink("$results_root\\$file_prefix$runpl_log");
unless (rename("$results_root\\X$file_prefix$runpl_log", "$results_root\\$file_prefix$runpl_log")) {
	print "Rename of $runpl_log file failed!\n";
}
verify_unlink ("$results_root\\$file_prefix$fail_list", "$results_root\\file_prefix$fail_env") 
    unless (grep($_ > 0, @totals_count[$FAILED, $CASCADE, $TIMED_OUT, $TEST_ERROR]) && ($debug == 0));

normal_exit();
