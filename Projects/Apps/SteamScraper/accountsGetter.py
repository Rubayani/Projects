 # MOCK DATA ONLY — for testing. Do not use to deceive or impersonate.
import os, random, string, time

accountsOut = "newAccounts.txt"
gamesFile = "listOfGames.txt"
numAccounts = 25  # change as you like

countries = ["US","GB","AU","SA","DE","SE","FR","ES","IT","RU","CA","AL","BR","JP","KR","CN","IN","NL","PL","TR"]
curr = {"US":"$", "GB":"£", "AU":"A$", "SA":"SAR", "DE":"€", "SE":"kr", "FR":"€", "ES":"€", "IT":"€", "RU":"₽",
        "CA":"C$", "AL":"€", "BR":"R$", "JP":"¥", "KR":"₩", "CN":"¥", "IN":"₹", "NL":"€", "PL":"zł", "TR":"₺"}

adj = ["kinda","neo","dark","super","meta","ghost","rapid","alpha","omega","silent","pixel","quantum","vivid"]
noun = ["Phantom","Wolf","Byte","Raven","Nova","Hunter","Cipher","Matrix","Atlas","Drifter","Specter","Comet"]

def randUsername():
    base = random.choice(adj) + random.choice(noun)
    suffix_len = random.randint(1, 30)
    suffix = ''.join(random.choice(string.ascii_letters + string.digits) for _ in range(suffix_len))
    return base + suffix

def randPassword(n=10):
    alphabet = string.ascii_letters + string.digits + "!@#$%&*?"
    return "".join(random.choice(alphabet) for _ in range(n))

def randLastOnline():
    if random.random() < 0.7:
        hrs = random.randint(0, 48)
        mins = random.randint(0, 59)
        return f"{hrs} hrs, {mins} mins ago"
    else:
        days = random.randint(2, 120)
        return f"{days} days ago"

def loadGames():
    if not os.path.exists(gamesFile):
        # fallback sample games if file missing
        return ["Rust","Counter-Strike 2","PAYDAY 2","BeamNG.drive","Apex Legends","War Thunder","Destiny 2",
                "Team Fortress 2","Dota 2","The Escapists 2","Automation - The Car Company Tycoon Game"]
    with open(gamesFile, "r", encoding="utf-8") as f:
        return [g.strip() for g in f if g.strip()]

def pickGames(allGames):
    k = random.randint(12, min(60, len(allGames)))  # library size
    return sorted(random.sample(allGames, k))

allGames = loadGames()
seenUsernames = set()

with open(accountsOut, "w", encoding="utf-8") as out:
    for _ in range(numAccounts):
        # unique username
        u = randUsername()
        while u in seenUsernames:
            u = randUsername()
        seenUsernames.add(u)

        pwd = randPassword()
        country = random.choice(countries)
        symbol = curr.get(country, "$")
        # price style: sometimes 0.00, sometimes 0,--€
        if symbol == "€" and random.random() < 0.4:
            balance = "0,--€"
        elif symbol in ("SAR","₽","₩","¥","₹","₺","zł","kr","R$","C$"):
            balance = f"{symbol} {random.randint(0, 5000)}"
        else:
            balance = f"{symbol} {random.randint(0, 99)}.{random.randint(0, 99):02d}"

        status = random.choice(["Verified","Unverified","Verified","Verified","Locked"])
        lastOnline = randLastOnline()
        lib = pickGames(allGames)
        total = len(lib)

        gamesStr = " |  ".join(lib)
        line = (f"{u}:{pwd} | Status = {status} | Balance = {balance} | LastOnline = {lastOnline} | "
                f"Country = {country} | Total Games = {total} | Games = [{gamesStr}]")
        out.write(line + "\n")

print(f"Done. Wrote {numAccounts} mock accounts to {accountsOut}")
