import React from "react";

function PageTitle({title, children}) {
  return (
    <div className="d-flex align-items-center justify-content-between mb-3">
      <h1 className="h4 mb-0">{title}</h1>
      {children}
    </div>
  );
}

export default PageTitle;
