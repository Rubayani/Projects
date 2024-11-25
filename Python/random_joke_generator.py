
import requests

def get_random_joke():
    url = "https://official-joke-api.appspot.com/random_joke"
    response = requests.get(url)
    if response.status_code == 200:
        joke = response.json()
        print(f"{joke['setup']} - {joke['punchline']}")
    else:
        print("Failed to fetch a joke.")

print("Here's a random joke:")
get_random_joke()
