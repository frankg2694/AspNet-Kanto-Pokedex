import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
    plugins: [react()],
    server: {
        port: 35630,
        // Send every request that starts with /api to the ASP.NET Core Server (AKA the proxy)
        proxy: {
            '/api': 'http://localhost:5219',
        },
    },
    // Put the production build in the Server's wwwroot folder
    build: {
        outDir: '../Pokedex.Server/wwwroot',
        // Delete the old files in wwwroot before each build
        emptyOutDir: true,
    },
    // Settings for Vitest
    test: {
        // Run the tests in jsdom, a fake browser that runs inside Node
        environment: 'jsdom',
        // File that runs before every test file (see src/test/setup.js)
        setupFiles: './src/test/setup.js',
    },
})