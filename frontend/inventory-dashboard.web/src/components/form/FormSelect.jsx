import React from "react";

export default function FormSelect({
  label,
  name,
  value,
  onChange,
  error,
  options = [],
  placeholder = "Select...",
  col = "col-md-6",
  multiple = false,
}) {
  return (
    <div className={col}>
      <label className="form-label">{label}</label>

      {error && <div className="text-danger mb-1">{error}</div>}

      <select
        name={name}
        value={value ?? ""}
        onChange={onChange}
        className={`form-select ${error ? "is-invalid" : ""}`}
        multiple={multiple}
      >
        <option value="">{placeholder}</option>

        {options.map((opt) => (
          <option key={opt.value} value={opt.value}>
            {opt.label}
          </option>
        ))}
      </select>
    </div>
  );
}
