import React from "react";

export default function FormLayout({
  children,
  onSubmit,
  className = "",
  showActions = true,
  submitLabel = "Save",
  loading = false,
  onCancel,
}) {
  return (
    <form
      onSubmit={(e) => {
        e.preventDefault();
        onSubmit?.();
      }}
      className={`row g-2 ${className}`}
    >
      {children}
      {showActions && (
        <div className="col-12 d-flex gap-2 mt-2">
        <button
          type="submit"
          className="btn btn-primary"
          disabled={loading}
        >
          {submitLabel}
        </button>

        <button
          type="button"
          className="btn btn-outline-secondary"
          onClick={onCancel}
        >
          Cancel
        </button>
      </div>
      )}
    </form>
  );
}
