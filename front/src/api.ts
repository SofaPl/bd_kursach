// src/api.ts
import axios from 'axios';

const api = axios.create({
    baseURL: 'https://localhost:7104/api', // �������� �� ��� URL
});

export default api;