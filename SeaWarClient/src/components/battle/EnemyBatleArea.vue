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
          shoot: isShoot({ x, y }),
          miss: isMiss({ x, y }),
          'my-shoot': isMyShoot,
          kill: isKill({ x, y }),
        }"
        @click="shoot({ x, y })"
      ></div>
    </div>
  </div>
</template>

<script lang="ts">
import { LYTERAL_COORDINATE_RU } from "@/models/consts";
import {
  EShootStatus,
  SHOOT_TARGET_STATUSES,
  type ICoordinateSimple,
} from "@/models/dto.model";
import { gameStore } from "@/stores/game.store";
import { mapStores } from "pinia";
import { defineComponent } from "vue";

export default defineComponent({
  name: "EnemyBatleArea",
  props: {
    verticalColumn: Number,
    horizontalColumn: Number,
  },
  setup() {
    return {
      EShootStatus,
      LYTERAL_COORDINATE_RU: LYTERAL_COORDINATE_RU,
    };
  },
  emits: {},
  computed: {
    verticarrArr() {
      return Array.from(Array(this.verticalColumn).keys());
    },
    horizontalArr() {
      return Array.from(Array(this.horizontalColumn).keys());
    },
    ...mapStores(gameStore),
    isMyShoot() {
      return this.gameStore.isMyShoot;
    },
  },
  methods: {
    click(coordinate: ICoordinateSimple) {
      this.$emit("click", coordinate);
    },
    statusCoodinante(coordinate: ICoordinateSimple) {
      return this.gameStore.getMyShootInHistory(coordinate)?.status;
    },
    isShoot(coordinate: ICoordinateSimple) {
      return SHOOT_TARGET_STATUSES.includes(this.statusCoodinante(coordinate));
    },
    isMiss(coordinate: ICoordinateSimple) {
      return this.statusCoodinante(coordinate) === EShootStatus.Miss;
    },
    shoot(coordinate: ICoordinateSimple) {
      if (!this.isMyShoot) {
        return;
      }
      this.gameStore.sendShoot(coordinate);
    },
    isKill(coordinate: ICoordinateSimple) {
      return this.statusCoodinante(coordinate) === EShootStatus.Killing;
    },
  },
});
</script>

<style lang="less" scoped>
@import "./battle.less";
.col-fixed {
  &.my-shoot {
    cursor: pointer;
    &:hover {
      background-color: rgba(0, 0, 0, 0.3);
    }
  }
  &.label {
    cursor: inherit;
    &:hover {
      background-color: none;
    }
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
