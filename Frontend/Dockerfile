FROM node:24-alpine AS build
WORKDIR /app

# Copiar package.json y package-lock.json
COPY package*.json ./

# Instalar dependencias
RUN npm install

# Copiar el resto del código
COPY . .

# Exponer el puerto
EXPOSE 4200

# Comando para iniciar la aplicación
CMD ["npm", "start", "--", "--host", "0.0.0.0", "--disable-host-check"]
