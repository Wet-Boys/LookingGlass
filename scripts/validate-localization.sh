#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
language_root="$repo_root/LookingGlass/Languages"
en_file="$language_root/en/LookingGlass.json"
zh_file="$language_root/zh-CN/LookingGlass.json"
package_file="$repo_root/Thunderstore/thunderstore.toml"

test -f "$en_file"
test -f "$zh_file"

python3 - "$en_file" "$zh_file" <<'PY'
import json
import sys

required_tokens = {
    "LG_TOKEN_MOD_DESCRIPTION",
    "LG_TOKEN_STATS_DISPLAY_DEFAULT_MAIN",
    "LG_TOKEN_STATS_DISPLAY_DEFAULT_SECONDARY",
    "LG_TOKEN_ITEMSTAT_PROC_COEFFICIENT",
    "LG_TOKEN_MISC_CORRUPTED_BY",
    "LG_TOKEN_PORTAL_NONE",
    "LG_TOKEN_BUFF_NAME_AffixRed",
}

def load(path):
    with open(path, encoding="utf-8-sig") as handle:
        data = json.load(handle)
    if not isinstance(data, dict):
        raise SystemExit(f"{path} must contain a JSON object")
    return data

en = load(sys.argv[1])
zh = load(sys.argv[2])

missing_en = sorted(required_tokens - en.keys())
missing_zh = sorted(required_tokens - zh.keys())
if missing_en or missing_zh:
    raise SystemExit(f"missing tokens: en={missing_en}, zh-CN={missing_zh}")

extra_zh = sorted(set(zh.keys()) - set(en.keys()))
missing_zh_all = sorted(set(en.keys()) - set(zh.keys()))
if extra_zh or missing_zh_all:
    raise SystemExit(f"token mismatch: extra zh-CN={extra_zh[:10]}, missing zh-CN={missing_zh_all[:10]}")
PY

grep -q 'Languages/en' "$package_file"
grep -q 'Languages/zh-CN' "$package_file"
