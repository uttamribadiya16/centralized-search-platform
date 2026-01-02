# Build stage
FROM node:18-alpine AS build
WORKDIR /app

# Copy package.json and package-lock.json
COPY account-frontend/package*.json ./
RUN npm install --production

# Copy source code
COPY account-frontend/ .

# Build the React app
RUN npm run build

# Production stage
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Remove default nginx static files
RUN rm -rf ./*

# Copy built React app
COPY --from=build /app/build .

# Copy nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 3000

CMD ["nginx", "-g", "daemon off;"]