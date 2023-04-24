<template>
  <div class="battle">
    <div class="grid grid-nogutter">
      <div class="col-fixed label"></div>
      <div class="col-fixed label" v-for="x in horizontalArr" :key="x">
        {{ LYTERAL_COORDINATE_RU[x] }}
      </div>
    </div>
    <div class="grid grid-nogutter" v-for="y in verticarrArr" :key="y">
      <div class="col-fixed label">{{ y + 1 }}</div>
      <div
        class="col-fixed"
        v-for="x in horizontalArr"
        :key="x"
        v-bind:class="{
          ship: isShip({ x, y }),
          move: isMove({ x, y }),
          init: isInitGame,
          shoot: isShoot({ x, y }),
          miss: isMiss({ x, y }),
          kill: isKill({ x, y }),
        }"
        @click="startMoveShip({ x, y })"
        @mousemove="toMoveShip({ x, y })"
      ></div>
    </div>
  </div>
</template>

<script lang="ts">
import {
  type IShip,
  CoordinateSimpleEqual,
  EGameStatus,
} from "@/models/IShip.model";
import { defineComponent, type PropType } from "vue";

import {
  EShootStatus,
  SHOOT_TARGET_STATUSES,
  type ICoordinateSimple,
} from "@/models/dto.model";
import { LYTERAL_COORDINATE_RU } from "@/models/consts";
import { EStatusGame, gameStore } from "@/stores/game.store";
import { mapStores } from "pinia";

export default defineComponent({
  name: "CurrentBattleArea",
  props: {
    verticalColumn: Number,
    horizontalColumn: Number,
    ships: Object as PropType<Array<IShip>>,
    gameStatus: {
      type: String as PropType<EStatusGame>,
      required: true,
    },
  },
  setup() {
    return {
      lastMoveCoordinate: null as CoordinateSimpleEqual,
      LYTERAL_COORDINATE_RU: LYTERAL_COORDINATE_RU,
    };
  },
  data() {
    return {
      movedShip: null as IShip,
    };
  },
  computed: {
    ...mapStores(gameStore),
    verticarrArr() {
      return Array.from(Array(this.verticalColumn).keys());
    },
    horizontalArr() {
      return Array.from(Array(this.horizontalColumn).keys());
    },
    isInitGame() {
      return this.gameStatus === EStatusGame.Init;
    },
  },
  methods: {
    isShip(coordinate: ICoordinateSimple) {
      return !!this.getShipByCoordiante(coordinate);
    },
    isMove(coordinate: ICoordinateSimple) {
      const ship = this.getShipByCoordiante(coordinate);
      return ship?.isMove();
    },
    startMoveShip(coordinate: ICoordinateSimple) {
      if (!this.isInitGame) {
        return;
      }
      this.movedShip = null;
      const ship = this.getShipByCoordiante(coordinate);
      this.ships
        .filter((s) => s !== ship)
        .forEach((s) => s.setStatus(EGameStatus.None));
      if (ship) {
        ship.setStatus(ship.isMove() ? EGameStatus.None : EGameStatus.Move);
        this.movedShip = ship.isMove() ? ship : null;
      }
      this.$forceUpdate();
    },
    toMoveShip(coordinate: ICoordinateSimple) {
      if (!this.movedShip || this.lastMoveCoordinate?.isEqual(coordinate)) {
        return;
      }
      this.lastMoveCoordinate = new CoordinateSimpleEqual(
        coordinate.x,
        coordinate.y
      );
      if (this.movedShip.canMove(coordinate, this.ships)) {
        this.movedShip.setNewCoordinate(coordinate);
        this.$forceUpdate();
      }
    },
    getShipByCoordiante(coordinate: ICoordinateSimple) {
      return this.ships?.find((s) => s.isExist(coordinate));
    },
    statusCoodinante(coordinate: ICoordinateSimple) {
      return this.gameStore.getEnemyShootInHistory(coordinate)?.status;
    },
    isShoot(coordinate: ICoordinateSimple) {
      return SHOOT_TARGET_STATUSES.includes(this.statusCoodinante(coordinate));
    },
    isMiss(coordinate: ICoordinateSimple) {
      return this.statusCoodinante(coordinate) === EShootStatus.Miss;
    },
    isKill(coordinate: ICoordinateSimple) {
      return this.statusCoodinante(coordinate) === EShootStatus.Killing;
    },
  },
});
</script>

<style lang="less" scoped>
@import "./battle.less";
@ship-cpolor: #6aa84f;
@ship-cpolor-init: #539ed6;

.ship {
  background-color: @ship-cpolor;
  &.move {
    opacity: 0.6;
  }
  &.init {
    background-color: @ship-cpolor-init;
  }
  &.shoot {
    background-color: @shoot-color;
  }
  &.shoot {
    background-color: @shoot-color;
  }
  &.kill {
    display: flex;
    align-items: center;
    justify-content: center;
    &::after {
      content: "X";
    }
  }
}
</style>
