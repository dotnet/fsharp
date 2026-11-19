---
name: Release notes to narrated slides
description: Turn a URL to written release notes into a short slide deck plus the script a presenter speaks over it. Inputs - release-notes URL, talk length in minutes.
---

# Release notes to narrated slides

Wrapper around [OnurGumus/codebase-video at commit 9deddc3](https://github.com/OnurGumus/codebase-video/tree/9deddc3f7b3a414650966850201b5da703367740). Download exactly that commit, not a branch head. Read its `SKILL.md` and engine notes, and use its setup, rendering and checks. Replace Claude-specific calls with your own. Do not copy it here.
It narrates with a synthetic voice. Here the presenter speaks, so you produce slides and a script, not audio.
To find release content, the [F# release announcement agent](fsharp-release-announcement.md) can run first.

**Inputs:** release-notes URL, minutes. Ask once with the ask tool: presenter name and team, audience, deep-dive topics, topics to skip, a one-line motivation, highlights of the previous release, and a design template (a style description, images or a URL). Never infer a team from a handle. Without answers, state your defaults.

**Output:** step-through slides (self-contained HTML plus PDF; no .pptx) and a script holding only the words to say, as speaker notes and a separate file. Show a rendered draft before the presenter records.

## Rules for release notes

- **Length:** about 30 s per slide, 1.8 spoken words per second, within the limit. Say the estimate is unverified until the presenter records.
- **Slide 1:** thanks; name and team only if known. One line on the preceding preview or RC of the same product with "go read those notes"; verify it, keep the presenter's claim, flag mismatches in the report. End with the agenda in one sentence.
- **Story:** lead with the benefit. One beat per slide, headline claim or number in the title, a one-sentence bridge at the end of each note. Depth on the presenter's topics; everything else on one "also" slide. Housekeeping goes in a footnote.
- **Evidence:** follow linked articles, tables and charts that back a slide. Show tables complete, including unchanged rows. Compile and run every code sample you show on the SDK the notes name, else mark it unverified. Show real compiler errors. Never retype numbers or code.
- **Narration:** say what the team did, then the range, never cherry-picked points. Caveats go on the slide, not in speech. Say preview, prototype, or not production once per topic. Separate what ships from copy-from-repo prototypes. Never promise speed; say "experiment and measure".
- **Voice:** natural first person, motivation before mechanism, the presenter's framing in plain terms. Do not invent technical claims.
- **Template:** follow its rules on type size and colour. It may be confidential: keep derived art, fonts and colours out of the repository.
- **Check:** render and look at every slide. Reject overflow, off-slide content, and text under the template's minimum (24 px at 1920 px wide if none).
- **Finish with a report:** recap verified, samples run (N/N), slides visually checked, claims you could not verify.
