
from barcode import EAN13
from barcode.writer import ImageWriter

def generate_barcode(data, file_name):
    barcode = EAN13(data, writer=ImageWriter())
    barcode.save(file_name)
    print(f"Barcode saved as {file_name}.png")

data = input("Enter 12-digit number for the barcode: ")
file_name = input("Enter file name to save the barcode (e.g., barcode): ")
generate_barcode(data, file_name)
