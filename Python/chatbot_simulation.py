
def chatbot_response(user_input):
    responses = {
        "hello": "Hi there! How can I help you?",
        "how are you": "I'm just a program, but I'm doing great!",
        "bye": "Goodbye! Have a nice day!"
    }
    return responses.get(user_input.lower(), "I'm not sure how to respond to that.")

print("Chatbot: Type 'bye' to exit.")
while True:
    user_input = input("You: ")
    if user_input.lower() == "bye":
        print("Chatbot: Goodbye! Have a nice day!")
        break
    print(f"Chatbot: {chatbot_response(user_input)}")
