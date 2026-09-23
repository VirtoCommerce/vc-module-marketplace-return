<template>
  <div class="tw-flex">
    <VcStatus v-bind="statusStyle(status)" :dot="$isMobile.value">
      {{ status }}
    </VcStatus>
  </div>
</template>

<script lang="ts" setup>
import { VcStatus } from "@vc-shell/framework/ui";

export interface Props {
  status: string | undefined;
}

defineProps<Props>();

const statusStyle = (status: string | undefined) => {
  const result: {
    outline: boolean;
    variant: "info" | "warning" | "danger" | "success" | "light-danger" | "info-dark" | "primary";
  } = {
    outline: true,
    variant: "info",
  };

  switch (status) {
    case "New":
      result.outline = true;
      result.variant = "info";
      break;
    case "Processing":
      result.outline = false;
      result.variant = "primary";
      break;
    case "Approved":
      result.outline = false;
      result.variant = "success";
      break;
    case "Completed":
      result.outline = false;
      result.variant = "success";
      break;
    case "Canceled":
      result.outline = true;
      result.variant = "danger";
      break;
  }
  return result;
};
</script>

<style lang="scss" scoped></style>
