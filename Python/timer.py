
import time

def pomodoro(minutes):
    seconds = minutes * 60
    print(f"Pomodoro started for {minutes} minutes.")
    while seconds:
        mins, secs = divmod(seconds, 60)
        print(f"Time left: {mins:02}:{secs:02}", end="\r")
        time.sleep(1)
        seconds -= 1
    print("\nTime's up! Take a break.")

duration = int(input("Enter Pomodoro duration in minutes: "))
pomodoro(duration)
