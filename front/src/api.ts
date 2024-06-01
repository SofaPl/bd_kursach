// src/api.ts
import axios from 'axios';

const api = axios.create({
    baseURL: 'https://localhost:7104/api', // Замените на ваш URL
});

export default api;