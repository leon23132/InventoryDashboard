import React from "react";

export default function DetailSection({ title, children, showDivider = true }) {
  return (
    <>
      {title && <div className="fw-semibold mb-2 mt-3">{title}</div>}

      <dl className="row mb-0">{children}</dl>

      {showDivider && <hr className="my-3" />}
    </>
  );
}
