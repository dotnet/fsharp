NAME=policy.2.3.FSharp.Core
ASSEMBLY = $(NAME).dll
DELAY_SIGN=1
TOKEN=$(FSCORE_DELAY_SIGN_TOKEN)

include ../config.make

# override the targets to build the assembly

$(outdir)$(NAME).dll: $(NAME).dll.config 
	@mkdir -p $(@D)
	cp $(NAME).dll.config $(@D)
	$(monobindir)/al /link:$(NAME).dll.config /out:$@ /delaysign /keyfile:$(topdir)mono/msfinal.pub

build: 
	$(MAKE) $(outdir)$(NAME).dll

clean: 
	-rm -f $(outdir)$(NAME).dll 

install: install-sdk-lib install-gac-lib

