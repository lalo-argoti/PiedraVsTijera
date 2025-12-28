FROM node:18-alpine

WORKDIR /app

# Dependencias (cache)
COPY package*.json ./
RUN npm ci

# Código
COPY . .

EXPOSE 4200
CMD ["npx", "ng", "serve", "--host", "0.0.0.0"]
