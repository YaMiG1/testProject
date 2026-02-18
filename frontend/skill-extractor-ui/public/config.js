// Runtime configuration
// This file is loaded before the app starts and allows injecting configuration
// from environment variables or docker-compose
window.__APP_CONFIG__ = {
  API_URL: process.env.REACT_APP_API_URL || "http://localhost:5004"
};
