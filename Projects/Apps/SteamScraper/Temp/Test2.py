import requests
from concurrent.futures import ThreadPoolExecutor, as_completed
import time, random, sys

APP_LIST_URL = "https://api.steampowered.com/ISteamApps/GetAppList/v2/"
REVIEWS_URL = "https://store.steampowered.com/appreviews/{appid}?json=1&language=all&purchase_type=all"
PRICE_URL = "https://store.steampowered.com/api/appdetails?appids={appid}&cc=us"

ban_trigger_count = 5 
fail_streak = 0        

def fetch_app_list():
    resp = requests.get(APP_LIST_URL)
    resp.raise_for_status()
    data = resp.json()
    return {app["name"]: app["appid"] for app in data["applist"]["apps"]}

def fetch_review_score(appid):
    url = REVIEWS_URL.format(appid=appid)
    resp = requests.get(url, timeout=10)
    time.sleep(4)  
    if resp.status_code == 429:
        raise RuntimeError("Steam API ban detected (429 Too Many Requests)")
    if resp.status_code != 200:
        return None
    data = resp.json()
    if "query_summary" in data:
        total = data["query_summary"]["total_reviews"]
        positive = data["query_summary"]["total_positive"]
        if total > 0:
            return round((positive / total) * 100, 2)
    return None

def fetch_price(appid):
    url = PRICE_URL.format(appid=appid)
    for attempt in range(3):
        try:
            resp = requests.get(url, timeout=10)
            time.sleep(4) 
            if resp.status_code == 429:
                raise RuntimeError("Steam API ban detected (429 Too Many Requests)")
            if resp.status_code != 200:
                time.sleep(4)
                continue

            data = resp.json()
            app_data = data.get(str(appid), {})
            if not app_data.get("success", False):
                return "Unavailable"

            game_data = app_data.get("data", {})
            if not game_data:
                return "Unavailable"

            if game_data.get("is_free"):
                return "Free"

            price_info = game_data.get("price_overview")
            if price_info:
                return price_info.get("final_formatted", "Unavailable")

            return "Unavailable"
        except Exception:
            time.sleep(4 + random.random())
    return "Unavailable"

def process_game(game, app_map):
    appid = app_map.get(game)
    if not appid:
        return (game, None, None, "AppID not found")
    score = fetch_review_score(appid)
    price = fetch_price(appid)
    return (game, score, price, None)

def price_to_number(price_str):
    if not price_str:
        return float("inf")
    price_str = price_str.strip()
    if price_str.lower().startswith("free"):
        return 0.0
    if price_str.lower() == "unavailable":
        return float("inf")
    try:
        
        num = "".join(ch for ch in price_str if ch.isdigit() or ch == ".")
        return float(num) if num else float("inf")
    except:
        return float("inf")

def main():
    global fail_streak
    with open("listOfGames.txt", "r", encoding="utf-8") as f:
        games = [line.strip() for line in f if line.strip()]

    app_map = fetch_app_list()
    checked_file = "gamesChecked(Reviews).txt"

    checked = set()
    try:
        with open(checked_file, "r", encoding="utf-8") as f:
            for line in f:
                game = line.split("-", 1)[0].strip()
                checked.add(game)
    except FileNotFoundError:
        pass

    to_check = [game for game in games if game not in checked]
    total = len(to_check)
    done = 0

    results = []

    try:
        with ThreadPoolExecutor(max_workers=2) as executor:
            futures = {executor.submit(process_game, game, app_map): game for game in to_check}
            for future in as_completed(futures):
                try:
                    game, score, price, error = future.result()
                except RuntimeError as e:
                    print("BAN DETECTED:", e)
                    break
                except Exception as e:
                    print("Error:", e)
                    continue

                done += 1
                left = total - done

                if error:
                    fail_streak += 1
                    print(f"[{done}/{total}] {game}: {error}")
                elif score is None:
                    fail_streak += 1
                    print(f"[{done}/{total}] {game}: Failed to get reviews")
                else:
                    fail_streak = 0
                    print(f"[{done}/{total}] {game}: {score}% | {price}")
                    results.append(f"{game} - {score}% | {price}")

                print(f"Checked: {done} | ⏳ Left: {left}")

                if fail_streak >= ban_trigger_count:
                    print("Too many failures in a row, possible ban. Stopping early.")
                    break

    finally:
        if results:
            # sort results list by price
            results.sort(key=lambda line: price_to_number(line.split("|")[-1]))
            with open(checked_file, "a", encoding="utf-8") as f:
                for line in results:
                    f.write(line + "\n")
                    f.flush()
        print("Progress saved to file. Exiting.")

if __name__ == "__main__":
    main()
