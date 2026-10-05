---
models:
  # Prevent the proxy's GPT-5 family alias from replacing the selected Sol model.
  gpt-5.6-sol: [copilot/gpt-5.6-sol, openai/gpt-5.6-sol]

engine:
  id: copilot
  model: ${{ (github.job == 'detection' && vars.GH_AW_MODEL_DETECTION_COPILOT) || vars.GH_AW_MODEL_AGENT_COPILOT || 'gpt-5.6-sol' }}
  env:
    GH_AW_REASONING_EFFORT: ${{ (github.job == 'detection' && vars.GH_AW_REASONING_EFFORT_DETECTION) || vars.GH_AW_REASONING_EFFORT || 'high' }}
  args: ["--reasoning-effort", "${{ env.GH_AW_REASONING_EFFORT }}"]
---

<!--
Shared model defaults for all agentic workflows, including threat detection.
Override GH_AW_MODEL_AGENT_COPILOT or GH_AW_REASONING_EFFORT in repository variables.
GH_AW_MODEL_DETECTION_COPILOT and GH_AW_REASONING_EFFORT_DETECTION override detection only.
When selecting another GPT-5 variant, add its exact alias under models to prevent proxy substitution.
After changing this file, run gh aw compile to regenerate every lock file.
-->
