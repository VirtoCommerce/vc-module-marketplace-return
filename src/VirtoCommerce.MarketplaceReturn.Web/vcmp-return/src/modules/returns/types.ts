export interface ReturnLineItemCandidate {
  id: string;
  orderLineItemId: string;
  name?: string;
  sku?: string;
  imageUrl?: string;
  price?: number;
  availableQuantity: number;
}
