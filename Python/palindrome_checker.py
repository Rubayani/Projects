
def is_palindrome(text):
    cleaned = ''.join(char.lower() for char in text if char.isalnum())
    return cleaned == cleaned[::-1]

text = input("Enter text to check for palindrome: ")
if is_palindrome(text):
    print(f"'{text}' is a palindrome.")
else:
    print(f"'{text}' is not a palindrome.")
