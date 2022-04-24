import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vitejs.dev/config/
export default defineConfig({
  optimizeDeps: { exclude: ['firebase/app', 'firebase/auth', 'firebase/analytics'] },
  plugins: [vue()]
})
