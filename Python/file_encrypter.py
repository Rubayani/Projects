
from cryptography.fernet import Fernet

def generate_key():
    return Fernet.generate_key()

def encrypt_file(file_path, key):
    with open(file_path, 'rb') as f:
        data = f.read()
    encrypted = Fernet(key).encrypt(data)
    with open(file_path, 'wb') as f:
        f.write(encrypted)

def decrypt_file(file_path, key):
    with open(file_path, 'rb') as f:
        data = f.read()
    decrypted = Fernet(key).decrypt(data)
    with open(file_path, 'wb') as f:
        f.write(decrypted)

key = generate_key()
print(f"Save this key to decrypt your file: {key.decode()}")
file_path = input("Enter file path to encrypt: ")
encrypt_file(file_path, key)
print("File encrypted.")

if input("Do you want to decrypt the file? (y/n): ").lower() == "y":
    decrypt_key = input("Enter decryption key: ").encode()
    decrypt_file(file_path, decrypt_key)
    print("File decrypted.")
