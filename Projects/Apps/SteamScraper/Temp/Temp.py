
# filename: extract_games.py

import re

# Input file with your steam accounts (replace with your actual filename)
input_file = "Temp.txt"
# Output file
output_file = "games_with_price2.txt"

games = []

with open(input_file, "r", encoding="utf-8") as f:
    for line in f:
        if("$" in line): 
            games.append(line)

r = []
for line in games:  # assuming results holds lines like "... | Paid | $39.99"
    parts = line.split("|")
    if len(parts) >= 3:
        game = parts[0].strip().replace("Checked:", "").strip()
        price_str = parts[2].strip().replace("$", "").replace(",", "")
        price = float(price_str)
        r.append((game,price))
        

r.sort(key=lambda x: x[1])


with open(output_file, "w", encoding="utf-8") as f:
    for line in r:  # assuming results holds lines like "... | Paid | $39.99"
        f.write(f"{line[0]} | {line[1]}" + "\n")
