export interface IAuthRequestDto {
  email: string;
  password: string;
}
export interface IAuthResponceDto {
  id: number;
  name: string;
  email: string;
  token: string;
}

export interface IUserCreateRequestDto {
  name: string;
  email: string;
  password: string;
  ipAdress: string;
}
