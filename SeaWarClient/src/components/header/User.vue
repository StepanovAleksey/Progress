<template>
  <template v-if="!userStore.token">
    <Button style="height: 40px" label="Войти" @click="showLogin()" />
    <Button
      style="height: 40px"
      class="ml-1"
      label="Регистарция"
      @click="showRegister()"
    />
  </template>
  <template v-if="userStore.token">
    <SplitButton :label="userStore.name" icon="pi pi-user" :model="items" />
  </template>
</template>

<script lang="ts">
import { userStore } from "@/stores/user.store";
import { mapStores } from "pinia";
import { defineComponent } from "vue";
import AuthModal from "./AuthModal.vue";
import RegisterModal from "./RegisterModal.vue";

export default defineComponent({
  name: "User",
  components: {},
  data() {
    return {
      items: [
        {
          label: "Статистика",
          icon: "pi pi-chart-bar",
          command: () => {},
        },
        {
          label: "Выйти",
          icon: "pi pi-sign-out",
          command: () => {
            this.logout();
          },
        },
      ],
    };
  },
  computed: {
    ...mapStores(userStore),
  },
  methods: {
    showLogin() {
      this.$dialog.open(AuthModal, {
        props: {
          header: "Авторизация",
          modal: true,
        },
      });
    },
    showRegister() {
      this.$dialog.open(RegisterModal, {
        props: {
          header: "Регистрация",
          modal: true,
        },
      });
    },
    logout() {
      this.userStore.close();
    },
  },
});
</script>

<style lang="less" scoped>
.field {
  display: flex;
  align-items: center;
  justify-content: center;
}
.dropdown-menu {
  li {
    cursor: pointer;
    &:hover {
      background-color: rgba(0, 0, 0, 0.3);
    }
  }
}
</style>
