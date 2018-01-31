# How to contribute?

## Setup 

1. Fork your own copy of the original repository in GitHub.

2. Clone your fork onto your local machine. Example

   ```
   git clone https://github.com/<your_username>/doorman.git
   ```

3. Add the original repository as a remote in your local repo. Example:

   ```
   git remote add upstream https://github.com/souldzin/doorman.git
   ```

## Notes on making a changes

* Be mindful of what branch you are on. 

  ```
  git status 

  git branch -v
  ```

* It might be helpful to create a new branch for each task you are working on. 

  ```
  # Create a feature branch
  git checkout -b feature/ir-sensors

  # Oh no! We found a bug. I've got to fix this right away...
  # 1) Commit my unfinished work...

  # 2) Create a bugfix branch
  git checkout -b bugfix/ui-server-error

  # 3) Reset my current branch to the code I need to fix
  git fetch --all 
  git reset --hard upstream develop

  # 4) Fix the bug, test, commit, push, and create a pull request.

  # Now, I'm done with the server error, I can switch back to feature/ir-sensors :)
  git checkout feature/ir-sensors
  ```

* Before starting work, make sure your code is up-to-date with the upstream branch. Example:

  ```
  git checkout -b feature/my-cool-feat

  # Option 1 - this should work most of the time
  git pull origin develop

  # Option 2 - this should work all of the time (I do this one)
  git fetch --all
  git reset --hard origin/develop
  ```

## Creating pull requests

1. I made some commits to my local branch and I'm ready to show them off.

  ```
  # Option 1 - stage changes for commit through cli
  git add --all

  # Confirm you have the right files staged
  git status 

  git commit -m "implemented the wigwam with system.py"

  # Option 2 - use the git gui (you could probably also use your IDE)
  git gui
  ```

2. Push these commits to my forked repository.

  ```
  # The first time you run this, you'll need to include the "-u" flag.
  # This tells git that my local branch feeds from the specified remote branch. Afterwards, git will remmeber this, so you don't need to "-u" flag.
  git push -u origin <name_of_branch>
  ```

3. In GitHub, visit your repository and hit "New Pull Request". 

4. Make sure that the source branch is correct and the target repository is "souldzin/doorman" and the target branch is "develop"

5. Wait for the PR (Pull Request) to be reviewed

6. If changes need to be made, there's no need to create another PR, simply update the branch that already has an open PR.

7. Once the PR is merged, you can consider deleting the branch from your local repository and your fork.

8. Good job!