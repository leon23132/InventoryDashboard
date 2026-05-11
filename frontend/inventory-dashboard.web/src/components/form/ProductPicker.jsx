import React, { useMemo, useState } from "react";

export default function ProductPicker({
  products = [],
  onQuantityChange,
  selectedIds = [],
  onToggle,
  onRemove,
  col = "col-12",
  title = "Assign Products",
  selectedTitle = "Selected Products:",
  placeholder = "Search products...",
}) {
  const [query, setQuery] = useState("");

  const normalized = useMemo(() => {
    return (products ?? []).map((p) => {
      const id = p.productId ?? p.id;
      const label =
        p.productTitle ?? p.productName ?? p.name ?? `Product ${id}`;
      return { id, label };
    });
  }, [products]);

  const selectedMap = useMemo(() => {
    const map = new Map();
    (selectedIds ?? []).forEach((p) => {
      map.set(p.productId, p);
    });
    return map;
  }, [selectedIds]);

  const selectedItems = useMemo(
    () => normalized.filter((x) => selectedMap.has(x.id)),
    [normalized, selectedMap],
  );

  const filtered = useMemo(() => {
    const q = (query ?? "").trim().toLowerCase();
    if (!q) return normalized;
    return normalized.filter((x) => x.label.toLowerCase().includes(q));
  }, [normalized, query]);

  return (
    <div className={`${col} mb-3 d-flex flex-column`}>
      {/* Selected chips */}
      <div className="mb-3">
        <div className="form-label mb-2 fw-semibold">{selectedTitle}</div>

        <div className="border rounded p-2">
          {selectedItems.length === 0 ? (
            <span className="text-muted small">No products selected</span>
          ) : (
            <div className="d-flex flex-wrap gap-2">
              {selectedItems.map((x) => (
                <span
                  key={x.id}
                  className="badge rounded-pill bg-light text-dark border px-3 py-2 d-inline-flex align-items-center"
                  style={{ fontWeight: 500 }}
                >
                  <span className="me-2">{x.label}</span>
                  <span className="text-muted" style={{ fontWeight: 500 }}>
                    / Quantity: {selectedMap.get(x.id)?.quantity ?? 1}
                  </span>

                  <button
                    type="button"
                    className="btn btn-sm btn-link text-muted ms-2 p-0"
                    onClick={() =>
                      onRemove ? onRemove(x.id) : onToggle?.(x.id)
                    }
                    aria-label={`Remove ${x.label}`}
                    style={{ textDecoration: "none", lineHeight: 1 }}
                  >
                    ×
                  </button>
                </span>
              ))}
            </div>
          )}
        </div>
      </div>

      <hr className="my-3" />

      {/* Assign header + counters */}
      <div className="d-flex align-items-center justify-content-between mb-2">
        <div className="h5 mb-0">{title}</div>
      </div>

      <form
        className="input-group input-group-sm mb-2"
        onSubmit={(e) => {
          e.preventDefault();
          // optional: hier könntest du später "Search Submit" machen,
          // aktuell filtert es ja live beim tippen, also leer lassen
        }}
      >
        <input
          type="text"
          className="form-control"
          placeholder={placeholder}
          value={query}
          onChange={(e) => setQuery(e.target.value)}
        />
        <button className="input-group-text" type="submit" title="Search">
          🔍
        </button>
      </form>

      {/* Checkbox list */}
      <div
        className="border rounded overflow-auto"
        style={{ maxHeight: "340px" }}
      >
        {filtered.length === 0 ? (
          <div className="p-3 text-muted">No matching products.</div>
        ) : (
          filtered.map((x) => {
            const selected = selectedMap.get(x.id);
            const checked = !!selected;

            return (
              <div
                key={x.id}
                className="d-flex align-items-center px-3 py-1 border-bottom"
                style={{ gap: 12 }}
              >
                <input
                  className="form-check-input m-0"
                  type="checkbox"
                  id={`prod-${x.id}`}
                  checked={checked}
                  onChange={() => onToggle?.(x.id)}
                  readOnly={!onToggle} // damit keine Warning falls noch keine Logik
                  style={{ width: 20, height: 20 }}
                />
                <label
                  className="mb-0"
                  htmlFor={`prod-${x.id}`}
                  style={{ cursor: "pointer" }}
                >
                  {x.label}
                </label>

                {checked && (
                  <input
                    type="number"
                    min="1"
                    className="form-control form-control-sm ms-auto"
                    style={{ width: 80 }}
                    value={selected?.quantity ?? 1}
                    onChange={(e) => onQuantityChange?.(x.id, e.target.value)}
                  />
                )}
              </div>
            );
          })
        )}
      </div>
    </div>
  );
}
