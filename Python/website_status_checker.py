
import requests

def check_website_status(url):
    try:
        response = requests.get(url)
        if response.status_code == 200:
            print(f"{url} is online.")
        else:
            print(f"{url} returned status code {response.status_code}.")
    except Exception as e:
        print(f"Could not reach {url}. Error: {e}")

url = input("Enter website URL (e.g., https://example.com): ")
check_website_status(url)
