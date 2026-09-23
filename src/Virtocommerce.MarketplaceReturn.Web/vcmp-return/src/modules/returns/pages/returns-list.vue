<template>
  <VcBlade :loading="loading" :title="title" :toolbar-items="bladeToolbar" width="30%">
    <VcDataTable
      v-model:active-item-id="selectedItemId"
      v-model:sort-field="sortField"
      v-model:sort-order="sortOrder"
      class="tw-grow tw-basis-0"
      :items="items"
      :total-count="totalCount"
      :total-label="$t('RETURNS.PAGES.LIST.TABLE.TOTALS')"
      :pagination="{ currentPage, pages }"
      :global-filters="globalFilters"
      :show-all-columns="expanded"
      state-key="RETURNS"
      :searchable="true"
      @row-click="onItemClick"
      @pagination-click="onPaginationClick"
      @search="onSearchChange"
      @filter="onFilter"
    >
      <VcColumn
        id="number"
        :title="t('RETURNS.PAGES.LIST.TABLE.HEADER.NUMBER')"
        :always-visible="true"
        :sortable="true"
        mobile-position="top-left"
      />

      <VcColumn
        id="orderId"
        :title="t('RETURNS.PAGES.LIST.TABLE.HEADER.ORDER_NUMBER')"
        :always-visible="true"
        mobile-position="bottom-left"
      >
        <template #body="{ data }">
          {{ data.order?.number || data.orderId }}
        </template>
      </VcColumn>

      <VcColumn
        id="status"
        :title="t('RETURNS.PAGES.LIST.TABLE.HEADER.STATUS')"
        :always-visible="true"
        :sortable="true"
        type="status"
        mobile-role="status"
      >
        <template #body="{ data }">
          <ReturnStatusTemplate :status="data.status" />
        </template>
      </VcColumn>

      <VcColumn
        id="createdDate"
        :title="t('RETURNS.PAGES.LIST.TABLE.HEADER.CREATED')"
        :sortable="true"
        type="date-ago"
        mobile-position="bottom-right"
      />
    </VcDataTable>
  </VcBlade>
</template>

<script lang="ts" setup>
import { computed, ref, watch, onMounted } from "vue";
import { IBladeToolbar, useBlade, useDataTableSort } from "@vc-shell/framework";
import { VcBlade, VcDataTable, VcColumn } from "@vc-shell/framework/ui";
import { useI18n } from "vue-i18n";
import { useReturnsList } from "../composables";
import { Return } from "../../../api_client/virtocommerce.marketplacereturn";
import { ReturnStatusTemplate } from "../components";

defineBlade({
  url: "/returns",
  name: "ReturnsList",
  isWorkspace: true,
  permissions: ["marketplace-return:access"],
  menuItem: {
    title: "RETURNS.MENU.TITLE",
    icon: "material-assignment_return",
    priority: 2,
  },
});

const { t } = useI18n({ useScope: "global" });
const { openBlade, expanded, param, exposeToChildren, options } = useBlade<{ orderId?: string }>();

const { sortField, sortOrder, sortExpression } = useDataTableSort({
  initialField: "createdDate",
  initialDirection: "DESC",
});

const { items, totalCount, pages, currentPage, searchQuery, loadReturns, loading, statuses } = useReturnsList({
  pageSize: 20,
  sort: sortExpression.value,
  orderId: options.value?.orderId,
});

const title = computed(() => t("RETURNS.PAGES.LIST.TITLE"));
const selectedItemId = ref<string>();

const globalFilters = computed(() => [
  {
    id: "status",
    label: t("RETURNS.PAGES.LIST.TABLE.FILTER.STATUS.TITLE"),
    filter: {
      options: statuses.map((status) => ({ value: status, label: status })),
    },
  },
]);

const bladeToolbar = computed((): IBladeToolbar[] => [
  {
    id: "add",
    icon: "material-add",
    title: t("RETURNS.PAGES.LIST.TOOLBAR.ADD"),
    permissions: ["marketplace-return:create"],
    clickHandler() {
      if (options.value?.orderId) {
        openBlade({
          name: "ReturnDetails",
          options: { orderId: options.value.orderId },
        });
      } else {
        openBlade({
          name: "ReturnOrderPicker",
        });
      }
    },
  },
  {
    id: "refresh",
    icon: "material-refresh",
    title: t("RETURNS.PAGES.LIST.TOOLBAR.REFRESH"),
    async clickHandler() {
      await reload();
    },
  },
]);

onMounted(async () => {
  await loadReturns({
    take: 20,
    sort: sortExpression.value,
  });
});

watch(sortExpression, async (newVal) => {
  await loadReturns({ ...searchQuery.value, sort: newVal });
});

watch(
  () => param.value,
  (newVal) => {
    selectedItemId.value = newVal;
  },
  { immediate: true, deep: true },
);

async function onItemClick(event: { data: Return }) {
  const item = event.data;
  openBlade({
    name: "ReturnDetails",
    param: item.id,
    onOpen() {
      selectedItemId.value = item.id;
    },
    onClose() {
      selectedItemId.value = undefined;
    },
  });
}

function openDetailsBlade(args: { param: string }) {
  openBlade({
    name: "ReturnDetails",
    param: args.param,
    onOpen() {
      selectedItemId.value = args.param;
    },
    onClose() {
      selectedItemId.value = undefined;
    },
  });
}

async function onPaginationClick(page: number) {
  await loadReturns({
    ...searchQuery.value,
    skip: (page - 1) * (searchQuery.value.take ?? 20),
  });
}

async function onSearchChange(keyword: string | undefined) {
  await loadReturns({
    ...searchQuery.value,
    keyword,
    skip: 0,
  });
}

async function onFilter(event: { filters: Record<string, unknown> }) {
  const status = event.filters.status as string | undefined;
  await loadReturns({
    ...searchQuery.value,
    statuses: status ? [status] : undefined,
    skip: 0,
  });
}

const reload = async () => {
  await loadReturns({
    ...searchQuery.value,
    skip: (currentPage.value - 1) * (searchQuery.value.take ?? 20),
    sort: sortExpression.value,
  });
};

exposeToChildren({
  reload,
  onItemClick: (item: Return) => onItemClick({ data: item }),
  openDetailsBlade,
});
</script>
