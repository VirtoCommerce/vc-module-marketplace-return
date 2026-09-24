<template>
  <VcBlade :title="title" :toolbar-items="bladeToolbar" width="40%">
    <VcTable
      :items="candidates"
      :columns="columns"
      state-key="return_line_item_picker"
      multiselect
      @selection-changed="onSelectionChanged"
    />
  </VcBlade>
</template>

<script lang="ts" setup>
import { computed, ref } from "vue";
import { IBladeToolbar, ITableColumns, useBlade } from "@vc-shell/framework";
import { VcBlade, VcTable } from "@vc-shell/framework/ui";
import { useI18n } from "vue-i18n";
import { ReturnLineItemCandidate } from "../types";

defineBlade({
  name: "ReturnLineItemPicker",
});

const { t } = useI18n({ useScope: "global" });
const { closeSelf, callParent, options } = useBlade<{ candidates: ReturnLineItemCandidate[] }>();

const title = t("RETURNS.PAGES.LINE_ITEM_PICKER.TITLE");
const candidates = computed(() => options.value?.candidates ?? []);
const selectedItems = ref<ReturnLineItemCandidate[]>([]);

const columns = ref<ITableColumns[]>([
  {
    id: "imageUrl",
    title: computed(() => t("RETURNS.PAGES.LINE_ITEM_PICKER.TABLE.HEADER.IMAGE")),
    width: "60px",
    type: "image",
  },
  {
    id: "name",
    title: computed(() => t("RETURNS.PAGES.LINE_ITEM_PICKER.TABLE.HEADER.NAME")),
    alwaysVisible: true,
  },
  {
    id: "availableQuantity",
    title: computed(() => t("RETURNS.PAGES.LINE_ITEM_PICKER.TABLE.HEADER.AVAILABLE")),
  },
]);

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

function onSelectionChanged(selected: ReturnLineItemCandidate[]) {
  selectedItems.value = selected;
}
</script>
