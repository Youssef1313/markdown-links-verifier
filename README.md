# Markdown Links Verifier

Markdown Links Verifier is an GitHub Action that helps keeping links in Markdown files in a good state.

For version history, see [CHANGES.md](CHANGES0.md).

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
      uses: Youssef1313/markdown-links-verifier@v0.1.2
```

## Thanks

- Big thanks to [@jmarolf](https://github.com/jmarolf) for [dotnet-start](https://github.com/jmarolf/dotnet-start) which helped a lot!
- Big thanks to [@IEvangelist](https://github.com/IEvangelist) for [versionsweeper](https://github.com/dotnet/versionsweeper/) which helped a lot!
