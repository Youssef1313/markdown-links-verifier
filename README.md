# Markdown Links Verifier

This is an undergoing work to create a GitHub Action for validating links in markdown files.

## Get started

To get started, create a *markdown-links-verifier.yml* under *.github/workflows/* directory with the following contents:

```yml
name: Markdown links verifier
on: [push, pull_request]

jobs:
  validate_links:
    name: Markdown links verifier
    runs-on: ubuntu-latest

    steps:
    - name: Checkout the repository
      uses: actions/checkout@v1

    - name: Validate links
      uses: Youssef1313/markdown-links-verifier@v0.0.6
```

## Thanks

Big thanks to [@jmarolf](https://github.com/jmarolf) for [dotnet-start](https://github.com/jmarolf/dotnet-start) which helped a lot!
