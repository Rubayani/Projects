
import os

def merge_text_files(file_list, output_file):
    with open(output_file, 'w') as outfile:
        for file_name in file_list:
            if os.path.exists(file_name):
                with open(file_name, 'r') as infile:
                    outfile.write(infile.read() + "\n")
            else:
                print(f"{file_name} not found.")
    print(f"All files merged into {output_file}.")

file_list = input("Enter file names to merge (comma-separated): ").split(",")
output_file = input("Enter output file name: ")
merge_text_files(file_list, output_file)
