#!/usr/bin/env bash
cmd=$(jq -r '.tool_input.command // ""')
if grep -qE '(git[[:space:]]+commit|gh[[:space:]]+pr)' <<<"$cmd" \
  && grep -qiE 'co-authored-by:.*(claude|anthropic)|generated with \[?claude code' <<<"$cmd"; then
  echo "Blocked: commits and PRs must not credit Claude. Remove the Co-Authored-By / 'Generated with Claude Code' lines and retry." >&2
  exit 2
fi
exit 0
