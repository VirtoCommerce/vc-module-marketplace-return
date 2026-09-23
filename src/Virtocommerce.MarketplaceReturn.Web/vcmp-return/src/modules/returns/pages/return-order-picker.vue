<template>
  <VcBlade :title="t('RETURNS.PAGES.ORDER_PICKER.TITLE')" :toolbar-items="bladeToolbar" width="40%">
    <VcContainer>
      <div class="tw-p-4 tw-space-y-4">
        <VcInput
          v-model="orderId"
          :label="t('RETURNS.PAGES.ORDER_PICKER.ORDER_ID')"
          :placeholder="t('RETURNS.PAGES.ORDER_PICKER.ORDER_ID_PLACEHOLDER')"
        />
      </div>
    </VcContainer>
  </VcBlade>
</template>

<script lang="ts" setup>
// NOTE: this picks an order by id only. Swap the input above for a real order search once this
// sub-app depends on @vcmp-vendor-portal/api's Orders search client (vendor-portal's own Orders
// module already has one - it wasn't available to bind against safely while scaffolding this).
import { computed, ref } from "vue";
import { IBladeToolbar, useBlade } from "@vc-shell/framework";
import { VcBlade, VcContainer, VcInput } from "@vc-shell/framework/ui";
import { useI18n } from "vue-i18n";

defineBlade({
  name: "ReturnOrderPicker",
});

const { t } = useI18n({ useScope: "global" });
const { openBlade, closeSelf } = useBlade();

const orderId = ref("");

const bladeToolbar = computed((): IBladeToolbar[] => [
  {
    id: "continue",
    title: t("RETURNS.PAGES.ORDER_PICKER.CONTINUE"),
    icon: "material-check",
    disabled: !orderId.value,
    async clickHandler() {
      const selectedOrderId = orderId.value;
      closeSelf();
      await openBlade({
        name: "ReturnDetails",
        options: { orderId: selectedOrderId },
      });
    },
  },
]);
</script>
