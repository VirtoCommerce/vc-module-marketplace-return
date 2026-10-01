<template>
  <VcWidget icon="lucide-rotate-ccw" :title="t('RETURNS.WIDGET.TITLE')" :value="totalCount" @click="onClick" />
</template>

<script lang="ts" setup>
import { computed, watch } from "vue";
import { injectBladeContext, useBladeNavigation } from "@vc-shell/framework";
import { VcWidget } from "@vc-shell/framework/ui";
import { useI18n } from "vue-i18n";
import { useReturnsList } from "../composables";

const { t } = useI18n({ useScope: "global" });
const { openBlade } = useBladeNavigation();

const bladeContext = injectBladeContext();
const orderId = computed(() => (bladeContext.value?.item as { id?: string } | undefined)?.id);

const { totalCount, loadReturns } = useReturnsList({ pageSize: 5 });

async function loadCount() {
  if (orderId.value) {
    await loadReturns({ orderId: orderId.value, skip: 0 });
  }
}

watch(orderId, loadCount, { immediate: true });

// Opens the same list blade as the main menu, scoped to this order (and the current seller,
// via useReturnsList's own currentSeller injection), so a click always shows every return for
// the order - there can be more than one. The list calls onReload whenever it reloads, so the
// badge stays in sync with returns created from it.
function onClick() {
  if (!orderId.value) {
    return;
  }

  openBlade({ blade: { name: "ReturnsList" }, options: { orderId: orderId.value, onReload: loadCount } });
}
</script>
