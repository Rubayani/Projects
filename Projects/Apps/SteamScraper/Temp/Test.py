with open("gamesChecked(Reviews).txt", "r", encoding="utf-8") as f:
    lines = [line.strip() for line in f if line.strip()]

lines.sort(
    key=lambda x: float(x.split("-")[-1].replace("%", "").strip()),
    reverse=True
)

with open("gamesChecked(Reviews)_sortedByScore.txt", "w", encoding="utf-8") as f:
    f.write("\n".join(lines))
