inputAccounts = "accounts.txt"

def getGames():
    gamesList = set()
    with open (inputAccounts,"r", encoding="utf-8") as f:
        for account in f:
            games = account.split("Games = [")[1].split("]")[0]
            for game in games.split(" | "):
                gamesList.add(game.strip())
    gamesList = list(gamesList)
    with open("listOfGames.txt","w") as f:
        print




while True:
    mode = int(input("1: get games"))
    match mode:
        case 1:
            getGames()