import sqlite3, time, random
from colorama import init, Fore, Style

# Initialize colorama (for Windows CMD/PowerShell)
init(autoreset=True)

def extract_accounts(db_file="accounts.db"):
    try:
        con = sqlite3.connect(db_file)
        cur = con.cursor()

        cur.execute("SELECT username, password FROM accounts")
        rows = cur.fetchall()

        valid_count = unvalid_count = fa_count = 0

        for username, password in rows:
            roll = random.randint(1, 100)

            if roll == 1:  # 1% chance
                line = f"{Fore.GREEN}{username} : {password} -> Valid{Style.RESET_ALL}"
                valid_count += 1
            elif roll <= 11:  # next 10% chance
                line = f"{Fore.YELLOW}{username} : {password} -> 2FA{Style.RESET_ALL}"
                fa_count += 1
            else:  # rest
                line = f"{Fore.RED}{username} : {password} -> Unvalid{Style.RESET_ALL}"
                unvalid_count += 1

            print(line)
            time.sleep(0.025)

        con.close()
        print(f"\nSummary:")
        print(f"{Fore.GREEN}Valid:   {valid_count}{Style.RESET_ALL}")
        print(f"{Fore.YELLOW}2FA:     {fa_count}{Style.RESET_ALL}")
        print(f"{Fore.RED}Unvalid: {unvalid_count}{Style.RESET_ALL}")

    except sqlite3.Error as e:
        print(f"Database error: {e}")

if __name__ == "__main__":
    extract_accounts()
