#!/bin/bash

# Nombre del contenedor e imagen
CONTAINER_NAME="cartera-api"
IMAGE_NAME="carteravirtual"

echo "🛑 Deteniendo y eliminando contenedor anterior (si existe)..."
docker stop $CONTAINER_NAME 2>/dev/null
docker rm $CONTAINER_NAME 2>/dev/null

echo "🔨 Construyendo imagen Docker..."
docker build -t $IMAGE_NAME .

echo "🚀 Ejecutando contenedor en segundo plano..."
docker run -d -p 8000:8000 --name $CONTAINER_NAME $IMAGE_NAME

echo "✅ API CarteraVirtual corriendo en http://localhost:8000"
