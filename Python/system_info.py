
import platform
import psutil

def display_system_info():
    print(f"System: {platform.system()} {platform.release()}")
    print(f"Processor: {platform.processor()}")
    print(f"RAM: {round(psutil.virtual_memory().total / (1024 * 1024 * 1024), 2)} GB")

display_system_info()
