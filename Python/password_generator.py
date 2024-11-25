import random
import string

def generate_password(length, use_special):
    pool = string.ascii_letters + string.digits
    if use_special:
        pool += string.punctuation
    return "".join(random.choice(pool) for _ in range(length))

print("Random Password Generator")
length = int(input("Enter password length: "))
use_special = input("Use special characters? (y/n): ").lower() == "y"
password = generate_password(length, use_special)
print(f"Generated Password: {password}")
