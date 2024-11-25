
def length_converter(value, from_unit, to_unit):
    conversions = {"m": 1, "km": 1000, "cm": 0.01, "mm": 0.001}
    return value * conversions[from_unit] / conversions[to_unit]

value = float(input("Enter value: "))
from_unit = input("From unit (m, km, cm, mm): ").lower()
to_unit = input("To unit (m, km, cm, mm): ").lower()
result = length_converter(value, from_unit, to_unit)
print(f"{value} {from_unit} = {result} {to_unit}")
