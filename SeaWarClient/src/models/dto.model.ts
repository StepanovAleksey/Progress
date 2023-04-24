export interface ICoordinateSimple {
  x: number;
  y: number;
}

export interface IMessageDto<T> {
  uid: string;
  payload: T;
}
export interface IShipDto {
  coordinates: Array<ICoordinateSimple>;
}

export interface IInitGameDto extends IGameDto {
  /**
   * ID всех игроков
   */
  allGamerIds: Array<string>;

  /**
   * ID игрока чей выстрел
   */
  shootGamerId: string;
}

export enum EShootStatus {
  Error = -1,
  Miss = 0,
  Hit = 2,
  Killing = 3,
  KillingAll = 4,
}

/** статусы попадания */
export const SHOOT_TARGET_STATUSES = [
  EShootStatus.Hit,
  EShootStatus.Killing,
  EShootStatus.KillingAll,
];

/**
 * Результаты вытсрела
 */
export interface IShootResultDto extends IGameShootDto {
  shootStatus: EShootStatus;
  nextGamerShooterConnectionId: string;
  coordinate: ICoordinateSimple;
}

/**
 * базовый класс для ДТО моделей по игре
 */
interface IGameDto {
  gameUid: string;
}

/**
 * базовый класс для ДТО моделей по обработке выстрела
 */
interface IGameShootDto extends IGameDto {
  sourceGamerConnectionId: string;
  targetGamerConnectionId: string;
}

/**
 * Корабль убит
 */
export interface IKillingShipDto extends IGameShootDto {
  coordinates: Array<ICoordinateSimple>;
}

export interface IEndGameDto extends IGameDto {
  /**
   * ID игрока который виграл
   */
  winnerGamerId: string;
}
