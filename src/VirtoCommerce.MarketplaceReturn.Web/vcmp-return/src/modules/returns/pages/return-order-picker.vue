<template>
  <VcBlade :loading="loading" :title="t('RETURNS.PAGES.ORDER_PICKER.TITLE')" width="40%">
    <VcDataTable
      v-model:sort-field="sortField"
      v-model:sort-order="sortOrder"
      :items="items"
      :total-count="totalCount"
      :pagination="{ currentPage, pages }"
      :searchable="true"
      state-key="return_order_picker"
      @row-click="onOrderClick"
      @pagination-click="onPaginationClick"
      @search="onSearchChange"
    >
      <VcColumn
        id="number"
        :title="t('RETURNS.PAGES.ORDER_PICKER.TABLE.HEADER.NUMBER')"
        :always-visible="true"
        :sortable="true"
        mobile-position="top-left"
      />

      <VcColumn id="status" :title="t('RETURNS.PAGES.ORDER_PICKER.TABLE.HEADER.STATUS')" mobile-role="status" />

      <VcColumn id="total" :title="t('RETURNS.PAGES.ORDER_PICKER.TABLE.HEADER.TOTAL')" type="money" />

      <VcColumn
        id="createdDate"
        :title="t('RETURNS.PAGES.ORDER_PICKER.TABLE.HEADER.CREATED')"
        :sortable="true"
        type="date-ago"
        mobile-position="bottom-right"
      />
    </VcDataTable>
  </VcBlade>
</template>

<script lang="ts" setup>
import { onMounted, watch } from "vue";
import { useBlade, useDataTableSort } from "@vc-shell/framework";
import { VcBlade, VcDataTable, VcColumn } from "@vc-shell/framework/ui";
import { useI18n } from "vue-i18n";
import { useOrdersPicker } from "../composables";
import type { CustomerOrder } from "@vcmp-vendor-portal/api/marketplacevendor";

defineBlade({
  name: "ReturnOrderPicker",
});

const { t } = useI18n({ useScope: "global" });
const { replaceWith } = useBlade();

const { sortField, sortOrder, sortExpression } = useDataTableSort({
  initialField: "createdDate",
  initialDirection: "DESC",
});

const { items, totalCount, pages, currentPage, searchQuery, loadOrders, loading } = useOrdersPicker({
  pageSize: 20,
});

onMounted(async () => {
  await loadOrders({ take: 20, sort: sortExpression.value });
});

watch(sortExpression, async (newVal) => {
  await loadOrders({ ...searchQuery.value, sort: newVal });
});

async function onPaginationClick(page: number) {
  await loadOrders({
    ...searchQuery.value,
    skip: (page - 1) * (searchQuery.value.take ?? 20),
  });
}

async function onSearchChange(keyword: string | undefined) {
  await loadOrders({
    ...searchQuery.value,
    keyword,
    skip: 0,
  });
}

async function onOrderClick(event: { data: CustomerOrder }) {
  const orderId = event.data.id;
  if (!orderId) {
    return;
  }
  await replaceWith({
    name: "ReturnDetails",
    options: { orderId },
  });
}
</script>
