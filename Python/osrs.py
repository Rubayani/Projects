import win32gui
import win32con
import win32api
import keyboard
import time


window_name = "RuneScape"
screen_width = 1920
screen_height = 1050

window_width = 800
window_height = 600

positions = [
    (0, 0),
    (screen_width - window_width, 0),
    (0, screen_height - window_height),
    (screen_width - window_width, screen_height - window_height)
]


def find_windows_by_title(title):
    hwnds = []

    def enum_window_callback(hwnd, extra):
        if win32gui.IsWindowVisible(hwnd) and title in win32gui.GetWindowText(hwnd):
            hwnds.append(hwnd)

    win32gui.EnumWindows(enum_window_callback, None)
    return hwnds


RuneScape_windows = find_windows_by_title(window_name)

if not RuneScape_windows:
    print("No windows with the name 'RuneScape' found.")
else:
    for i, hwnd in enumerate(RuneScape_windows[:4]):
        try:
            x, y = positions[i]
            win32gui.SetWindowPos(hwnd, None, x, y, window_width, window_height,
                                  win32con.SWP_NOZORDER | win32con.SWP_NOACTIVATE)
        except Exception as e:
            print(f"Could not adjust window '{win32gui.GetWindowText(hwnd)}': {e}")


    def bring_to_front(index):
        if index < len(RuneScape_windows):
            hwnd = RuneScape_windows[index]
            try:
                win32gui.ShowWindow(hwnd, win32con.SW_RESTORE)
                win32gui.SetForegroundWindow(hwnd)

                win32api.keybd_event(win32con.VK_MENU, 0, 0, 0)
                time.sleep(0.05)
                win32api.keybd_event(win32con.VK_TAB, 0, 0, 0)
                time.sleep(0.05)
                win32api.keybd_event(win32con.VK_TAB, 0, win32con.KEYEVENTF_KEYUP, 0)
                win32api.keybd_event(win32con.VK_MENU, 0, win32con.KEYEVENTF_KEYUP, 0)
            except Exception as e:
                print(f"Could not bring window '{win32gui.GetWindowText(hwnd)}' to front: {e}")


    keyboard.add_hotkey('f1', bring_to_front, args=[0])
    keyboard.add_hotkey('f2', bring_to_front, args=[1])
    keyboard.add_hotkey('f3', bring_to_front, args=[2])
    keyboard.add_hotkey('f4', bring_to_front, args=[3])

    print("Press F1 to F4 to bring each window to the front.")
    print("Press Ctrl+C to exit.")
    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        print("Exiting...")
