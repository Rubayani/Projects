
from PIL import Image
import os

def resize_image(input_path, output_path, width, height):
    with Image.open(input_path) as img:
        img_resized = img.resize((width, height))
        img_resized.save(output_path)

input_path = input("Enter input image path: ")
output_path = input("Enter output image path: ")
width = int(input("Enter width: "))
height = int(input("Enter height: "))
resize_image(input_path, output_path, width, height)
print("Image resized.")
