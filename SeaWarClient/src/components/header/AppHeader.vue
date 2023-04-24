<template>
  <div class="grid h-full justify-content-between">
    <div class="col-5 md:col-3 flex-center">
      <h3>Морской бой</h3>
    </div>
    <div class="col-5 md:col-3 flex justify-content-end align-items-center">
      <User></User>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import User from "./User.vue";

export default defineComponent({
  name: "AppHeader",
  components: {
    User,
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
