import React from "react";

function ContentCard({
  children,
  cardHeader,
  disableHeader = true,
  className = "",
}) {
  return (
    <div className={`card shadow-sm ${className}`}>
      {!disableHeader && cardHeader && (
        <div className="p-3 border-bottom">{cardHeader}</div>
      )}
      <div className="card-body">{children}</div>
    </div>
  );
}

export default ContentCard;
