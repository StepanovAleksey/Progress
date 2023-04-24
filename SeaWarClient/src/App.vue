<template>
  <div class="grid grid-nogutter h-full flex-column">
    <div class="col-12 border"><AppHeader></AppHeader></div>
    <div class="col-12 flex-grow-1 scroll">
      <div class="grid h-full grid-nogutter">
        <div class="col-2"><Chat></Chat></div>
        <div class="col flex flex-column">
          <div class="grid grid-nogutter">
            <div
              class="col-12 font-bold text-blue-500 text-3xl flex align-items-center justify-content-center"
            >
              {{ gameDescription[gameStore.gameStatus] }}
              <span v-if="isGame"
                >: {{ isMyShoot ? "ваш ход" : "ход соперника" }}</span
              >
            </div>
          </div>
          <div class="grid grid-nogutter flex-grow-1 scroll">
            <div class="col-6 flex align-items-center justify-content-center">
              <div class="flex flex-column">
                <div class="text-lg">Ваше поле {{ myConnectionId }}</div>
                <div class="flex-grow-1">
                  <UserBattle
                    :horizontal-column="HORIZONTAL_AREA_LENGTH"
                    :vertical-column="VERTICAL_AREA_LENGTH"
                    :ships="ships"
                    :game-status="gameStore.gameStatus"
                  >
                  </UserBattle>
                </div>
              </div>
            </div>
            <div class="col-6 flex align-items-center justify-content-center">
              <div class="flex flex-column">
                <Button
                  v-if="gameStore.gameStatus === EStatusGame.Init"
                  style="height: 60px"
                  label="Начать игру"
                  @click="startGame()"
                />
                <Button
                  v-if="gameStore.gameStatus === EStatusGame.Init"
                  style="height: 60px"
                  class="mt-2"
                  label="Начать игру с ботом"
                  @click="startGameWithBot()"
                />
              </div>
              <div v-if="gameStore.gameStatus === EStatusGame.Find">
                Ищем игрока<br />
                <ProgressSpinner />
              </div>
              <div v-if="gameStore.gameStatus === EStatusGame.EndGame">
                <div v-if="gameStore.userWinnerId === myConnectionId">
                  Поздравляем вы победили!!
                </div>
                <div v-else>вы проиграли, так бывает(</div>
                <Button @click="refresh()">Повторить</Button>
              </div>
              <div
                class="flex flex-column"
                v-if="gameStore.gameStatus === EStatusGame.Game"
              >
                <div>Соперник: {{ gameStore.enemyGamerId }}</div>
                <div class="flex-grow-1">
                  <RivalBatle
                    :horizontal-column="HORIZONTAL_AREA_LENGTH"
                    :vertical-column="VERTICAL_AREA_LENGTH"
                  >
                  </RivalBatle>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  <DynamicDialog />
  <Toast position="bottom-right" />
</template>

<script lang="ts">
import { defineComponent } from "vue";
import AppHeader from "./components/header/AppHeader.vue";
import Chat from "./components/Chat.vue";
import UserBattle from "./components/battle/CurrentBattleArea.vue";
import RivalBatle from "./components/battle/EnemyBatleArea.vue";
import { SHIPS, type IShip } from "./models/IShip.model";
import { HORIZONTAL_AREA_LENGTH, VERTICAL_AREA_LENGTH } from "./models/consts";
import { ShipHelper } from "./services/ship.helper";
import { gameStore, EStatusGame } from "./stores/game.store";
import { mapStores } from "pinia";
import { useToast } from "primevue/usetoast";
import { messageService } from "./services/message.service";

const timeMock = 30;
export default defineComponent({
  components: {
    AppHeader,
    Chat,
    UserBattle,
    RivalBatle,
  },
  setup() {
    return {
      toast: useToast(),
      HORIZONTAL_AREA_LENGTH,
      VERTICAL_AREA_LENGTH,
      shipHelper: new ShipHelper(
        SHIPS,
        HORIZONTAL_AREA_LENGTH,
        VERTICAL_AREA_LENGTH
      ),
      EStatusGame,
      gameDescription: {
        [EStatusGame.Init]: "Разметка кораблей",
        [EStatusGame.Find]: "Поиск игрока",
        [EStatusGame.Game]: "Идёт игра",
        [EStatusGame.EndGame]: "Игра закончилась",
      },
    };
  },
  data() {
    return {
      ships: [] as Array<IShip>,
      seconds: timeMock,
      myConnectionId: null as string,
    };
  },
  computed: {
    ...mapStores(gameStore),
    isMyShoot() {
      return this.gameStore.isMyShoot;
    },
    isGame() {
      return this.gameStore.gameStatus === EStatusGame.Game;
    },
  },
  mounted() {
    this.setRandomShips();
    messageService.messageBus.subscribe((message) => {
      this.toast.add(message);
    });
    this.gameStore.connectionId$.subscribe((connectionId) => {
      this.myConnectionId = connectionId;
    });
  },
  methods: {
    setRandomShips() {
      this.ships = this.shipHelper.setRandomCoordinate();
    },
    startGame() {
      this.gameStore.gameStart(this.ships.map((s) => s.toDto()));
    },
    refresh() {
      this.setRandomShips();
      this.gameStore.refreshGame();
    },
    startGameWithBot() {
      this.gameStore.gameStart(
        this.ships.map((s) => s.toDto()),
        true
      );
    },
  },
});
</script>

<style lang="less" scoped>
.move {
  background-color: #a8cf97;
}
</style>
