
import smtplib

def send_email(subject, body, to_email, from_email, password):
    server = smtplib.SMTP('smtp.gmail.com', 587)
    server.starttls()
    server.login(from_email, password)
    message = f"Subject: {subject}\n\n{body}"
    server.sendmail(from_email, to_email, message)
    server.quit()
    print("Email sent successfully.")

from_email = input("Enter your email: ")
password = input("Enter your email password: ")
to_email = input("Enter recipient's email: ")
subject = input("Enter subject: ")
body = input("Enter email body: ")
send_email(subject, body, to_email, from_email, password)
