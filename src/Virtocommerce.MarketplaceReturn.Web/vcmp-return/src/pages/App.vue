<template>
  <VcApp :is-ready="isReady" title="Vcmp Return" :version="version"> </VcApp>
</template>

<script lang="ts" setup>
import { useUser } from "@vc-shell/framework";
import { VcApp } from "@vc-shell/framework/ui";
import { onMounted, ref } from "vue";

const isReady = ref(false);
const version = import.meta.env.PACKAGE_VERSION;

const { isAuthenticated } = useUser();

onMounted(async () => {
  try {
    if (isAuthenticated.value) {
      isReady.value = true;
    }
  } catch (e) {
    console.log(e);
    throw e;
  }
});
</script>

<style lang="scss">
@use "./../styles/index.scss";
</style>
