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

watch(
  orderId,
  async (id) => {
    if (id) {
      await loadReturns({ orderId: id, skip: 0 });
    }
  },
  { immediate: true },
);

// Opens the same list blade as the main menu, scoped to this order (and the current seller,
// via useReturnsList's own currentSeller injection), so a click always shows every return for
// the order - there can be more than one.
function onClick() {
  if (!orderId.value) {
    return;
  }

  openBlade({ blade: { name: "ReturnsList" }, options: { orderId: orderId.value } });
}
</script>
