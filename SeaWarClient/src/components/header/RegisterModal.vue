<template>
  <div>
    <div class="flex flex-column gap-2">
      <label for="username">Name</label>
      <InputText v-model="name" />
    </div>
    <div class="flex flex-column gap-2">
      <label for="username">Email</label>
      <InputText v-model="email" />
    </div>
    <div class="flex flex-column gap-2">
      <label for="username">Password</label>
      <Password v-model="password" :feedback="true" />
    </div>
    <div class="flex flex-column gap-2">
      <label for="username">Confirm Password</label>
      <Password v-model="confirmPassword" :feedback="false" />
    </div>
    <div class="flex flex-column gap-2 mt-2">
      <Button
        class="btn btn-primary"
        v-bind:disabled="!isFormValid"
        @click="register()"
        label="Регистрация"
      />
    </div>
  </div>
</template>

<script lang="ts">
import { userStore } from "@/stores/user.store";
import { mapStores } from "pinia";
import { defineComponent } from "vue";

export default defineComponent({
  name: "RegisterModal",
  emits: ["close"],
  data() {
    return {
      name: null as string,
      email: null as string,
      password: null as string,
      confirmPassword: null as string,
      errors: [] as Array<string>,
    };
  },
  computed: {
    ...mapStores(userStore),
    isFormValid() {
      return (
        !!this.name &&
        !!this.email &&
        !!this.password &&
        !!this.confirmPassword &&
        this.password === this.confirmPassword
      );
    },
  },
  methods: {
    register() {
      this.errors = [];
      this.userStore
        .create(this.name, this.email, this.password)
        .then(() => this.$emit("close"));
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
