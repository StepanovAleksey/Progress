import {
  type IShipDto,
  type ICoordinateSimple,
  EShootStatus,
} from "@/models/dto.model";
import { defineStore } from "pinia";
import { filter, switchMap, take, tap, map, takeUntil } from "rxjs";
import { wsWorker } from "../services/wsWorker.service";
import { userStore } from "./user.store";

export enum EStatusGame {
  Init = "Init",
  Find = "Find",
  Game = "Game",
  EndGame = "EndGame",
}
export interface IcoordinateShoot extends ICoordinateSimple {
  status: EShootStatus;
}
export const gameStore = defineStore("game", {
  state() {
    return {
      /** статус игры */
      gameStatus: EStatusGame.Init,

      /** ID противника */
      enemyGamerId: null as string,

      /** ID игрока который стреляет */
      shootGamerId: null as string,

      /** история выстрелов по полю противника (ходы текущего) */
      enemyShootHistory: [] as Array<IcoordinateShoot>,

      /** история выстрелов по полю текущего игрока (ходы соперника) */
      myShootHistory: [] as Array<IcoordinateShoot>,

      userWinnerId: null as string,
    };
  },
  actions: {
    connected() {},
    gameStart(sheeps: Array<IShipDto>, useBot: boolean = false) {
      const _userStore = userStore();
      wsWorker
        .getAnswer$<boolean>(
          wsWorker.sendMsg(!useBot ? "GameStart" : "GameStartWithBot", sheeps)
        )
        .pipe(
          filter((result) => result),
          tap(() => (this.gameStatus = EStatusGame.Find)),
          /** выставялм пользователя */
          tap(() => wsWorker.sendMsg("SetUserId", _userStore.id)),
          switchMap(() =>
            wsWorker.startGame$.pipe(
              take(1),
              tap((result) => {
                this.enemyGamerId = result.allGamerIds.find(
                  (gId) => gId !== this.connectionId$.value
                );
                this.shootGamerId = result.shootGamerId;
                this.gameStatus = EStatusGame.Game;
              })
            )
          ),
          switchMap(() => wsWorker.resultShoot$),
          takeUntil(wsWorker.endGame$),
          map((msg) => msg)
        )
        .subscribe((result) => {
          this.shootGamerId = result.nextGamerShooterConnectionId;
          if (result.targetGamerConnectionId === this.enemyGamerId) {
            this.enemyShootHistory.push({
              ...result.coordinate,
              status: result.shootStatus,
            });
          }
          if (result.targetGamerConnectionId === wsWorker.connectionId$.value) {
            this.myShootHistory.push({
              ...result.coordinate,
              status: result.shootStatus,
            });
          }
        });

      wsWorker.killingShip$
        .pipe(takeUntil(wsWorker.endGame$))
        .subscribe((killingDto) => {
          let shootHistory: Array<IcoordinateShoot> = [];
          /**
           * убили корабль текущего игрока
           */
          if (killingDto.targetGamerConnectionId === this.connectionId$.value) {
            shootHistory = this.myShootHistory;
          }

          /**
           * убили корабль сопреника
           */
          if (killingDto.targetGamerConnectionId === this.enemyGamerId) {
            shootHistory = this.enemyShootHistory;
          }

          killingDto.coordinates.forEach((ch) => {
            shootHistory
              .filter((c) => c.x === ch.x && c.y === ch.y)
              .forEach((c) => (c.status = EShootStatus.Killing));
          });
        });

      wsWorker.endGame$.subscribe((endGame) => {
        this.gameStatus = EStatusGame.EndGame;
        this.userWinnerId = endGame.winnerGamerId;
      });
    },
    /** отправить выстрел */
    sendShoot(coordinate: ICoordinateSimple) {
      wsWorker.sendMsg("Shoot", coordinate);
    },
    refreshGame() {
      this.gameStatus = EStatusGame.Init;
      this.$reset();
    },
  },
  getters: {
    /** текущий ID коннекта  */
    connectionId$() {
      return wsWorker.connectionId$;
    },

    /** проверка что выстрел текущего игрока */
    isMyShoot(state) {
      return wsWorker.connectionId$.value === state.shootGamerId;
    },
    /** получить ход соперника */
    getEnemyShootInHistory(state) {
      return (coordinate: ICoordinateSimple) => {
        return state.myShootHistory.find(
          (c) => c.x === coordinate.x && c.y === coordinate.y
        );
      };
    },
    /** получить ход текущего игрока */
    getMyShootInHistory(state) {
      return (coordinate: ICoordinateSimple) => {
        return state.enemyShootHistory.find(
          (c) => c.x === coordinate.x && c.y === coordinate.y
        );
      };
    },
  },
});
