# Multi-stage build for Vite React app
# Stage 1: Build
FROM node:20-alpine AS build
WORKDIR /app

# Accept API_URL as build argument (default to http://localhost:5004)
ARG API_URL=http://localhost:5004
ENV VITE_API_URL=$API_URL

# Copy package files
COPY package.json package-lock.json ./
# Install dependencies
RUN npm ci

# Copy source code
COPY . .
# Build the Vite app
RUN npm run build

# Stage 2: Serve with nginx
FROM nginx:alpine
# Copy nginx config
COPY --from=build /app/nginx.conf /etc/nginx/nginx.conf

# Copy built app to nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Expose port
EXPOSE 80

# Run nginx
CMD ["nginx", "-g", "daemon off;"]
