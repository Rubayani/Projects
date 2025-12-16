#include <Windows.h>
#include <TlHelp32.h>
#include <iostream>

// Radar settings
const int radarSize = 200;
const float scale = 10.0f;  // lower = more zoomed out

struct Vec3 { float x, y, z; };
struct Vec2 { float x, y; };

// Fake player + enemy positions (replace with real game memory later)
Vec3 player = { 100.0f, 0.0f, 100.0f };
Vec3 enemy  = { 150.0f, 0.0f, 180.0f };

// Convert world (X,Z) → radar (screen)
Vec2 WorldToRadar(const Vec3& player, const Vec3& obj) {
    Vec2 pos;
    pos.x = (obj.x - player.x) / scale + radarSize / 2;
    pos.y = (obj.z - player.z) / scale + radarSize / 2;
    return pos;
}

// Window procedure
LRESULT CALLBACK WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam) {
    switch (msg) {
    case WM_PAINT: {
        PAINTSTRUCT ps;
        HDC hdc = BeginPaint(hwnd, &ps);

        // Clear background
        RECT rect;
        GetClientRect(hwnd, &rect);
        FillRect(hdc, &rect, (HBRUSH)(COLOR_WINDOW + 1));

        // Draw self (center)
        Ellipse(hdc, radarSize/2 - 5, radarSize/2 - 5, radarSize/2 + 5, radarSize/2 + 5);

        // Draw enemy
        Vec2 radarPos = WorldToRadar(player, enemy);
        Ellipse(hdc, radarPos.x - 3, radarPos.y - 3, radarPos.x + 3, radarPos.y + 3);

        EndPaint(hwnd, &ps);
        break;
    }
    case WM_DESTROY:
        PostQuitMessage(0);
        return 0;
    }
    return DefWindowProc(hwnd, msg, wParam, lParam);
}

int WINAPI WinMain(HINSTANCE hInst, HINSTANCE, LPSTR, int) {
    // Register window class
    WNDCLASS wc = { 0 };
    wc.lpfnWndProc = WndProc;
    wc.hInstance = hInst;
    wc.lpszClassName = "RadarESP";
    RegisterClass(&wc);

    // Create window
    HWND hwnd = CreateWindowEx(
        0, "RadarESP", "Radar ESP",
        WS_OVERLAPPEDWINDOW ^ WS_THICKFRAME ^ WS_MAXIMIZEBOX,
        100, 100, radarSize, radarSize,
        0, 0, hInst, 0);

    ShowWindow(hwnd, SW_SHOW);

    // Main loop
    MSG msg;
    while (true) {
        while (PeekMessage(&msg, 0, 0, 0, PM_REMOVE)) {
            if (msg.message == WM_QUIT) return 0;
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }

        // Refresh window ~60fps
        InvalidateRect(hwnd, NULL, TRUE);
        Sleep(16);
    }
}
