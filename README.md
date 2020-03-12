# Repro Steps

- `docker build . -t repro`
- `docker run --rm -it repro`
  - Note: `docker run --rm -i repro` works fine; this only repros with `-t`
- `docker stop CONTAINER_ID`
