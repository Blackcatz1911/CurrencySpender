import os
import subprocess
import requests
import re
import git

FORK_DIR = "../DalamudPluginsD17"
GITHUB_REPO_OWNER = "Blackcatz1911"
GITHUB_REPO_NAME = "CurrencySpender"
BRANCH_NAME = f"update-manifest-{GITHUB_REPO_NAME}"
IMAGES_URL = "https://raw.githubusercontent.com/Blackcatz1911/CurrencySpender/refs/heads/master/Data/logo.png"
repo = ""
repo_path = "testing/live" if repo == "TEST" else "stable"

# Helper functions to execute shell commands
def run_command(command, cwd=None):
    """Run a shell command and return its output"""
    result = subprocess.run(command, shell=True, cwd=cwd, text=True, capture_output=True)
    if result.returncode != 0:
        print(f"Error running command: {result.stderr}")
        exit(1)
    return result.stdout



# Parse version and changelog from Changelog.md
with open("Changelog.md", "r") as changelog_file:
    changelog_content = changelog_file.read()

# Extract the latest version and changelog entry
version_match = re.search(r"^## (\d{1,2}\.\d{1,2}\.\d{1,2})", changelog_content, re.MULTILINE)
if not version_match:
    print("Version not found in Changelog.md.")
    exit(1)

version = version_match.group(1)

changelog = re.search(rf"## {version}\n(.*?)(?=\n##|\Z)", changelog_content, re.DOTALL)
if not changelog:
    print(f"Changelog entry for version {version} not found.")
    exit(1)

changelog_text = changelog.group(1).strip()

git_rev = run_command('git rev-parse HEAD').strip()

os.chdir(FORK_DIR)
print(f"Changed directory to: {os.getcwd()}")

# Create manifest.toml content
print("Creating manifest.toml...")
target_path = f"{repo_path}/{GITHUB_REPO_NAME}"
os.makedirs(target_path, exist_ok=True)

manifest_content = f"""
[plugin]
repository = "https://github.com/{GITHUB_REPO_OWNER}/{GITHUB_REPO_NAME}.git"
owners = ["{GITHUB_REPO_OWNER}"]
project_path = "{GITHUB_REPO_NAME}"
commit = "{git_rev}"
changelog = \"\"\"\\
**{version}**
{changelog_text}
\"\"\"
version = "{version}"
"""

with open(f"{target_path}/manifest.toml", "w") as manifest_file:
    manifest_file.write(manifest_content)

# Add the images
print("Adding images...")
images_dir = os.path.join(target_path, "images")
os.makedirs(images_dir, exist_ok=True)
image_path = os.path.join(images_dir, "icon.png")

with open(image_path, "wb") as img_file:
    img_file.write(requests.get(IMAGES_URL).content)

manifest_path = os.path.join(target_path, "manifest.toml")
images_dir = os.path.join(target_path, "images")
image_path = os.path.join(images_dir, "icon.png")

print(f"Working directory: {os.getcwd()}")
FORK_DIR_ABS = os.path.abspath(FORK_DIR)
print(f"FORK_DIR_ABS: {FORK_DIR_ABS}")

try:
    # Initialize the repository
    repo = git.Repo(FORK_DIR_ABS)
    repo.git.checkout("main")

    # Fetch latest changes from upstream (if needed)
    print("Fetching latest origin changes...")
    repo.remotes.origin.fetch()
    print("Fetching latest upstream changes...")
    repo.remotes.upstream.fetch()

    with repo.config_writer() as config:
        config.set_value("user", "name", "Blackcatz1911")
        config.set_value("user", "email", "Blackcatz1911@users.noreply.github.com")

    # Check if the branch already exists
    if BRANCH_NAME in repo.heads:
        print(f"Branch {BRANCH_NAME} already exists. Deleting it.")
        repo.git.branch("-D", BRANCH_NAME)

    # Create and switch to the new branch
    print(f"Creating and checking out branch {BRANCH_NAME}...")
    new_branch = repo.create_head(BRANCH_NAME)
    new_branch.checkout()

    # Stage all changes
    print("Staging changes...")
    repo.git.add(A=True)  # Adds all changes to the index

    # Commit the changes
    commit_message = f"Update manifest for {GITHUB_REPO_NAME}"
    print(f"Committing changes with message: {commit_message}")
    repo.index.commit(commit_message)

    # Push the branch to the remote repository
    print(f"Pushing branch {BRANCH_NAME} to origin...")
    # origin = repo.remotes.origin
    # origin.push(refspec=f"{BRANCH_NAME}:{BRANCH_NAME}")

    print(f"Branch {BRANCH_NAME} pushed successfully!")

except git.GitCommandError as e:
    print(f"Git command failed: {e}")
except Exception as e:
    print(f"An error occurred: {e}")