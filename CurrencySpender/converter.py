import re, os, requests

# Get the directory of the current script
script_directory = os.path.dirname(os.path.abspath(__file__))

# Define the file name
file_name = "list.txt"  # Replace with your file's name

# Construct the full file path
file_path = os.path.join(script_directory, file_name)

# Open and read the file
with open(file_path, "r") as file:
    file_content = file.read()

# Regular expression to capture item names based on their context
pattern = r'^(.*)\nYou can buy \d+,'

# Find all matches
item_names = re.findall(pattern, file_content, re.MULTILINE)

# Print the extracted item names
ids = []
for item_name in item_names:
    url = 'https://beta.xivapi.com/api/1/search?sheets=Item&query=Name~"'+item_name+'"'
    response = requests.get(url)
    data = response.json()
    ids.append(data['results'][0]['row_id'])

ids.sort()
print(ids)
