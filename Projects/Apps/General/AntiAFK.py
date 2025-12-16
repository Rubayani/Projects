from datetime import datetime, timedelta
import time
import requests

def parse_date(s):
    return datetime.strptime(s, "%Y-%m-%dT%H:%M:%S.%f+00:00")


def get_real_time_sell(url_name):
    try:
        url = f"https://api.warframe.market/v1/items/{url_name}/orders"
        data = requests.get(url).json()["payload"]["orders"]
    except:
        return None

    now = datetime.utcnow()
    sell_prices = []

    for o in data:
        if o.get("order_type") != "sell":
            continue
        
        # Parse dates safely
        try:
            last_update = parse_date(o.get("last_update"))
            last_seen = parse_date(o["user"]["last_seen"])
        except:
            continue

        # Real-time filter
        if last_update < now - timedelta(days =1):
            continue

        sell_prices.append(o["platinum"])

    if not sell_prices:
        return None
    
    return min(sell_prices)


mods = [
    "path_of_statues",
    "tectonic_fracture",
    "ore_gaze",
    "titanic_rumbler",
    "rubble_heap",
    "prismatic_companion",
    "recrystalize",
]




print("Checking real-time prices...\n")

for mod in mods:
    price = get_real_time_sell(mod)
    print(f"https://warframe.market/items/{mod} {price}")
    time.sleep(1.2)