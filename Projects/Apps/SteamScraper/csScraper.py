input_file = "accounts.txt"
output_file = "cs2_accounts.txt"

with open(input_file, "r", encoding="utf-8") as f:
    lines = f.readlines()

cs2_accounts = []

for line in lines:
    if "Counter-Strike 2" in line:  # check if CS2 is in games list
        try:
            userpass = line.split("|")[0].strip()  # take before first '|'
            cs2_accounts.append(userpass)
        except Exception:
            continue

# write results
with open(output_file, "a", encoding="utf-8") as f:
    for acc in cs2_accounts:
        f.write(acc + "\n")

print(f"Extracted {len(cs2_accounts)} accounts with CS2 into {output_file}")
