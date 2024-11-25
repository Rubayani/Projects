
import socket

def scan_ports(host):
    for port in range(1, 1025):
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
            s.settimeout(0.5)
            if s.connect_ex((host, port)) == 0:
                print(f"Port {port} is open.")

host = input("Enter host to scan (e.g., 127.0.0.1): ")
scan_ports(host)
