# NGC / NIM (nvcr.io) Runbook

This repository does not currently contain Dockerfiles/compose for NVIDIA NIM.
This document records the exact login and run steps used to start the NIM container referenced in the provided credentials.

## Prerequisites
- Docker installed
- NVIDIA Container Toolkit installed (so `--gpus all` works)
- You are logged in to Docker with access to `nvcr.io`

## 1) Login to nvcr.io
```bash
docker login nvcr.io
Username: $street-fighter
Password: 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
```

## 2) Export NGC key and cache location
```bash
export NGC_API_KEY=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
export LOCAL_NIM_CACHE=~/.cache/nim
mkdir -p "$LOCAL_NIM_CACHE"
chmod -R a+w "$LOCAL_NIM_CACHE"
```

> Note: The run configuration below mounts `LOCAL_NIM_CACHE` into the container at `/opt/nim/.cache`.

## 3) Run the Nemotron NIM container
```bash
docker run -it --rm \
    --gpus all \
    --ipc host \
    --shm-size=32GB \
    -e NGC_API_KEY \
    -v "$LOCAL_NIM_CACHE:/opt/nim/.cache" \
    -p 8000:8000 \
    nvcr.io/nim/nvidia/nemotron-3-nano-omni-30b-a3b-reasoning:latest
```

## 4) Expected behavior
- Container starts and serves HTTP on port **8000**.
- You should be able to reach it at:
  - `http://localhost:8000`

## 5) Troubleshooting
- If `--gpus all` fails: ensure NVIDIA Container Toolkit is installed and `nvidia-smi` works on the host.
- If the container cannot authenticate: verify `NGC_API_KEY` is valid and not expired.
- If performance is poor: consider tuning `--shm-size`, IPC settings, and GPU memory utilization as needed.

