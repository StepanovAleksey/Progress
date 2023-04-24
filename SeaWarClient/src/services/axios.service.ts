import { API_BASE_URL } from "@/models/consts";
import axios from "axios";
const axiosConfig = {
  baseURL: API_BASE_URL,
  timeout: 30000,
};

export const appAxios = axios.create(axiosConfig);
