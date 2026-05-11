export function formatCHF(value) {
  if (value === null || value === undefined || Number.isNaN(Number(value))) return "—";

  return new Intl.NumberFormat("de-CH", {
    style: "currency",
    currency: "CHF",
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(Number(value));
}
