import os
import platform
import time
import webbrowser

def toggle_internet_with_browser():
    system = platform.system()

    # Step 1: Open Google
    webbrowser.open("https://playretrogames.com")
    print("Opened Google... waiting 5 seconds before disabling internet.")
    time.sleep(5)

    # Step 2: Disable internet
    if system == "Windows":
        adapter = "Wi-Fi"  # change if needed
        os.system(f'netsh interface set interface name="{adapter}" admin=disable')
    elif system in ("Linux", "Darwin"):
        adapter = "wlan0"  # change if needed
        os.system(f"sudo ifconfig {adapter} down")
    else:
        print("Unsupported OS")
        return

    print("Internet is OFF. Go refresh the Google page manually to see the error.")

    # Step 3: Wait until user wants to turn it back on
    input("Press Enter to turn Wi-Fi back ON...")

    # Step 4: Re-enable internet
    if system == "Windows":
        os.system(f'netsh interface set interface name="{adapter}" admin=enable')
    elif system in ("Linux", "Darwin"):
        os.system(f"sudo ifconfig {adapter} up")

    print("Internet back ON.")

toggle_internet_with_browser()
