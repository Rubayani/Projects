
import time

def task_reminder(task, delay):
    print(f"Reminder set for: {task}")
    time.sleep(delay)
    print(f"Reminder: {task}")

task = input("Enter task to be reminded about: ")
delay = int(input("Enter delay in seconds: "))
task_reminder(task, delay)
