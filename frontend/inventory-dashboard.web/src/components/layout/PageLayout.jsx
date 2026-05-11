import React, { Children } from "react";

function PageLayout({ children }) {
  return <div className="container py-3">{children}</div>;
}

export default PageLayout;
