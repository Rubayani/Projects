import re

accounts_file = "accounts.txt"          # your full accounts list
target_games_file = "games_with_price2.txt" # list of games you're tracking
output_file = "accounts_with_targets.txt"

# Load target games
with open(target_games_file, "r", encoding="utf-8") as f:
    target_games = [line.split("|")[0].strip() for line in f if line.strip()]

print("🎯 Target games:", target_games)

results = []

with open(accounts_file, "r", encoding="utf-8") as f:
    for line in f:
        # extract Games = [ ... ]
        match = re.search(r"Games = \[(.*?)\]", line)
        if not match:
            continue

        games_str = match.group(1)
        account_games = [g.strip() for g in games_str.split("|")]

        # check if account has any target game
        found = [g for g in target_games if g in account_games]
        if found:
            results.append(line.strip())  # save whole account line

# Write matching accounts
with open(output_file, "w", encoding="utf-8") as f:
    f.write("\n".join(results))

print(f"✅ Found {len(results)} accounts with target games. Saved to {output_file}")
