<template>
  <div>
    <div class="flex flex-column gap-2">
      <label for="username">Email</label>
      <InputText v-model="email" />
    </div>
    <div class="flex flex-column gap-2">
      <label for="username">Password</label>
      <Password v-model="password" :feedback="false" />
    </div>
    <div class="flex flex-column gap-2 mt-2">
      <Button
        class="btn btn-primary"
        v-bind:disabled="!email || !password"
        @click="auth()"
        label="Войти"
      />
    </div>
  </div>
</template>

<script lang="ts">
import { userStore } from "@/stores/user.store";
import { mapStores } from "pinia";
import { defineComponent } from "vue";

export default defineComponent({
  name: "AuthModal",
  computed: {
    ...mapStores(userStore),
  },
  inject: ["dialogRef"],
  data() {
    return {
      email: null as string,
      password: null as string,
      errors: [] as Array<string>,
    };
  },
  methods: {
    auth() {
      this.errors = [];
      this.userStore
        .login(this.email, this.password)
        .then(() => {
          // @ts-ignore
          this.dialogRef.close();
        })
        .catch((error) => {
          if (error.response?.data?.errors) {
            Object.values(error.response.data.errors).forEach((values) => {
              this.errors.push(...(values as string[]));
            });
            return;
          }
          this.errors.push(error.response.data);
        });
    },
  },
});
</script>

<style lang="less" scoped>
.flex-center {
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
