import { defineConfig } from "vite";

const apiTarget = process.env.VITE_API_PROXY_TARGET || "https://localhost:7062";

export default defineConfig({
  esbuild: {
    jsx: "automatic"
  },
  server: {
    proxy: {
      "/login": { target: apiTarget, changeOrigin: true, secure: false },
      "/user": { target: apiTarget, changeOrigin: true, secure: false },
      "/sleep": { target: apiTarget, changeOrigin: true, secure: false },
      "/insights": { target: apiTarget, changeOrigin: true, secure: false }
    }
  }
});
