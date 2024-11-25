
import requests

def get_definition(word):
    url = f"https://api.dictionaryapi.dev/api/v2/entries/en/{word}"
    response = requests.get(url)
    if response.status_code == 200:
        data = response.json()
        meanings = data[0]['meanings']
        for meaning in meanings:
            print(f"Part of speech: {meaning['partOfSpeech']}")
            for definition in meaning['definitions']:
                print(f"- {definition['definition']}")
    else:
        print("Word not found.")

word = input("Enter a word to look up: ")
get_definition(word)
