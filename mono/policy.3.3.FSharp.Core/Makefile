NAME=policy.3.3.FSharp.Core
ASSEMBLY = $(NAME).dll
DELAY_SIGN=1
TOKEN=$(FSCORE_DELAY_SIGN_TOKEN)

include ../config.make

$(outdir)$(NAME).dll: $(NAME).dll.config 
	@mkdir -p $(@D)
	cp $(NAME).dll.config $(@D)
	$(monobindir)/al /link:$(NAME).dll.config /out:$@ /delaysign /keyfile:$(topdir)mono/msfinal.pub

build: 
	$(MAKE) $(outdir)$(NAME).dll

clean: 
	-rm -f $(outdir)$(NAME).dll 

install: install-sdk-lib install-gac-lib