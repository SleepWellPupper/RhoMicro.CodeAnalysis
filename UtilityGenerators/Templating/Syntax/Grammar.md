# Grammar

```abnf
template                  = not-empty-template / empty-template
not-empty-template        = template-block-body
empty-template            = ;empty

template-block            = [leading-trivia] open-template-block template-block-body close-template-block [trailing-trivia]
open-template-block       = OAB CLN
close-template-block      = CLN CAB
template-block-body       = template-block-body-child *template-block-body-child
template-block-body-child = render-block / code-block / text

text                      = text-child *text-child
text-child                = not-escaped-text / escaped-text
not-escaped-text          = not-escaped-text-child *not-escaped-text-child
not-escaped-text-child    = not-newline / newline / whitespaces
                            ; By explicitly recognizing newlines, we may
                              substitute them for localized newlines upon
                              rendering. This also improves testability as it
                              removes the requirement for specific newlines in
                              test files.
not-newline               = ; any text that does not match (open-render-block /
                              close-render-block / open-template-block /
                              close-template-block / open-code-block /
                              close-code-block / newline)

escaped-text              = escaped-open-block / escaped-close-block / empty-block
empty-block               = open-block close-block ; empty blocks will always be
                                                     rendered as escaped, i.e.
                                                     there are no functional empty
                                                     blocks
escaped-open-block        = open-block escape-colon
escaped-close-block       = escape-colon close-block
escape-colon              = CLN
open-block                = open-code-block / open-render-block / open-template-block
close-block               = close-code-block / close-render-block / close-template-block

render-block              = render-block-head [render-block-body]
render-block-head         = open-render-block text close-render-block
open-render-block         = OPA CLN
close-render-block        = CLN CPA
render-block-body         = [render-block-trivia] template-block
render-block-trivia       = newline

code-block                = [leading-trivia] open-code-block code-block-body close-code-block [trailing-trivia]
open-code-block           = OCB CLN
close-code-block          = CLN CCB
code-block-body           = code-block-body-child *code-block-body-child
code-block-body-child     = render-block / text

leading-trivia            = whitespaces
trailing-trivia           = newline

newline                   = LF / CR / (CR LF)
whitespace                = SPC / TAB
whitespaces               = *whitespace

lower-alpha               = %x61-%x7A

CLN                       = ":"
OCB                       = "{"
CCB                       = "}"
OPA                       = "("
CPA                       = ")"
OAB                       = "<"
CAB                       = ">"
OBR                       = "["
CBR                       = "]"
LF                        = "\n"
CR                        = "\r"
SPC                       = " "
TAB                       = "\t"
```
