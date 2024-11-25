from forex_python.converter import CurrencyRates

def convert_currency(amount, from_currency, to_currency):
    rates = CurrencyRates()
    try:
        result = rates.convert(from_currency.upper(), to_currency.upper(), amount)
        print(f"{amount} {from_currency} = {result:.2f} {to_currency}")
    except Exception as e:
        print(f"Error: {e}")

amount = float(input("Enter amount: "))
from_currency = input("From currency (e.g., USD): ")
to_currency = input("To currency (e.g., EUR): ")
convert_currency(amount, from_currency, to_currency)
