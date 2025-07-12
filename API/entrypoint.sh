#!/bin/bash

ollama serve &
OLLAMA_PID=$!

# Wait for Ollama to start
echo "Waiting for Ollama to start..."
until curl -s -f http://0.0.0.0:11434/api/version >/dev/null 2>&1; do
    sleep 1
done
echo "Ollama is running"

# Pull the model
echo "Pulling mistral model..."
ollama pull mistral

# Keep the container running
wait $OLLAMA_PID