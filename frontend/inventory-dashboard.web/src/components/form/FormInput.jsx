import React from "react";

export default function FormInput({
  label,
  name,
  value,
  onChange,
  error,
  type = "text",
  col = "col-md-6",
}) {
  return (
    <div className={col}>
      <label className="form-label mb-2">{label}</label>

      {error && <div className="text-danger mb-1">{error}</div>}

      <input
        type={type}
        name={name}
        value={value ?? ""}
        onChange={onChange}
        className={`form-control ${error ? "is-invalid" : ""}`}
      />
    </div>
  );
}
