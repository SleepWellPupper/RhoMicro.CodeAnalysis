# Grammar

```abnf
template                  = not.empty-template / empty-template
not-empty-template         = template-block-body
empty-template            = ;empty

block-sequence            = block *trivia-block
trivia-block             = trivia block
block                     = render-block / code-block / template-block

render-block              = render-block-head [render-block-body]
render-block-head         = open-render-block text close-render-block
open-render-block         = OPA CLN
close-render-block        = CLN CPA
render-block-body         = [trivia] template-block

template-block            = open-template-block template-block-body close-template-block
open-template-block       = OAB CLN
close-template-block      = CLN CAB
template-block-body       = template-block-body-child *template-block-body-child
template-block-body-child = text / block-sequence
text                      = text-child *text-child
text-child                = not-escaped-text / escaped-text
not-escaped-text          = ;any text that does not match (open-render-block / close-render-block / open-template-block / close-template-block / open-code-block / close-code-block)

code-block                = open-code-block code-body close-code-block
open-code-block           = OCB CLN
close-code-block          = CLN CCB
code-body                 = code-body-child *code-body-child
code-body-child           = text / render-block

escaped-text              = escaped-open-block / escaped-close-block / empty-block
empty-block               = open-block close-block ;empty blocks will always be rendered as escaped, i.e. there are no functional empty blocks
escaped-open-block        = open-block escape-colon
escaped-close-block       = escape-colon close-block
escape-colon              = CLN
open-block                = open-code-block / open-render-block / open-template-block
close-block               = close-code-block / close-render-block / close-template-block

trivia                    = LF / CR / (CR LF) *(SPC / TAB)

CLN                       = ":"
OCB                       = "{"
CCB                       = "}"
OPA                       = "("
CPA                       = ")"
OAB                       = "<"
CAB                       = ">"
LF                        = "\n"
CR                        = "\r"
SPC                       = " "
TAB                       = "\t"
```