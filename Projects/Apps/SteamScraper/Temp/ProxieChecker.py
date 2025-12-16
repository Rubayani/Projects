import requests, random, time

def get_working_proxy(proxies):
    while proxies:
        proxy = random.choice(proxies)
        try:
            test = requests.get("https://store.steampowered.com", proxies={"http": proxy, "https": proxy}, timeout=8)
            if test.status_code == 200:
                return proxy
        except:
            proxies.remove(proxy)  # drop bad proxy
    return None  # no proxies left

# Example use
with open("proxies.txt") as f:
    proxy_list = [line.strip() for line in f if line.strip()]

proxy = get_working_proxy(proxy_list)
if proxy:
    print("✅ Using proxy:", proxy)
else:
    print("❌ No working proxies found")
