
def create_note():
    file_name = input("Enter file name for the note: ")
    content = input("Enter the note content: ")
    with open(file_name, 'w') as f:
        f.write(content)
    print(f"Note saved as {file_name}")

def view_note():
    file_name = input("Enter file name to view: ")
    try:
        with open(file_name, 'r') as f:
            print(f.read())
    except FileNotFoundError:
        print("File not found.")

print("[1] Create a new note")
print("[2] View an existing note")
choice = input("Choose an option: ")
if choice == "1":
    create_note()
elif choice == "2":
    view_note()
else:
    print("Invalid choice.")
