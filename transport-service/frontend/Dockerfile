# Use an official Node runtime as the base image
FROM node:18-alpine as builder

# Set the working directory in the container
WORKDIR /app

# Copy package.json and package-lock.json (if available)
COPY package*.json ./

# Install dependencies
RUN npm install --production

# Copy the current directory contents into the container
COPY . .

# Build the app for production
RUN npm run build

# Use nginx to serve the React app
FROM nginx:alpine

# Copy the build folder from react to the nginx html folder
COPY --from=builder /app/build /usr/share/nginx/html

# Copy custom nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose port 3000
EXPOSE 3000

# Start nginx
CMD ["nginx", "-g", "daemon off;"]