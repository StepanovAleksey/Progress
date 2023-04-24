import type {
  IAuthRequestDto,
  IAuthResponceDto,
  IUserCreateRequestDto,
} from "@/models/auth.dto";
import { appAxios } from "@/services/axios.service";
import { defineStore } from "pinia";
import { gameStore } from "./game.store";
import { wsWorker } from "@/services/wsWorker.service";

const LOGIN_URL = "login";
const REGISTER_URL = "registration";
const USER_INFO_STORAGE_KEY = "USER_INFO_LOCAL_KEY";

export const userStore = defineStore("user", {
  state() {
    const userInfo: IAuthResponceDto = JSON.parse(
      localStorage.getItem(USER_INFO_STORAGE_KEY) ||
        JSON.stringify(<IAuthResponceDto>{
          id: null,
          name: null,
          email: null,
          token: null,
        })
    );
    return userInfo;
  },
  actions: {
    login(email: string, password: string) {
      return appAxios
        .post<IAuthResponceDto>(LOGIN_URL, <IAuthRequestDto>{
          email,
          password,
        })
        .then((responce) => responce.data)
        .then((data) => {
          this.setUser(data);
          return data;
        });
    },
    create(name: string, email: string, password: string) {
      return appAxios
        .post<IAuthResponceDto>(REGISTER_URL, <IUserCreateRequestDto>{
          name,
          email,
          password,
        })
        .then((responce) => responce.data)
        .then((data) => {
          this.setUser(data);
          return data;
        });
    },
    setUser(userInfo: IAuthResponceDto) {
      this.email = userInfo.email;
      this.id = userInfo.id;
      this.name = userInfo.name;
      this.token = userInfo.token;
      localStorage.setItem(USER_INFO_STORAGE_KEY, JSON.stringify(userInfo));
    },
    close() {
      this.setUser({
        id: null,
        name: null,
        email: null,
        token: null,
      });
    },
  },
});
