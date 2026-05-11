import React from "react";

export default function DetailField({ label, children }) {
  return (
    <>
      <dt className="col-sm-3 fw-semibold">{label}</dt>
      <dd className="col-sm-9 mb-1">
        {children ?? <span className="text-muted">-</span>}
      </dd>
    </>
  );
}
