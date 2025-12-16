import sqlite3, secrets, string, random, sys, time
from pathlib import Path

DB_PATH = Path("accounts.db")
TABLE = "accounts"

# ---- Config ----
DEFAULT_COUNT = 152561  # or pass as CLI arg
USERNAME_MIN, USERNAME_MAX = 6, 20

# Pools for usernames
ADJS = ["rapid","silent","ember","lunar","feral","scarlet","ashen","vivid","arcane","brisk","primal","tidal","polar","bold"]
NOUNS = ["falcon","willow","apex","harbor","nebula","glyph","raven","pylon","cipher","lotus","hydra","vector","nexus","thistle"]

# ---- Password generator ----
def make_password():
    """Mix of easy, medium, and strong passwords."""
    def style1():
        words = ["dragon","hunter","shadow","sunshine","coffee","winter","storm","flower","secret","mango","storm"]
        return secrets.choice(words) + str(secrets.randbelow(9999))

    def style2():
        words = ["love","blue","happy","dark","light","master","nova","angel","devil"]
        return secrets.choice(words).capitalize() + secrets.choice("!@#$%&*?")

    def style3():
        names = ["John","Anna","Mike","Sara","Ali","Omar","Lina","David","Nora","Leo"]
        return secrets.choice(names) + str(random.randint(1990, 2025))

    def style4():
        return "".join(secrets.choice(string.ascii_letters + string.digits)
                       for _ in range(random.randint(8, 12)))

    def style5():
        pool = string.ascii_letters + string.digits + "!@#$%^&*()-_=+"
        return "".join(secrets.choice(pool) for _ in range(random.randint(12, 20)))

    styles = [style1, style2, style3, style4, style5]
    choice = random.choices(styles, weights=[3, 2, 2, 2, 1])[0]
    return choice()

# ---- Username generator ----
def make_username(existing):
    for _ in range(100):
        gen = f"{secrets.choice(ADJS)}{secrets.choice(NOUNS)}{secrets.randbelow(10000)}"
        if gen not in existing:
            existing.add(gen)
            return gen
    # fallback
    gen = secrets.token_hex(6)
    existing.add(gen)
    return gen

# ---- DB setup ----
def prepare_db(path: Path):
    con = sqlite3.connect(path)
    cur = con.cursor()
    cur.execute(f"""
        CREATE TABLE IF NOT EXISTS {TABLE}(
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            username TEXT UNIQUE NOT NULL,
            password TEXT NOT NULL
        );
    """)
    con.commit()
    return con

# ---- Progress bar ----
def progress_bar(duration, total_steps=50):
    start = time.time()
    while True:
        elapsed = time.time() - start
        progress = min(elapsed / duration, 1.0)
        filled = int(total_steps * progress)
        bar = "█" * filled + "-" * (total_steps - filled)
        sys.stdout.write(f"\r |{bar}| {int(progress*100)}%")
        sys.stdout.flush()
        if progress >= 1:
            break
        time.sleep(0.2)
    print()

# ---- Main ----
def main():
    count = DEFAULT_COUNT
    if len(sys.argv) >= 2:
        try:
            val = int(sys.argv[1])
            if 5000 <= val <= 10000:
                count = val
        except:
            pass

    # Random total duration
    total_time = random.randint(70, 300)
    progress_bar(1)

    print("Creating database and inserting accounts...")

    con = prepare_db(DB_PATH)
    cur = con.cursor()
    existing = set()

    for _ in range(count):
        u = make_username(existing)
        p = make_password()
        cur.execute(f"INSERT OR IGNORE INTO {TABLE}(username, password) VALUES (?,?)", (u, p))

    con.commit()
    total = cur.execute(f"SELECT COUNT(*) FROM {TABLE}").fetchone()[0]
    con.close()
    print(f"✅ Done. {total} accounts saved into {DB_PATH}")

if __name__ == "__main__":
    main()
