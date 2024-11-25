from PIL import ImageGrab
import time

def capture_screenshot():
    timestamp = time.strftime("%Y-%m-%d_%H-%M-%S")
    screenshot = ImageGrab.grab()
    screenshot.save(f"screenshot_{timestamp}.png", "PNG")
    print(f"Screenshot saved as screenshot_{timestamp}.png")

capture_screenshot()
