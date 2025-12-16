import pymem
import time

pm = pymem.Pymem("bio4.exe")
ammo_address = 0x08CA07E0  # Replace with real static ammo address

try:
    print("[*] Freezing ammo at", hex(ammo_address))
    while True:
        pm.write_uchar(ammo_address, 255)
        time.sleep(0.05)  # Update every 50 ms (adjust as needed)
except KeyboardInterrupt:
    print("\n[!] Freeze stopped.")
