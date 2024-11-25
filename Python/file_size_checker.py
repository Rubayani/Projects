
import os

def list_file_sizes(directory):
    if not os.path.exists(directory):
        print("Directory not found.")
        return
    for file in os.listdir(directory):
        file_path = os.path.join(directory, file)
        if os.path.isfile(file_path):
            print(f"{file}: {os.path.getsize(file_path)} bytes")

directory = input("Enter directory path: ")
list_file_sizes(directory)
