import React from "react";

function BackButton({ ...props }) {
  return (
    <button
      type="button"
      className="btn btn-outline-secondary btn-sm"
      {...props}
    >
      Back
    </button>
  );
}

export default BackButton;
