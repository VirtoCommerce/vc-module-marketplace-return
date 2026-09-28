<template>
  <VcBlade :title="title" :toolbar-items="bladeToolbar" width="50%">
    <VcDataTable
      v-model:selection="selectedItems"
      :items="candidates"
      :total-count="candidates.length"
      selection-mode="multiple"
      edit-mode="inline"
      state-key="return_line_item_picker"
    >
      <VcColumn
        id="imageUrl"
        :title="t('RETURNS.PAGES.LINE_ITEM_PICKER.TABLE.HEADER.IMAGE')"
        width="60px"
        type="image"
        class="tw-pr-0"
      />

      <VcColumn id="name" :title="t('RETURNS.PAGES.LINE_ITEM_PICKER.TABLE.HEADER.NAME')" :always-visible="true">
        <template #body="{ data }">
          <ReturnLineItemName :name="data.name" :sku="data.sku" />
        </template>
      </VcColumn>

      <VcColumn
        id="quantity"
        :title="t('RETURNS.PAGES.LINE_ITEM_PICKER.TABLE.HEADER.RETURNED')"
        :always-visible="true"
        type="number"
      >
        <template #body="{ data, index }">
          <VcInput
            :key="`quantity-${index}-${quantityRevision[index] ?? 0}`"
            type="number"
            :model-value="data.quantity"
            @update:model-value="onQuantityChange(index, $event)"
          />
        </template>
      </VcColumn>

      <VcColumn id="orderedQuantity" :title="t('RETURNS.PAGES.LINE_ITEM_PICKER.TABLE.HEADER.ORDERED')" type="number" />

      <VcColumn id="reason" :title="t('RETURNS.PAGES.LINE_ITEM_PICKER.TABLE.HEADER.REASON')">
        <template #body="{ data, index }">
          <div @keydown.space.stop>
            <VcInput :model-value="data.reason" @update:model-value="onReasonChange(index, $event)" />
          </div>
        </template>
      </VcColumn>
    </VcDataTable>
  </VcBlade>
</template>

<script lang="ts" setup>
import { computed, ref } from "vue";
import { IBladeToolbar, useBlade } from "@vc-shell/framework";
import { VcBlade, VcDataTable, VcColumn, VcInput } from "@vc-shell/framework/ui";
import { useI18n } from "vue-i18n";
import { ReturnLineItemCandidate } from "../types";
import { ReturnLineItemName } from "../components";

defineBlade({
  name: "ReturnLineItemPicker",
});

const { t } = useI18n({ useScope: "global" });
const { closeSelf, callParent, options } = useBlade<{ candidates: ReturnLineItemCandidate[] }>();

const title = t("RETURNS.PAGES.LINE_ITEM_PICKER.TITLE");

const candidates = ref<ReturnLineItemCandidate[]>(
  (options.value?.candidates ?? []).map((candidate) => ({
    ...candidate,
    quantity: candidate.orderedQuantity,
    reason: candidate.reason ?? "",
  })),
);
const selectedItems = ref<ReturnLineItemCandidate[]>([]);

const bladeToolbar = computed((): IBladeToolbar[] => [
  {
    id: "confirm",
    title: t("RETURNS.PAGES.LINE_ITEM_PICKER.TOOLBAR.CONFIRM"),
    icon: "material-check",
    disabled: selectedItems.value.length === 0,
    clickHandler() {
      callParent("addLineItems", { items: selectedItems.value });
      closeSelf();
    },
  },
]);

function onReasonChange(index: number, value: unknown) {
  const candidate = candidates.value[index];
  if (!candidate) {
    return;
  }

  candidate.reason = value as string;
}

const quantityRevision = ref<number[]>([]);

function onQuantityChange(index: number, value: unknown) {
  const candidate = candidates.value[index];
  if (!candidate) {
    return;
  }

  const max = candidate.orderedQuantity;
  const numeric = Number(value);
  const clamped = Math.min(Math.max(Number.isFinite(numeric) ? numeric : 0, 0), max);
  if (clamped !== numeric) {
    quantityRevision.value[index] = (quantityRevision.value[index] ?? 0) + 1;
  }
  candidate.quantity = clamped;
}
</script>
